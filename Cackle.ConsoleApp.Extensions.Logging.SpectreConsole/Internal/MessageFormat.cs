using System.Collections;

namespace Cackle.ConsoleApp.Extensions.Logging.SpectreConsole.Internal;

/// <summary>
///     Represents a format for messages, consisting of a collection of segments.
/// </summary>
public class MessageFormat : IEnumerable<MessageFormatSegment>
{
    // A list to store the segments of the message format.
    private readonly List<MessageFormatSegment> _segments = [];

    /// <inheritdoc />
    public IEnumerator<MessageFormatSegment> GetEnumerator()
    {
        return _segments.GetEnumerator();
    }

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    /// <summary>
    ///     Adds a new segment to the message format with the specified type.
    /// </summary>
    /// <param name="type">The type of the message segment to add.</param>
    public void Add(MessageSegmentType type)
    {
        _segments.Add(new MessageFormatSegment(type));
    }

    /// <summary>
    ///     Adds a new segment to the message format with the specified type and value.
    /// </summary>
    /// <param name="type">The type of the message segment to add.</param>
    /// <param name="value">The value of the message segment to add.</param>
    public void Add(MessageSegmentType type, string value)
    {
        _segments.Add(new MessageFormatSegment(type, value));
    }
}