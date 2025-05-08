using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Spectre.Console;

namespace Cackle.ConsoleApp.Internal.FluentInterfaces;

/// <summary>
///     A fluent interface for configuring the <see cref="ConsoleHostBuilder" />. This interface allows for configuring
///     console settings, exception handling, and services.
/// </summary>
public interface IConfigureConsoleHostBuilder
{
    /// <summary>
    ///     Configures the <see cref="AnsiConsole" /> using settings from the specified configuration section.
    /// </summary>
    /// <param name="sectionName">The name of the configuration section containing <see cref="AnsiConsoleSettings" />.</param>
    /// <returns>The current <see cref="IConfigureConsoleHostBuilder" /> for further configuration.</returns>
    IConfigureConsoleHostBuilder ConfigureConsole(string sectionName);

    /// <summary>
    ///     Configures the <see cref="AnsiConsole" /> using the provided configuration action.
    /// </summary>
    /// <param name="configure">An action that configures the <see cref="AnsiConsoleSettings" />.</param>
    /// <returns>The current <see cref="IConfigureConsoleHostBuilder" /> for further configuration.</returns>
    IConfigureConsoleHostBuilder ConfigureConsole(Action<AnsiConsoleSettings> configure);

    /// <summary>
    ///     Configures the <see cref="ExceptionSettings" /> using settings from the specified configuration section.
    /// </summary>
    /// <param name="sectionName">The name of the configuration section containing <see cref="ExceptionSettings" />.</param>
    /// <returns>The current <see cref="IConfigureConsoleHostBuilder" /> for further configuration.</returns>
    IConfigureConsoleHostBuilder ConfigureExceptions(string sectionName);

    /// <summary>
    ///     Configures the <see cref="ExceptionSettings" /> using the provided configuration action.
    /// </summary>
    /// <param name="configure">An action that configures the <see cref="ExceptionSettings" />.</param>
    /// <returns>The current <see cref="IConfigureConsoleHostBuilder" /> for further configuration.</returns>
    IConfigureConsoleHostBuilder ConfigureExceptions(Action<ExceptionSettings> configure);

    /// <summary>
    ///     Configures the application's services using the provided action.
    /// </summary>
    /// <param name="configure">An action that registers services in the <see cref="IServiceCollection" />.</param>
    /// <returns>The current <see cref="IConfigureConsoleHostBuilder" /> for further configuration.</returns>
    IConfigureConsoleHostBuilder ConfigureServices(Action<IServiceCollection> configure);

    /// <summary>
    ///     Configures the application's services using the provided action, providing access to both the
    ///     <see cref="IServiceCollection" /> and the <see cref="IConfiguration" />.
    /// </summary>
    /// <param name="configure">
    ///     An action that registers services in the <see cref="IServiceCollection" /> and uses the
    ///     <see cref="IConfiguration" />.
    /// </param>
    /// <returns>The current <see cref="IConfigureConsoleHostBuilder" /> for further configuration.</returns>
    IConfigureConsoleHostBuilder ConfigureServices(Action<IServiceCollection, IConfiguration> configure);

    /// <summary>
    ///     Builds the <see cref="ConsoleHost" /> instance with the configured services and command app.
    /// </summary>
    /// <returns>A new instance of <see cref="ConsoleHost" />.</returns>
    ConsoleHost Build();
}