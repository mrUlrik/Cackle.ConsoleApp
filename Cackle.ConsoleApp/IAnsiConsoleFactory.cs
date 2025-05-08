using Spectre.Console;

namespace Cackle.ConsoleApp;

/// <summary>
///     An interface for a factory that creates <see cref="IAnsiConsole" /> instances.
/// </summary>
public interface IAnsiConsoleFactory
{
    /// <summary>
    ///     Creates a new <see cref="IAnsiConsole" /> instance.
    /// </summary>
    /// <returns>A new <see cref="IAnsiConsole" /> instance.</returns>
    IAnsiConsole Create();

    /// <summary>
    ///     Creates a new <see cref="IAnsiConsole" /> instance configured for error output.
    /// </summary>
    /// <returns>A new <see cref="IAnsiConsole" /> instance configured for error output.</returns>
    IAnsiConsole CreateError();

    /// <summary>
    ///     Creates a new <see cref="IAnsiConsole" /> instance with the specified <see cref="TextWriter" />.
    /// </summary>
    /// <param name="outWriter">The <see cref="TextWriter" /> to use for output.</param>
    /// <returns>A new <see cref="IAnsiConsole" /> instance with the specified output.</returns>
    IAnsiConsole Create(TextWriter outWriter);
}