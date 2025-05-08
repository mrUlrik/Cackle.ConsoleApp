namespace Cackle.ConsoleApp.Internal.FluentInterfaces;

/// <summary>
///     A fluent interface for building a <see cref="ConsoleHost" />.
/// </summary>
public interface IConsoleHostBuilder
{
    /// <summary>
    ///     Builds the <see cref="ConsoleHost" /> instance with the configured services and command app.
    /// </summary>
    /// <returns>A new instance of <see cref="ConsoleHost" />.</returns>
    ConsoleHost Build();
}