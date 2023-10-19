![GitHub last commit (branch)](https://img.shields.io/github/last-commit/mrUlrik/Cackle.ConsoleApp/main)
[![CodeFactor](https://www.codefactor.io/repository/github/mrulrik/cackle.consoleapp/badge)](https://www.codefactor.io/repository/github/mrulrik/cackle.consoleapp)
![GitHub tag (with filter)](https://img.shields.io/github/v/tag/mrUlrik/Cackle.ConsoleApp)

<img align="right" alt="Cackle Logo" height="128" width="128" src="https://github.com/mrUlrik/Cackle.ConsoleApp/blob/main/docs/images/package.png?raw=true" />

# Cackle.ConsoleApp
A library that provides logging, dependency injection, command line parsing, and other small features in a simple package. This library is an amalgamation of excellent libraries I use frequently, knitted together in an easy to use package. My daily work life results in creating many console applications and recreating the wheel became extremely tiring.

This library utilizes and provides the following:
* [Ardalis.GuardGlauses](https://github.com/ardalis/GuardClauses)
	* Basic predefined, guard clauses with simpler than home-brew expansion
* [CommandLineParser](https://github.com/commandlineparser/commandline)
	* Provides advanced command line specification with very clean, easy to read implementation
* [Serilog](https://serilog.net/)
	* Simply my preferring logging library
	* Configuration automatically loaded from Microsoft.Extension.Configuration
	* Taps into Microsoft.Extensions.Logging for automatic logging support for any third-party libraries that support ILogging in your project
* Microsoft's Configuration, Dependency Injection, Logging, and Options
	* A standard set of ubiquitous libraries for familiarity and ease-of-use
	* Allows for populating configuration from appsettings.json to CommandLineParser commands
	* Allows for registering CommandLineParser commands to ServiceProvider and take advantage of dependency injection

## Opinionated
Obviously there are opinions here, particularly the library choices (especially DI), but an unexpected opinion may be found when using IOptions. *The library does not ask for named options or an IConfigurationSection.* 

Instead, it defaults to using the name of the [class you are using](https://github.com/mrUlrik/Cackle.ConsoleApp/blob/5e02b73993778983650816ce8e0690fd983358a0/examples/DataFetchExample/Configuration/RandomConfig.cs) for IOptions. Additionally, when parsing [appsettings.json](https://github.com/mrUlrik/Cackle.ConsoleApp/blob/5e02b73993778983650816ce8e0690fd983358a0/examples/DataFetchExample/appsettings.json) it looks for this name as a child to the "Commands" key.

## Examples

### [DataFetchExample](https://github.com/mrUlrik/Cackle.ConsoleApp/tree/bb9ff60b354384cb0426769bfa254e8f4480191f/examples/DataFetchExample)
Demonstration of configuring a console command that uses a web-based API, configured in appsettings.json, to fetch a string from the Internet and print to console.

### [HelloWorldExample](https://github.com/mrUlrik/Cackle.ConsoleApp/tree/bb9ff60b354384cb0426769bfa254e8f4480191f/examples/SimpleExample)
Your run of the mill Hello World. Program.cs registers a class inheriting [ICommand](https://github.com/mrUlrik/Cackle.ConsoleApp/blob/5e02b73993778983650816ce8e0690fd983358a0/src/ICommand.cs) ([ICommandAsync](https://github.com/mrUlrik/Cackle.ConsoleApp/blob/5e02b73993778983650816ce8e0690fd983358a0/src/ICommandAsync.cs) also available)--quite familiar to those who use Microsoft.DependencyInjection--with the option of adding a third type to configure IOptions.

#### Program.cs
* Create the builder,
* Register your command (`ICommand<TArgs>`) with it's CommandLineParser argument specification (`TArgs`)
* Build and run the host, passing it the incoming `args`.
```cs
using Cackle.ConsoleApp;
using SimpleExample;

// Create a host to handle services, configuration, and execution
var host = CommandHostBuilder.Create();

// Register our command with it's argument specification
host.RegisterCommand<HelloWorld, HelloWorldArgs>();

// Finally, build the host and run it
return host.Build().Run(args);
```

#### HelloWorld.cs
Inherit `ICommand<TArgs>` with your CommandLineParser argument specification (`TArgs`) which will require `Invoke` (or `InvokeAsync` if using `ICommandAsync<TArgs>`) where you perform work.
```cs
using Cackle.ConsoleApp;
using CommandLine;

namespace SimpleExample;

/// <summary>
///     Print a greeting to the console
/// </summary>
internal class HelloWorld : ICommand<HelloWorldArgs>
{
	/// <summary>
	///     Entry point into the commandAsync
	/// </summary>
	/// <param name="options">Command line arguments</param>
	/// <returns>Program exit code</returns>
	public int Invoke(HelloWorldArgs options)
	{
		if (options.Verbose) Console.WriteLine("Parsed user input, printing to screen...");
		Console.WriteLine($"Hello {options.Target}!");

		return 0;
	}
}
```
Finally, let CommandLineParser come in and do the heavy lifting.
```cs
/// <summary>
///     Reply with a greeting
/// </summary>
[Verb("greeting", true)]
internal class HelloWorldArgs : ICommandArgs
{
	/// <summary>
	///     Produce verbose output
	/// </summary>
	[Option('v', "verbose", Required = false, HelpText = "Set output to verbose messages.")]
	public bool Verbose { get; set; }

	/// <summary>
	///     The target of your greeting
	/// </summary>
	[Option('t', "target", Required = true, HelpText = "Who do you want to say hello to?")]
	public string Target { get; set; } = default!;
}
```
