namespace Izi.FluentData.Validation;

/// <summary>Runs a nested validator one level deeper than the caller.</summary>
internal static class ChildValidation
{
    /// <summary>Validates <paramref name="value"/> at <paramref name="depth"/> + 1.</summary>
    /// <typeparam name="T">The child value type.</typeparam>
    /// <param name="child">The child validator.</param>
    /// <param name="value">The value. The caller skips null.</param>
    /// <param name="cancellationToken">Passed to the child.</param>
    /// <param name="depth">The caller's depth.</param>
    /// <returns>The child result.</returns>
    internal static ValueTask<ValidationResult> Run<T>(
        IValidator<T> child,
        T value,
        CancellationToken cancellationToken,
        int depth)
    {
        var next = depth + 1;
        ValidationLimits.Check(next);
        if (child is Validator<T> typed)
        {
            return typed.ValidateNested(value, cancellationToken, next);
        }

        return RunCustom(child, value, cancellationToken, next);
    }

    private static ValueTask<ValidationResult> RunCustom<T>(
        IValidator<T> child,
        T value,
        CancellationToken cancellationToken,
        int next)
    {
        // Restore on this frame. The child's continuations already captured the context while next was set.
        // Restoring inside an async state machine would leave next in the caller's flow.
        var previous = ValidationDepth.Current;
        ValidationDepth.Current = next;
        try
        {
            return child.ValidateAsync(value, cancellationToken);
        }
        finally
        {
            ValidationDepth.Current = previous;
        }
    }
}

/// <summary>Shared pieces of a validation run. The success path does not call the methods that allocate.</summary>
internal static class ValidationRun
{
    /// <summary>The message used when an intermediate parent is null.</summary>
    internal const string MustNotBeNull = "Must not be null.";

    /// <summary>A null failure list is the shared success result.</summary>
    /// <param name="failures">Failures so far, or null.</param>
    /// <returns>The result.</returns>
    internal static ValidationResult ToResult(List<PropertyValidationResult>? failures)
        => failures is null ? ValidationResult.Success : new ValidationResult(failures);

    /// <summary>Records one message.</summary>
    /// <param name="failures">Earlier failures, or null.</param>
    /// <param name="propertyName">The property the message belongs to.</param>
    /// <param name="message">The message. It does not name a property.</param>
    /// <returns>The list that now contains the failure.</returns>
    internal static List<PropertyValidationResult> AddMessage(
        List<PropertyValidationResult>? failures,
        string propertyName,
        string message)
    {
        failures ??= [];
        failures.Add(new PropertyValidationResult(propertyName, new ValidationError(message)));
        return failures;
    }

    /// <summary>Records every message, or returns <paramref name="failures"/> when there are none.</summary>
    /// <param name="failures">Earlier failures, or null.</param>
    /// <param name="propertyName">The property the messages belong to.</param>
    /// <param name="messages">The messages, or null when the property passed.</param>
    /// <returns>The failure list.</returns>
    internal static List<PropertyValidationResult>? AddMessages(
        List<PropertyValidationResult>? failures,
        string propertyName,
        List<string>? messages)
    {
        if (messages is null)
        {
            return failures;
        }

        failures ??= new List<PropertyValidationResult>(1);
        failures.Add(new PropertyValidationResult(propertyName, new ValidationError(messages)));
        return failures;
    }

    /// <summary>Appends the messages of a failed rule. A passing rule leaves <paramref name="messages"/> unchanged.</summary>
    /// <param name="messages">Messages already collected for this property, or null.</param>
    /// <param name="result">The rule result.</param>
    /// <param name="replacement">The message from <c>WithMessage</c>, or null to keep the rule's own messages.</param>
    /// <returns>The collected messages.</returns>
    internal static List<string>? Collect(List<string>? messages, ValidationRuleResult result, string? replacement)
    {
        if (result.IsValid)
        {
            return messages;
        }

        messages ??= [];
        if (replacement is not null)
        {
            messages.Add(replacement);
            return messages;
        }

        var errors = result.Errors;
        for (var i = 0; i < errors.Count; i++)
        {
            messages.Add(errors[i]);
        }

        return messages;
    }

    /// <summary>Prefixes each failed child property with <paramref name="propertyName"/>.</summary>
    /// <param name="failures">Earlier failures, or null.</param>
    /// <param name="propertyName">The parent property name.</param>
    /// <param name="child">The child validator's result.</param>
    /// <returns>The failure list. A valid child does not allocate.</returns>
    internal static List<PropertyValidationResult>? Prefix(
        List<PropertyValidationResult>? failures,
        string propertyName,
        ValidationResult child)
    {
        var errors = child.Errors;
        if (errors.Count == 0)
        {
            return failures;
        }

        failures ??= new List<PropertyValidationResult>(errors.Count);
        for (var i = 0; i < errors.Count; i++)
        {
            var error = errors[i];
            failures.Add(new PropertyValidationResult(propertyName + "." + error.PropertyName, error.Errors));
        }

        return failures;
    }

    /// <summary>Reports the first null parent and stops.</summary>
    /// <typeparam name="T">The object being validated.</typeparam>
    /// <param name="parents">The intermediate members.</param>
    /// <param name="instance">The object being validated.</param>
    /// <param name="failures">Earlier failures, updated when a parent is null.</param>
    /// <returns>True when a parent was null and the leaf must be skipped.</returns>
    internal static bool HasNullParent<T>(
        ParentNullCheck<T>[] parents,
        T instance,
        ref List<PropertyValidationResult>? failures)
    {
        for (var i = 0; i < parents.Length; i++)
        {
            if (parents[i].IsNull(instance))
            {
                failures = AddMessage(failures, parents[i].Name, MustNotBeNull);
                return true;
            }
        }

        return false;
    }
}
