using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Cackle.ConsoleApp.Extensions.Logging.SpectreConsole;

/// <summary>
///     An internal logger implementation that uses Spectre.Console's <see cref="Spectre.Console.AnsiConsole" /> to write
///     log messages to the console. This logger closely replicates the behavior of the standard <see cref="ILogger" />
///     from <see cref="Microsoft.Extensions.Logging">Microsoft.Extensions.Logging</see>.
/// </summary>
internal sealed class SpectreLogger : ILogger
#if NET9_0
    , IBufferedLogger
#endif
{
    /// <summary>
    ///     A thread-static StringWriter to minimize object allocation for formatting log messages.
    /// </summary>
    // ReSharper disable once InconsistentNaming
    [ThreadStatic] private static StringWriter? t_stringWriter;

    private readonly string _name;
    private readonly SpectreLoggerProcessor _queueProcessor;

    /// <summary>
    ///     Initializes a new instance of the <see cref="SpectreLogger" /> class.
    /// </summary>
    /// <param name="name">The name of the logger.</param>
    /// <param name="loggerProcessor">The <see cref="SpectreLoggerProcessor" /> instance to enqueue log messages.</param>
    /// <param name="formatter">The <see cref="SpectreFormatter" /> instance to format log messages.</param>
    /// <param name="scopeProvider">The <see cref="IExternalScopeProvider" /> instance for accessing external scopes.</param>
    /// <param name="options">The <see cref="SpectreLoggerOptions" /> instance containing logger settings.</param>
    internal SpectreLogger(
        string name,
        SpectreLoggerProcessor loggerProcessor,
        SpectreFormatter formatter,
        IExternalScopeProvider? scopeProvider,
        SpectreLoggerOptions options)
    {
        _name = name;
        _queueProcessor = loggerProcessor;
        Formatter = formatter;
        ScopeProvider = scopeProvider;
        Options = options;
    }

    /// <summary>
    ///     Gets the <see cref="SpectreFormatter" /> used to format log messages.
    /// </summary>
    private SpectreFormatter Formatter { get; }

    /// <summary>
    ///     Gets or sets the <see cref="IExternalScopeProvider" /> that provides external scope information.
    /// </summary>
    internal IExternalScopeProvider? ScopeProvider { get; set; }

    /// <summary>
    ///     Gets or sets the <see cref="SpectreLoggerOptions" /> that configure the logger's behavior.
    /// </summary>
    internal SpectreLoggerOptions Options { get; set; }

#if NET9_0
    /// <inheritdoc />
    public void LogRecords(IEnumerable<BufferedLogRecord> records)
    {
        ArgumentNullException.ThrowIfNull(records);

        // Get the thread-local StringWriter, creating a new one if it doesn't exist for the current thread.
        var writer = t_stringWriter ??= new StringWriter();

        var sb = writer.GetStringBuilder();
        foreach (var rec in records)
        {
            // Create a LogEntry for the buffered record. The formatter here simply returns the FormattedMessage if available.
            var logEntry = new LogEntry<BufferedLogRecord>(rec.LogLevel, _name, rec.EventId, rec, null,
                static (s, _) => s.FormattedMessage ?? string.Empty);

            // Format the log entry into the StringWriter (scope is null for buffered records).
            Formatter.Write(in logEntry, null, writer);

            if (sb.Length == 0) continue;

            // Get the formatted log message as an ANSI string and clear the StringBuilder for the next log entry.
            var computedAnsiString = sb.ToString();
            sb.Clear();

            // Enqueue the formatted log message to the processor for asynchronous writing to the console.
            _queueProcessor.EnqueueMessage(new LogMessageEntry(computedAnsiString,
                // Determine if the log message should be written to standard error based on the log level threshold.
                rec.LogLevel >= Options.LogToStandardErrorThreshold));
        }

        // If the StringBuilder's capacity has grown significantly, reset it to a reasonable default to save memory.
        if (sb.Capacity > 1024) sb.Capacity = 1024;
    }
#endif

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception,
        Func<TState, Exception?, string> formatter)
    {
        // Check if the current log level is enabled based on the logger options.
        if (!IsEnabled(logLevel)) return;

        ArgumentNullException.ThrowIfNull(formatter);

        // Get the thread-local StringWriter, creating a new one if it doesn't exist for the current thread.
        t_stringWriter ??= new StringWriter();

        // Create a LogEntry instance containing all the information then format the log into the StringWriter.
        var logEntry = new LogEntry<TState>(logLevel, _name, eventId, state, exception, formatter);
        Formatter.Write(in logEntry, ScopeProvider, t_stringWriter);

        var sb = t_stringWriter.GetStringBuilder();
        if (sb.Length == 0) return;

        // Get the formatted log message as an ANSI string and clear the StringBuilder for the next log entry.
        var computedAnsiString = sb.ToString();
        sb.Clear();

        // If the StringBuilder's capacity has grown significantly, reset it to a reasonable default to save memory.
        if (sb.Capacity > 1024) sb.Capacity = 1024;

        // Enqueue the formatted log message to the processor for asynchronous writing to the console.
        _queueProcessor.EnqueueMessage(new LogMessageEntry(computedAnsiString, exception,
            // Determine if the log message should be written to standard error based on the log level threshold.
            logLevel >= Options.LogToStandardErrorThreshold));
    }

    /// <inheritdoc />
    public bool IsEnabled(LogLevel logLevel)
    {
        // Check if the provided logLevel is not None and is greater than or equal to the configured LogLevel.
        return logLevel != LogLevel.None && logLevel >= Options.LogLevel;
    }

    /// <inheritdoc />
    public IDisposable BeginScope<TState>(TState state) where TState : notnull
    {
        // If an external scope provider is available, push the new scope onto it.
        // Otherwise, return a NullScope instance which does nothing.
        return ScopeProvider?.Push(state) ?? NullScope.Instance;
    }
}