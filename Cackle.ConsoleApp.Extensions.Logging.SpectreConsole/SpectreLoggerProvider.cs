using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Spectre.Console;

namespace Cackle.ConsoleApp.Extensions.Logging.SpectreConsole;

/// <summary>
///     A provider for creating <see cref="SpectreLogger" /> instances.
/// </summary>
[ProviderAlias("SpectreConsole")]
public class SpectreLoggerProvider : ILoggerProvider, ISupportExternalScope
{
    private readonly IDisposable? _exceptionReloadToken;
    private readonly SpectreFormatter _formatter;
    private readonly ConcurrentDictionary<string, SpectreLogger> _loggers;
    private readonly SpectreLoggerProcessor _messageQueue;
    private readonly IOptionsMonitor<SpectreLoggerOptions> _options;
    private readonly IDisposable? _optionsReloadToken;
    private IExternalScopeProvider? _scopeProvider;

    /// <summary>
    ///     Initializes a new instance of the <see cref="SpectreLoggerProvider" /> class.
    /// </summary>
    /// <param name="options">The <see cref="IOptionsMonitor{SpectreLoggerOptions}" /> for accessing logger settings.</param>
    /// <param name="formatterOptions">
    ///     The <see cref="IOptionsMonitor{SpectreFormatterOptions}" /> for accessing formatter
    ///     settings.
    /// </param>
    /// <param name="exceptionSettings">
    ///     The <see cref="IOptionsMonitor{ExceptionSettings}" /> for accessing exception handling
    ///     settings.
    /// </param>
    /// <param name="consoleFactory">The <see cref="IAnsiConsoleFactory" /> for creating <see cref="IAnsiConsole" /> instances.</param>
    public SpectreLoggerProvider(
        IOptionsMonitor<SpectreLoggerOptions> options,
        IOptionsMonitor<SpectreFormatterOptions> formatterOptions,
        IOptionsMonitor<ExceptionSettings> exceptionSettings,
        IAnsiConsoleFactory consoleFactory)
    {
        _options = options;
        _loggers = new ConcurrentDictionary<string, SpectreLogger>();
        _formatter = new SpectreFormatter(formatterOptions);

        // Create separate AnsiConsole instances for standard output and standard error.
        var console = consoleFactory.Create();
        var errorConsole = consoleFactory.CreateError();

        // Initialize the message queue processor with the consoles, exception settings, and queue options.
        _messageQueue = new SpectreLoggerProcessor(
            console,
            errorConsole,
            exceptionSettings.CurrentValue,
            options.CurrentValue.QueueFullMode,
            options.CurrentValue.MaxQueueLength);

        // Load initial logger options and subscribe to changes.
        ReloadLoggerOptions(options.CurrentValue);
        _optionsReloadToken = _options.OnChange(ReloadLoggerOptions);

        // Load initial exception handling options and subscribe to changes.
        ReloadExceptionOptions(exceptionSettings.CurrentValue);
        _exceptionReloadToken = exceptionSettings.OnChange(ReloadExceptionOptions);
    }

    /// <inheritdoc />
    public void Dispose()
    {
        _loggers.Clear();
        _optionsReloadToken?.Dispose();
        _exceptionReloadToken?.Dispose();
        _messageQueue.Dispose();
        GC.SuppressFinalize(this);
    }

    /// <inheritdoc />
    public ILogger CreateLogger(string name)
    {
        return _loggers.TryGetValue(name, out var logger)
            ? logger
            // If a logger with the given name doesn't exist, create a new SpectreLogger and add it to the dictionary.
            : _loggers.GetOrAdd(name, new SpectreLogger(
                name, _messageQueue, _formatter,
                _scopeProvider, _options.CurrentValue));
    }

    /// <inheritdoc />
    public void SetScopeProvider(IExternalScopeProvider scopeProvider)
    {
        // Store the external scope provider and update the provider for all existing loggers.
        _scopeProvider = scopeProvider;
        foreach (var logger in _loggers)
            logger.Value.ScopeProvider = scopeProvider;
    }

    /// <summary>
    ///     Reloads the logger options and updates all existing loggers.
    /// </summary>
    /// <param name="options">The new <see cref="SpectreLoggerOptions" />.</param>
    private void ReloadLoggerOptions(SpectreLoggerOptions options)
    {
        _messageQueue.FullMode = options.QueueFullMode;
        _messageQueue.MaxQueueLength = options.MaxQueueLength;

        foreach (var (_, logger) in _loggers) logger.Options = options;
    }

    /// <summary>
    ///     Reloads the exception handling options and updates the message queue processor.
    /// </summary>
    /// <param name="exceptionSettings">The new <see cref="ExceptionSettings" />.</param>
    private void ReloadExceptionOptions(ExceptionSettings exceptionSettings)
    {
        _messageQueue.ExceptionSettings = exceptionSettings;
    }
}