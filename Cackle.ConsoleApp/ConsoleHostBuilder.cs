using Cackle.ConsoleApp.Internal;
using Cackle.ConsoleApp.Internal.FluentInterfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Cackle.ConsoleApp;

/// <summary>
///     Provides a builder for configuring and creating a console host.
/// </summary>
public class ConsoleHostBuilder : IConsoleHostBuilder, IConfigureConsoleHostBuilder
{
    private readonly Action<IConfigurator> _commandAppConfig;
    private readonly IConfiguration _config;
    private readonly IServiceCollection _services;

    /// <summary>
    ///     Initializes a new instance of the <see cref="ConsoleHostBuilder" /> class.
    /// </summary>
    /// <param name="commandAppConfig">An action to configure the Spectre.Console <see cref="CommandApp" />.</param>
    /// <param name="services">The <see cref="IServiceCollection" /> to register services.</param>
    /// <param name="config">The <see cref="IConfiguration" /> instance.</param>
    private ConsoleHostBuilder(Action<IConfigurator> commandAppConfig, IServiceCollection services,
        IConfiguration config)
    {
        _services = services;
        _config = config;
        _commandAppConfig = commandAppConfig;
    }

    /// <inheritdoc />
    public IConfigureConsoleHostBuilder ConfigureConsole(string sectionName)
    {
        // Get and bind the specified configuration section for AnsiConsoleSettings.
        var consoleConfig = _config.GetRequiredSection(sectionName);
        var consoleSettings = consoleConfig.Get<AnsiConsoleSettings>();
        ArgumentNullException.ThrowIfNull(consoleSettings, nameof(sectionName));

        // Create and set the global AnsiConsole instance using the configured settings.
        AnsiConsole.Console = AnsiConsole.Create(consoleSettings);

        // Configure AnsiConsoleSettings using the specified configuration section for DI.
        _services.Configure<AnsiConsoleSettings>(consoleConfig);
        return this;
    }

    /// <inheritdoc />
    public IConfigureConsoleHostBuilder ConfigureConsole(Action<AnsiConsoleSettings> configure)
    {
        // Create a new instance of AnsiConsoleSettings and apply the provided configuration action.
        var consoleSettings = new AnsiConsoleSettings();
        configure(consoleSettings);

        // Create and set the global AnsiConsole instance using the configured settings.
        AnsiConsole.Console = AnsiConsole.Create(consoleSettings);

        // Configure AnsiConsoleSettings using the specified configuration section for DI.
        _services.Configure(configure);
        return this;
    }

    /// <inheritdoc />
    public IConfigureConsoleHostBuilder ConfigureExceptions(string sectionName)
    {
        var exceptionSettings = _config.GetRequiredSection(sectionName);
        _services.Configure<ExceptionSettings>(exceptionSettings);
        return this;
    }

    /// <inheritdoc />
    public IConfigureConsoleHostBuilder ConfigureExceptions(Action<ExceptionSettings> configure)
    {
        _services.Configure(configure);
        return this;
    }

    /// <inheritdoc />
    public IConfigureConsoleHostBuilder ConfigureServices(Action<IServiceCollection> services)
    {
        return ConfigureServices((collection, _) => services(collection));
    }

    /// <inheritdoc />
    public IConfigureConsoleHostBuilder ConfigureServices(Action<IServiceCollection, IConfiguration> services)
    {
        services(_services, _config);
        return this;
    }

    /// <inheritdoc />
    public ConsoleHost Build()
    {
        _services.Add(ServiceDescriptor.Singleton<IAnsiConsole>(sp =>
            sp.GetRequiredService<IAnsiConsoleFactory>().Create()));

        var registrar = new TypeRegistrar(_services);
        var app = new CommandApp(registrar);

        using (var services = _services.BuildServiceProvider())
        {
            var factory = services.GetRequiredService<IAnsiConsoleFactory>();
            app.Configure(c => c.ConfigureConsole(factory.Create()));
        }

        app.Configure(_commandAppConfig);

        return new ConsoleHost(app);
    }

    /// <summary>
    ///     Creates a new instance of the <see cref="ConsoleHostBuilder" /> with default configuration including loading
    ///     appsettings.json and environment-specific appsettings.
    /// </summary>
    /// <param name="configureApp">An action to configure the Spectre.Console <see cref="CommandApp" />.</param>
    /// <returns>A new instance of <see cref="ConsoleHostBuilder" />.</returns>
    public static ConsoleHostBuilder Create(Action<IConfigurator> configureApp)
    {
        // Load the configuration from appsettings.json and environment-specific appsettings.
        var configBuilder = new ConfigurationBuilder()
            .SetBasePath(HostEnv.BaseDirectory)
            .AddJsonFile("appsettings.json", true, true)
            .AddJsonFile($"appsettings.{HostEnv.Environment}.json", true, true);

        // Build the configuration.
        IConfiguration config = configBuilder.Build();

        var services = new ServiceCollection();
        services.AddSingleton(config);

        // Configure the AnsiConsole and ExceptionSettings using the loaded configuration.
        services.Configure<AnsiConsoleSettings>(config.GetSection("AnsiConsole"));
        services.Configure<ExceptionSettings>(config.GetSection("ExceptionSettings"));
        services.AddSingleton<IAnsiConsoleFactory, AnsiConsoleFactory>();

        // Create and return a new ConsoleHostBuilder instance.
        return new ConsoleHostBuilder(configureApp, services, config);
    }
}