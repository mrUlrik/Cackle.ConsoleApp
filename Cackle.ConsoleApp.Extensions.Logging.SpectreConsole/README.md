# Cackle.ConsoleApp.Extensions.Logging.SpectreConsole

[![Nuget](https://img.shields.io/nuget/v/Cackle.ConsoleApp.Extensions.Logging.SpectreConsole)](https://www.nuget.org/packages/Cackle.ConsoleApp.Extensions.Logging.SpectreConsole)
[![License](https://img.shields.io/github/license/cackledigital/Cackle.ConsoleApp)](https://github.com/cackledigital/Cackle.ConsoleApp/blob/main/LICENSE)

This library provides a logging provider for [Microsoft.Extensions.Logging](https://docs.microsoft.com/en-us/dotnet/core/extensions/logging) that utilizes the rich console output capabilities of [Spectre.Console](https://spectreconsole.net/). It's designed to seamlessly integrate with the `Cackle.ConsoleApp` framework, offering styled and informative console logging within your Spectre.Console-powered applications.

**Key Features:**

* **Familiar `Microsoft.Extensions.Logging` Integration:** Leverages the standard logging abstractions you're already familiar with.
* **Spectre.Console Styling:** Outputs log messages with colors and markup provided by Spectre.Console for enhanced readability.
* **Configurable Output Format:** Allows customization of the log message format, including timestamp, log level, category, event ID, and message.
* **Log Level Filtering:** Respects the log level configurations from `appsettings.json` or code.
* **External Scope Support:** Includes information from logging scopes in the output.
* **Queueing for Performance:** Asynchronously processes log messages to prevent blocking your application's main thread.
* **Queue Full Handling:** Configurable behavior when the log message queue is full (wait or drop).
* **Exception Formatting:** Utilizes Spectre.Console's exception formatting for clear error output.
* **Standard Error Threshold:** Configurable log level threshold to direct messages to standard error.

## Installation

You can install the library via NuGet Package Manager or the .NET CLI:

```bash
dotnet add package Cackle.ConsoleApp.Extensions.Logging.SpectreConsole
```

## Usage

To use the Spectre.Console logging provider within your `Cackle.ConsoleApp` application, you need to add it to your logging configuration during host building.

### Basic Setup

If you're using the Cackle.ConsoleApp.ConsoleHostBuilder, you can add the logging provider like this:
```csharp
using Cackle.ConsoleApp;
using Cackle.ConsoleApp.Extensions.Logging.SpectreConsole;
using Microsoft.Extensions.Logging;
using Spectre.Console.Cli;

public class Program
{
    public static int Main(string[] args)
    {
        return ConsoleHostBuilder.Create(app =>
        {
            app.AddCommand<MyCommand>("mycommand");
        })
        .AddSpectreConsoleLogging() // Add the Spectre.Console logging provider
        .Build()
        .Run(args);
    }
}

public class MyCommand : Command<CommandSettings>
{
    private readonly ILogger<MyCommand> _logger;

    public MyCommand(ILogger<MyCommand> logger)
    {
        _logger = logger;
    }

    public override int Execute([NotNull] CommandContext context, [NotNull] CommandSettings settings)
    {
        _logger.LogTrace("This is a trace log.");
        _logger.LogDebug("This is a debug log.");
        _logger.LogInformation("This is an information log.");
        _logger.LogWarning("This is a warning log!");
        _logger.LogError(new InvalidOperationException("Something went wrong!"), "This is an error log.");
        _logger.LogCritical("This is a critical log!");

        using (_logger.BeginScope("User: 123"))
        {
            _logger.LogInformation("This log message is within a scope.");
        }

        return 0;
    }
}
```

### Advanced Configuration
You can configure the Spectre.Console logging provider in your `appsettings.json` under the `Logging` section, specifically within the `SpectreConsole` provider options:
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Cackle.ConsoleApp.Program": "Debug"
    },
    "SpectreConsole": {
      "LogLevel": {
        "Default": "Information",
        "Cackle.ConsoleApp.MyCommand": "Trace"
      },
      "FormatterOptions": {
        "MessageFormat": "[{Timestamp:HH:mm:ss}] [{Level}] {Category}: {Message}{NewLine}{Exception}",
        "TimestampFormat": "yyyy-MM-dd HH:mm:ss.fff",
        "IncludeScopes": true,
        "ScopeMarkup": "[grey]",
        "TraceMarkup": "[grey]",
        "DebugMarkup": "[blue]",
        "InformationMarkup": "[green]",
        "WarningMarkup": "[yellow]",
        "ErrorMarkup": "[red]",
        "CriticalMarkup": "[bold red]",
        "UseUtcTimestamp": false,
        "EventIdFormat": "x"
      },
      "QueueFullMode": "Wait",
      "MaxQueueLength": 1024,
      "LogToStandardErrorThreshold": "Warning"
    }
  }
}
```

#### Configuration Options
* `LogLevel`: Controls the minimum log level to be outputted by the Spectre.Console logger.
* `FormatterOptions`:
  * `MessageFormat`: Defines the format string for log messages. Supported placeholders `{Timestamp}`, `{Level}`, `{Category}`, `{EventId}`, `{Message}`, and `{NewLine}`.
  * `TimestampFormat`: The format string for the timestamp.
  * `IncludeScopes`: A boolean indicating whether to include scope information in the output.
  * `ScopeMarkup`: The Spectre.Console markup to apply to scope information.
  * `TraceMarkup`, `DebugMarkup`, `InformationMarkup`, `WarningMarkup`, `ErrorMarkup`, `CriticalMarkup`: Spectre.Console markup strings to style the log level.
  * `UseUtcTimestamp`: A boolean indicating whether to use UTC timestamps.
  * `EventIdFormat`: The format string for the event ID.
* `QueueFullMode`: Specifies the behavior when the log message queue is full.
  * `Wait`: Blocks the logging thread until space becomes available.
  * `DropWrite`: Drops the current log message and optionally writes a warning to the console about dropped messages.
* `MaxQueueLength`: The maximum number of messages allowed in the logging queue.
* `LogToStandardErrorThreshold`: Specifies the minimum log level that will also be written to standard error.

# Contributing
Contributions are welcome! Please feel free to submit pull requests or open issues on the GitHub repository.