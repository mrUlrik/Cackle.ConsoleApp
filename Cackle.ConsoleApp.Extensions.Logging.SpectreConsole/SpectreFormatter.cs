// This code is derived from the Microsoft.Extensions.Logging library,
// which is licensed under the MIT License. See LICENSE file for details.

using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using Cackle.ConsoleApp.Extensions.Logging.SpectreConsole.Internal;
using Cackle.ConsoleApp.Internal;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

namespace Cackle.ConsoleApp.Extensions.Logging.SpectreConsole;

/// <summary>
///     Allows custom log messages formatting for Spectre.Console output.
/// </summary>
public class SpectreFormatter : IDisposable
{
    private readonly IDisposable? _optionsReloadToken;

    /// <summary>
    ///     Initializes a new instance of the <see cref="SpectreFormatter" /> class.
    /// </summary>
    /// <param name="options">The <see cref="IOptionsMonitor{SpectreFormatterOptions}" /> for accessing formatter settings.</param>
    public SpectreFormatter(IOptionsMonitor<SpectreFormatterOptions> options)
    {
        // Load initial formatter options and subscribe to changes.
        ReloadLoggerOptions(options.CurrentValue);
        _optionsReloadToken = options.OnChange(ReloadLoggerOptions);
    }

    /// <summary>
    ///     Gets or sets the current <see cref="SpectreFormatterOptions" />.
    /// </summary>
    internal SpectreFormatterOptions FormatterOptions { get; set; }

    /// <summary>
    ///     Gets or sets the parsed <see cref="MessageFormat" /> used for formatting log messages.
    /// </summary>
    internal MessageFormat LogFormat { get; set; }

    /// <inheritdoc />
    public void Dispose()
    {
        _optionsReloadToken?.Dispose();
    }

    /// <summary>
    ///     Reloads the formatter options and updates the parsed log format.
    /// </summary>
    /// <param name="options">The new <see cref="SpectreFormatterOptions" />.</param>
    [MemberNotNull(nameof(FormatterOptions))]
    [MemberNotNull(nameof(LogFormat))]
    private void ReloadLoggerOptions(SpectreFormatterOptions options)
    {
        FormatterOptions = options;

        // Parse the message format string into a structured MessageFormat object.
        LogFormat = MessageFormatParser.Parse(options.MessageFormat);
    }

    /// <summary>
    ///     Writes the log entry to the specified <see cref="TextWriter" /> using the configured format.
    /// </summary>
    /// <typeparam name="TState">The type of the state object.</typeparam>
    /// <param name="logEntry">The <see cref="LogEntry{TState}" /> to write.</param>
    /// <param name="scopeProvider">The <see cref="IExternalScopeProvider" /> for accessing external scopes.</param>
    /// <param name="textWriter">The <see cref="TextWriter" /> to write the formatted log message to.</param>
    public void Write<TState>(in LogEntry<TState> logEntry, IExternalScopeProvider? scopeProvider,
        TextWriter textWriter)
    {
#if NET9_0
        // If the log entry state is a BufferedLogRecord (used with IBufferedLogger).
        if (logEntry.State is BufferedLogRecord bufferedRecord)
        {
            // Use the pre-formatted message from the buffered record.
            var message = bufferedRecord.FormattedMessage ?? string.Empty;
            WriteInternal(null, textWriter, message, bufferedRecord.LogLevel, bufferedRecord.EventId.Id,
                logEntry.Category, bufferedRecord.Timestamp);
        }
        else
        {
#endif
            // Format the log message using the provided formatter delegate.
            var message = logEntry.Formatter(logEntry.State, logEntry.Exception);

            // ReSharper disable ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
            if (logEntry.Exception is null && message is null) return;
            // ReSharper restore ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract

            WriteInternal(scopeProvider, textWriter, message, logEntry.LogLevel, logEntry.EventId.Id, logEntry.Category,
                GetCurrentDateTime());
#if NET9_0
        }
#endif
    }

    /// <summary>
    ///     Writes the formatted log message to the <see cref="TextWriter" />.
    /// </summary>
    /// <param name="scopeProvider">The <see cref="IExternalScopeProvider" /> for accessing external scopes.</param>
    /// <param name="textWriter">The <see cref="TextWriter" /> to write to.</param>
    /// <param name="message">The formatted log message.</param>
    /// <param name="logLevel">The <see cref="LogLevel" /> of the message.</param>
    /// <param name="eventId">The <see cref="EventId" /> of the message.</param>
    /// <param name="category">The log category.</param>
    /// <param name="stamp">The timestamp of the log event.</param>
    private void WriteInternal(IExternalScopeProvider? scopeProvider, TextWriter textWriter, string message,
        LogLevel logLevel, int eventId, string category, DateTimeOffset stamp)
    {
        // Use stack-allocated spans for formatting event ID and timestamp to avoid heap allocations.
        Span<char> eventIdSpan = stackalloc char[10];
        Span<char> timestampSpan = stackalloc char[64];

        // Iterate through each segment of the parsed log format.
        foreach (var segment in LogFormat)
            switch (segment.Type)
            {
                case MessageSegmentType.Text:
                    // Write literal text segments directly.
                    textWriter.Write(segment.Value);
                    break;

                case MessageSegmentType.Timestamp:
                {
                    // Try to format the timestamp using the specified format and culture and fallback to ToString if fails.
                    if (stamp.TryFormat(timestampSpan, out var charsWritten,
                            FormatterOptions.TimestampFormat, CultureInfo.CurrentUICulture))
                        textWriter.Write(timestampSpan[..charsWritten]);
                    else
                        textWriter.Write(stamp.ToString(FormatterOptions.TimestampFormat,
                            CultureInfo.CurrentUICulture));
                    break;
                }

                case MessageSegmentType.LogLevel:
                    // Write the log level with Spectre.Console markup.
                    WriteLogLevelMarkup(textWriter, logLevel);
                    break;

                case MessageSegmentType.Category:
                    // Write the log category.
                    textWriter.Write(category);
                    break;

                case MessageSegmentType.EventId:
                {
                    // Try to format the event id using the specified format and culture and fallback to ToString if fails.
                    if (eventId.TryFormat(eventIdSpan, out var charsWritten,
                            FormatterOptions.EventIdFormat, CultureInfo.CurrentUICulture))
                        textWriter.Write(eventIdSpan[..charsWritten]);
                    else
                        textWriter.Write(eventId.ToString(FormatterOptions.EventIdFormat,
                            CultureInfo.CurrentUICulture));
                    break;
                }

                case MessageSegmentType.Message:
                    // Write the log message.
                    textWriter.Write(message);
                    break;

                case MessageSegmentType.NewLine:
                    // Write a newline character.
                    textWriter.Write(Environment.NewLine);
                    break;

                default:
                    // Throw an exception for unknown message segment types.
                    throw new ArgumentOutOfRangeException(ResMan.Format("InvalidPlaceHolder",
                        segment.Type.ToString("G")));
            }

        // Write scope information if IncludeScopes is enabled.
        WriteScopeInformation(textWriter, scopeProvider);
    }

    /// <summary>
    ///     Writes the log level with Spectre.Console markup.
    /// </summary>
    /// <param name="textWriter">The <see cref="TextWriter"/> to write to.</param>
    /// <param name="logLevel">The <see cref="LogLevel"/> to format.</param>
    private void WriteLogLevelMarkup(TextWriter textWriter, LogLevel logLevel)
    {
        textWriter.Write(GetLogLevelMarkup(logLevel));
        textWriter.Write(logLevel switch
        {
            LogLevel.Trace => "trce",
            LogLevel.Debug => "dbug",
            LogLevel.Information => "info",
            LogLevel.Warning => "warn",
            LogLevel.Error => "fail",
            LogLevel.Critical => "crit",
            _ => "none"
        });
        textWriter.Write("[/]");
    }

    /// <summary>
    ///     Writes the scope information to the <see cref="TextWriter"/>.
    /// </summary>
    /// <param name="textWriter">The <see cref="TextWriter"/> to write to.</param>
    /// <param name="scopeProvider">The <see cref="IExternalScopeProvider"/> to iterate through scopes.</param>
    private void WriteScopeInformation(TextWriter textWriter, IExternalScopeProvider? scopeProvider)
    {
        if (!FormatterOptions.IncludeScopes || scopeProvider == null) return;

        // Iterate through each active scope and write information on new lines with markup.
        scopeProvider.ForEachScope((scope, state) =>
        {
            state.Write(Environment.NewLine);
            state.Write(FormatterOptions.ScopeMarkup);
            state.Write("=> ");
            state.Write(scope);
            state.Write("[/]");
        }, textWriter);
    }

    /// <summary>
    ///     Gets the current date and time, respecting the UseUtcTimestamp option.
    /// </summary>
    /// <returns>The current <see cref="DateTimeOffset"/>.</returns>
    private DateTimeOffset GetCurrentDateTime()
    {
        return FormatterOptions.UseUtcTimestamp ? DateTimeOffset.UtcNow : DateTimeOffset.Now;
    }

    /// <summary>
    ///     Gets the Spectre.Console markup string for the given log level.
    /// </summary>
    /// <param name="logLevel">The <see cref="LogLevel"/>.</param>
    /// <returns>The Spectre.Console markup string.</returns>
    private string GetLogLevelMarkup(LogLevel logLevel)
    {
        return logLevel switch
        {
            LogLevel.Trace => FormatterOptions.TraceMarkup,
            LogLevel.Debug => FormatterOptions.DebugMarkup,
            LogLevel.Information => FormatterOptions.InformationMarkup,
            LogLevel.Warning => FormatterOptions.WarningMarkup,
            LogLevel.Error => FormatterOptions.ErrorMarkup,
            LogLevel.Critical => FormatterOptions.CriticalMarkup,
            LogLevel.None => string.Empty,
            _ => string.Empty
        };
    }
}