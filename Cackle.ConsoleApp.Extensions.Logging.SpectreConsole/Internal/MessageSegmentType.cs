namespace Cackle.ConsoleApp.Extensions.Logging.SpectreConsole.Internal;

/// <summary>
///     Defines the types of segments that can be parsed from a message format string.
/// </summary>
public enum MessageSegmentType
{
    /// <summary>
    ///     Represents plain text in the message format.
    /// </summary>
    Text,

    /// <summary>
    ///     Represents a timestamp placeholder in the message format.
    /// </summary>
    Timestamp,

    /// <summary>
    ///     Represents a log level placeholder in the message format.
    /// </summary>
    LogLevel,

    /// <summary>
    ///     Represents a category placeholder in the message format.
    /// </summary>
    Category,

    /// <summary>
    ///     Represents an event ID placeholder in the message format.
    /// </summary>
    EventId,

    /// <summary>
    ///     Represents a message placeholder in the message format.
    /// </summary>
    Message,

    /// <summary>
    ///     Represents a newline placeholder in the message format.
    /// </summary>
    NewLine
}