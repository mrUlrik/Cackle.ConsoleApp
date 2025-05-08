// This code is derived from the Microsoft.Extensions.Logging library,
// which is licensed under the MIT License. See LICENSE file for details.

namespace Cackle.ConsoleApp.Extensions.Logging.SpectreConsole;

/// <summary>
///     Represents a log message entry with an optional exception and error flag.
/// </summary>
internal readonly struct LogMessageEntry
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="LogMessageEntry" /> struct with a message and an optional error flag.
    /// </summary>
    /// <param name="message">The log message.</param>
    /// <param name="logAsError">Indicates whether the message should be logged as an error. Defaults to <c>false</c>.</param>
    public LogMessageEntry(string message, bool logAsError = false)
    {
        Message = message;
        LogAsError = logAsError;
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="LogMessageEntry" /> struct with a message, an exception, and an
    ///     optional error flag.
    /// </summary>
    /// <param name="message">The log message.</param>
    /// <param name="exception">The exception associated with the log message, if any.</param>
    /// <param name="logAsError">Indicates whether the message should be logged as an error. Defaults to <c>false</c>.</param>
    public LogMessageEntry(string message, Exception? exception, bool logAsError = false) : this(message, logAsError)
    {
        Exception = exception;
    }

    /// <summary>
    ///     Gets the log message.
    /// </summary>
    public readonly string Message;

    /// <summary>
    ///     Gets a value indicating whether the message should be logged as an error.
    /// </summary>
    public readonly bool LogAsError;

    /// <summary>
    ///     Gets the exception associated with the log message, if any.
    /// </summary>
    public readonly Exception? Exception;
}