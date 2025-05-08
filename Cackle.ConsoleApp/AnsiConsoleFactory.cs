using Microsoft.Extensions.Options;
using Spectre.Console;

namespace Cackle.ConsoleApp;

/// <summary>
///     A factory that creates <see cref="IAnsiConsole" /> instances based on <see cref="AnsiConsoleSettings" /> configured
///     via IOptions.
/// </summary>
/// <param name="options">The <see cref="IOptionsMonitor{AnsiConsoleSettings}" /> instance.</param>
public class AnsiConsoleFactory(IOptionsMonitor<AnsiConsoleSettings> options) : IAnsiConsoleFactory
{
    /// <inheritdoc />
    public IAnsiConsole Create()
    {
        return AnsiConsole.Create(options.CurrentValue);
    }

    /// <inheritdoc />
    public IAnsiConsole CreateError()
    {
        var console = Create();
        console.Profile.Out = new AnsiConsoleOutput(Console.Error);
        return console;
    }

    /// <inheritdoc />
    public IAnsiConsole Create(TextWriter outWriter)
    {
        var console = Create();
        console.Profile.Out = new AnsiConsoleOutput(outWriter);
        return console;
    }
}