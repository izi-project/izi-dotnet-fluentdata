using System.Collections;

namespace Izi.FluentData.Validation;

/// <summary>The messages from one failed check. Messages do not name a property.</summary>
public sealed class ValidationError : IReadOnlyList<string>
{
    /// <summary>An error with no messages. <see cref="IsValid"/> is true.</summary>
    public static ValidationError Empty { get; } = new(Array.Empty<string>());

    private readonly IReadOnlyList<string> _messages;

    /// <summary>Creates an error from <paramref name="messages"/>.</summary>
    /// <param name="messages">The messages, in order. None of them name a property.</param>
    internal ValidationError(IReadOnlyList<string> messages)
    {
        ArgumentNullException.ThrowIfNull(messages);
        _messages = messages;
    }

    /// <summary>True when this error has no messages.</summary>
    public bool IsValid => Count == 0;

    /// <summary>The number of messages.</summary>
    public int Count => _messages.Count;

    /// <summary>Gets the message at <paramref name="index"/>.</summary>
    /// <param name="index">The zero-based message index.</param>
    public string this[int index] => _messages[index];

    /// <summary>Returns the messages in order.</summary>
    /// <returns>An enumerator over the messages.</returns>
    public IEnumerator<string> GetEnumerator() => _messages.GetEnumerator();

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
