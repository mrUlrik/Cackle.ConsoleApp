using System.Diagnostics;

namespace Cackle.ConsoleApp.Extensions.Logging.SpectreConsole.Internal;

/// <summary>
///     Represents a segment of a parsed message format.
/// </summary>
/// <remarks>
///     A segment can either represent a specific type (e.g., Timestamp, LogLevel) or plain text.
/// </remarks>
/// <param name="messageSegmentType">
///     The type of the segment, indicating its purpose or content.
/// </param>
/// <param name="value">
///     The optional value of the segment, typically used for text segments.
/// </param>
[DebuggerDisplay("{Type}: {Value}")]
public readonly struct MessageFormatSegment(MessageSegmentType messageSegmentType, string? value = null)
{
    /// <summary>
    ///     Gets the type of the segment, indicating its purpose or content.
    /// </summary>
    public MessageSegmentType Type { get; } = messageSegmentType;

    /// <summary>
    ///     Gets the optional value of the segment, typically used for text segments.
    /// </summary>
    public string? Value { get; } = value;
}