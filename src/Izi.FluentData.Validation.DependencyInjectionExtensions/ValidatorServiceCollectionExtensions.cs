using System.Reflection;
using Izi.FluentData.Validation;

// Placed in the DI namespace so the helpers surface wherever IServiceCollection is in scope.
namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// <see cref="IServiceCollection"/> extensions for registering <see cref="IValidator{T}"/>
/// implementations, including single-type, instance, factory, and assembly-scanning overloads.
/// </summary>
public static class ValidatorServiceCollectionExtensions
{
    /// <summary>
    /// Registers <typeparamref name="TImplementation"/> for every <see cref="IValidator{T}"/>
    /// it implements. Defaults to a singleton: a validator builds its rule set once in the
    /// constructor and is then stateless and thread-safe, so a single shared instance is ideal.
    /// </summary>
    public static IServiceCollection AddValidator<TImplementation>(this IServiceCollection services, ServiceLifetime lifetime = ServiceLifetime.Singleton) where TImplementation : class
        => services.AddValidator(typeof(TImplementation), lifetime);

    /// <summary>Non-generic variant of <see cref="AddValidator{TImplementation}"/>.</summary>
    public static IServiceCollection AddValidator(this IServiceCollection services, Type implementationType, ServiceLifetime lifetime = ServiceLifetime.Singleton)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(implementationType);

        var serviceTypes = GetValidatorInterfaces(implementationType);
        if (serviceTypes.Count == 0)
            throw new ArgumentException(
                $"{implementationType.Name} does not implement IValidator<T>.", nameof(implementationType));

        // Register the concrete type once...
        services.Add(new ServiceDescriptor(implementationType, implementationType, lifetime));

        // ...then forward each IValidator<T> to that registration so a singleton is
        // never instantiated twice (and the concrete type stays resolvable on its own).
        foreach (var serviceType in serviceTypes)
        {
            services.Add(new ServiceDescriptor(serviceType, sp => sp.GetRequiredService(implementationType), lifetime));
        }

        return services;
    }

    /// <summary>Registers a pre-built validator instance as a singleton.</summary>
    public static IServiceCollection AddValidator<T>(this IServiceCollection services, IValidator<T> instance)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(instance);

        services.Add(new ServiceDescriptor(typeof(IValidator<T>), instance));
        return services;
    }

    /// <summary>Registers a validator produced by a factory.</summary>
    public static IServiceCollection AddValidator<T>(this IServiceCollection services, Func<IServiceProvider, IValidator<T>> factory, ServiceLifetime lifetime = ServiceLifetime.Singleton)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(factory);

        services.Add(new ServiceDescriptor(typeof(IValidator<T>), factory, lifetime));
        return services;
    }

    /// <summary>
    /// Scans the given assemblies and registers every concrete <see cref="IValidator{T}"/>
    /// implementation found.
    /// </summary>
    public static IServiceCollection AddValidators(this IServiceCollection services, ServiceLifetime lifetime, params Assembly[] assemblies)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(assemblies);

        foreach (var type in assemblies.SelectMany(a => a.DefinedTypes))
        {
            if (type is { IsAbstract: false, IsInterface: false, IsGenericTypeDefinition: false }
                && GetValidatorInterfaces(type).Count > 0)
            {
                services.AddValidator(type, lifetime);
            }
        }

        return services;
    }

    private static List<Type> GetValidatorInterfaces(Type type)
        => [.. type.GetInterfaces().Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IValidator<>))];
}
