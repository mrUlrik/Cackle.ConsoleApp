# Cackle.ConsoleApp

[![Nuget](https://img.shields.io/nuget/v/Cackle.ConsoleApp)](https://www.nuget.org/packages/Cackle.ConsoleApp)
[![License](https://img.shields.io/github/license/mrUlrik/Cackle.ConsoleApp)](https://github.com/mrUlrik/Cackle.ConsoleApp/blob/main/LICENSE)

`Cackle.ConsoleApp` is a lightweight library that enhances the [Spectre.Console](https://spectreconsole.net/) framework by introducing features commonly found in modern .NET console applications, such as:

* **Microsoft Dependency Injection (DI):** Seamless integration with the standard .NET DI container.
* **`IConfiguration` Support:** Easy access to application configuration.
* **`IOptions<T>` Pattern:** Strongly-typed configuration management.
* **Asynchronous Commands with Cancellation:** Provides an `CancellableAsyncCommand` base class that includes `CancellationToken` support and graceful handling of escape sequences (Ctrl+C).

This library aims to simplify the development of interactive console applications built with Spectre.Console, making them more maintainable and aligned with modern .NET practices.

## Installation

You can install the library via NuGet Package Manager or the .NET CLI:

```bash
dotnet add package Cackle.ConsoleApp
```

## Usage
`Cackle.ConsoleApp` provides a `ConsoleHostBuilder` to bootstrap your console application. This builder allows you to configure services, console settings, and commands in a fluent manner.

### Complex Setup
```csharp
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Cackle.ConsoleApp;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Spectre.Console;
using Spectre.Console.Cli;

public class Program
{
    public static Task<int> Main(string[] args)
    {
        return ConsoleHostBuilder
            // Create a new console host builder
            .Create(app =>
                app.AddCommand<GreetCommand>("greet")
                    .WithDescription("Greets the user."))

            // Configure the console host with a custom configuration
            .ConfigureServices((services, config) =>
                services.AddTransient<IGreetingService, DefaultGreetingService>())
            
            // Configure AnsiConsole settings from the "AnsiConsole" section
            .ConfigureConsole("AnsiConsole")
            
            // Build the console host
            .Build()
            
            // Run the command line application asynchronously with the provided arguments.
            .RunAsync(args);
    }
}

public interface IGreetingService
{
    string GetGreeting(string name);
}

public class DefaultGreetingService(IConfiguration config) : IGreetingService
{
    public string GetGreeting(string name)
    {
        var greetingFormat = config.GetSection("GreetingSettings")["Format"] ?? "Hello, {0}!";
        return string.Format(greetingFormat, name);
    }
}

public class GreetCommand(IGreetingService greetingService, ILogger<GreetCommand> logger)
    : CancellableAsyncCommand<GreetCommand.Settings>
{
    public override async Task<int> ExecuteAsync(CommandContext context, Settings settings,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Greeting command started for {Name}", settings.Name);
        
        await Task.Delay(100, cancellationToken);
        
        var greeting = greetingService.GetGreeting(settings.Name!);
        AnsiConsole.WriteLine(greeting);
        
        logger.LogInformation("Greeting command finished.");
        return 0;
    }

    public class Settings : CommandSettings
    {
        [CommandArgument(0, "[Name]")]
        public string? Name { get; init; }
    }
}
```

##### Explanation
1. `ConsoleHostBuilder.Create(Action<IConfigurator> configureApp)`: This static method initializes the host builder and allows you to configure the Spectre.Console CommandApp.
2. `app.AddCommand<GreetCommand>("greet")`: Registers a command named "greet" with the Spectre.Console CLI.
3. `.ConfigureServices(Action<IServiceCollection, IConfiguration> services)`: Configures the application's services using the Microsoft Dependency Injection container. You have access to both the IServiceCollection and the IConfiguration.
4. `.ConfigureConsole("AnsiConsole")`: Configures the global AnsiConsole.Console instance using settings from the specified configuration section (e.g., "AnsiConsole").
5. `.Build()`: Builds the ConsoleHost instance.
6. `.RunAsync(string[] args)`: Executes the console application with the provided arguments.
7. `CancellableAsyncCommand<TSettings>`: A base class for asynchronous commands that includes built-in CancellationToken support and handles Ctrl+C events gracefully.

#### Configuration
`Cackle.ConsoleApp` automatically loads configuration from `appsettings.json` and environment-specific `appsettings.{Environment}.json` files. You can access this configuration using `IConfiguration` in your services and commands, or through the `IOptions<T>` pattern for strongly-typed settings.

Example `appsettings.json`:
```json
{
  "AnsiConsole": {
    "ColorSystem": "Standard",
    "Profile": {
      "Width": 80,
      "Height": 25
    }
  },
  "GreetingSettings": {
    "Format": "Welcome, {0}!"
  }
}
```

### Async Commands with Cancellation
The `CancellableAsyncCommand<TSettings>` base class simplifies the creation of asynchronous console commands. It automatically handles `CancellationToken` propagation and provides a virtual `ExecuteAsync` method. It also gracefully handles Ctrl+C (CancelKeyPress) events, allowing you to perform cleanup before the application exits.

# Key Components
* `ConsoleHostBuilder`: The main entry point for configuring and building the console application host.
* `IConfigureConsoleHostBuilder`: A fluent interface for configuring the host builder.
* `ConsoleHost`: Encapsulates the Spectre.Console `CommandApp` and provides methods for running the application.
* `CancellableAsyncCommand<TSettings>`: An abstract base class for asynchronous commands with cancellation support.
* `IAnsiConsoleFactory` and `AnsiConsoleFactory`: Provides a factory for creating `IAnsiConsole` instances, integrating with `IOptions<AnsiConsoleSettings>`.
* `HostEnv`: A static class providing access to host environment information (e.g., environment name, base directory).

# Contributing
Contributions are welcome! Please feel free to submit pull requests or open issues on the GitHub repository.