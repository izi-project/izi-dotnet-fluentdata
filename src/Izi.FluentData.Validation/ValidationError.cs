using System.Collections;

namespace Izi.FluentData.Validation;

/// <summary>The messages from one failed check. Messages do not name a property.</summary>
public sealed class ValidationError : IReadOnlyList<string>
{
    /// <summary>An error with no messages. <see cref="IsValid"/> is true.</summary>
    public static ValidationError Empty { get; } = new(Array.Empty<string>());

    private readonly string? _one;
    private readonly IReadOnlyList<string>? _many;

    /// <summary>Creates an error from one <paramref name="message"/>.</summary>
    /// <param name="message">The message. It does not name a property.</param>
    internal ValidationError(string message)
    {
        ArgumentNullException.ThrowIfNull(message);
        _one = message;
    }

    /// <summary>Creates an error from <paramref name="messages"/>. An empty list is <see cref="Empty"/>.</summary>
    /// <param name="messages">The messages, in order. None of them name a property.</param>
    /// <returns>The error. The messages are copied, so later changes to <paramref name="messages"/> are not visible.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="messages"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException"><paramref name="messages"/> contains a null message.</exception>
    public static ValidationError From(IReadOnlyList<string> messages)
    {
        ArgumentNullException.ThrowIfNull(messages);
        if (messages.Count == 0)
        {
            return Empty;
        }

        var copy = new string[messages.Count];
        for (var i = 0; i < messages.Count; i++)
        {
            var message = messages[i];
            if (message is null)
            {
                throw new ArgumentException("A validation message must not be null.", nameof(messages));
            }

            copy[i] = message;
        }

        return new ValidationError(copy);
    }

    /// <summary>Creates an error from <paramref name="messages"/>.</summary>
    /// <param name="messages">The messages, in order. None of them name a property.</param>
    internal ValidationError(IReadOnlyList<string> messages)
    {
        ArgumentNullException.ThrowIfNull(messages);
        if (messages.Count == 1)
        {
            _one = messages[0];
            return;
        }

        _many = messages;
    }

    /// <summary>True when this error has no messages.</summary>
    public bool IsValid => Count == 0;

    /// <summary>The number of messages.</summary>
    public int Count => _one is not null ? 1 : _many!.Count;

    /// <summary>Gets the message at <paramref name="index"/>.</summary>
    /// <param name="index">The zero-based message index.</param>
    public string this[int index]
    {
        get
        {
            if (_one is not null)
            {
                if (index != 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(index));
                }

                return _one;
            }

            return _many![index];
        }
    }

    /// <summary>Returns the messages in order.</summary>
    /// <returns>An enumerator over the messages.</returns>
    public IEnumerator<string> GetEnumerator()
        => _one is not null ? Single(_one) : _many!.GetEnumerator();

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <summary>Joins the messages with a comma and a space.</summary>
    /// <returns>The messages, or an empty string when there are none.</returns>
    public override string ToString()
    {
        if (Count == 0)
        {
            return string.Empty;
        }

        if (Count == 1)
        {
            return this[0];
        }

        return string.Join(", ", this);
    }

    private static IEnumerator<string> Single(string message)
    {
        yield return message;
    }
}
