using System.Reflection;
using Izi.FluentData.Transformer;

// Placed in the DI namespace so the helpers surface wherever IServiceCollection is in scope.
namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// <see cref="IServiceCollection"/> extensions for registering <see cref="ITransformer{T}"/>
/// implementations, including single-type, instance, factory, and assembly-scanning overloads.
/// </summary>
public static class TransformerServiceCollectionExtensions
{
    /// <summary>
    /// Registers <typeparamref name="TImplementation"/> for every <see cref="ITransformer{T}"/>
    /// it implements. Defaults to a singleton: a transformer compiles its property
    /// getters/setters and builds its pipelines once in the constructor and is then
    /// stateless and thread-safe, so a single shared instance is ideal.
    /// </summary>
    public static IServiceCollection AddTransformer<TImplementation>(this IServiceCollection services, ServiceLifetime lifetime = ServiceLifetime.Singleton) where TImplementation : class
        => services.AddTransformer(typeof(TImplementation), lifetime);

    /// <summary>Non-generic variant of <see cref="AddTransformer{TImplementation}"/>.</summary>
    public static IServiceCollection AddTransformer(this IServiceCollection services, Type implementationType, ServiceLifetime lifetime = ServiceLifetime.Singleton)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(implementationType);

        var serviceTypes = GetTransformerInterfaces(implementationType);
        if (serviceTypes.Count == 0)
            throw new ArgumentException(
                $"{implementationType.Name} does not implement ITransformer<T>.", nameof(implementationType));

        // Register the concrete type once...
        services.Add(new ServiceDescriptor(implementationType, implementationType, lifetime));

        // ...then forward each ITransformer<T> to that registration so a singleton is
        // never instantiated twice (and the concrete type stays resolvable on its own).
        foreach (var serviceType in serviceTypes)
        {
            services.Add(new ServiceDescriptor(serviceType, sp => sp.GetRequiredService(implementationType), lifetime));
        }

        return services;
    }

    /// <summary>Registers a pre-built transformer instance as a singleton.</summary>
    public static IServiceCollection AddTransformer<T>(this IServiceCollection services, ITransformer<T> instance)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(instance);

        services.Add(new ServiceDescriptor(typeof(ITransformer<T>), instance));
        return services;
    }

    /// <summary>Registers a transformer produced by a factory.</summary>
    public static IServiceCollection AddTransformer<T>(this IServiceCollection services, Func<IServiceProvider, ITransformer<T>> factory, ServiceLifetime lifetime = ServiceLifetime.Singleton)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(factory);

        services.Add(new ServiceDescriptor(typeof(ITransformer<T>), factory, lifetime));
        return services;
    }

    /// <summary>
    /// Scans the given assemblies and registers every concrete <see cref="ITransformer{T}"/>
    /// implementation found.
    /// </summary>
    public static IServiceCollection AddTransformers(this IServiceCollection services, ServiceLifetime lifetime, params Assembly[] assemblies)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(assemblies);

        foreach (var type in assemblies.SelectMany(a => a.DefinedTypes))
        {
            if (type is { IsAbstract: false, IsInterface: false, IsGenericTypeDefinition: false }
                && GetTransformerInterfaces(type).Count > 0)
            {
                services.AddTransformer(type, lifetime);
            }
        }

        return services;
    }

    private static List<Type> GetTransformerInterfaces(Type type)
        => type.GetInterfaces()
            .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(ITransformer<>))
            .ToList();
}
