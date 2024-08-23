using Ardalis.GuardClauses;
using Cackle.ConsoleApp.Features;
using Cackle.ConsoleApp.Internal;
using CommandLine;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace Cackle.ConsoleApp;

/// <summary>
///     Builds a new <see cref="CommandHost" />
/// </summary>
public class CommandHostBuilder
{
    /// <summary>
    ///     Commands registered with the host
    /// </summary>
    private readonly CommandCollection _commands = new();

    /// <summary>
    ///     Services registered with the host
    /// </summary>
    private readonly ServiceCollection _services = new();

    /// <summary>
    ///     Configuration provided by Microsoft.Extensions.Configuration
    /// </summary>
    private IConfiguration? _config;

    /// <summary>
    ///     Serilog configuration.
    /// </summary>
    private readonly LoggerConfiguration _loggerConfiguration = new();

    /// <summary>
    ///     The <see cref="Parser" /> that will handle parsing the command line arguments
    /// </summary>
    private Parser? _parser;

    /// <summary>
    ///     Running Environment
    /// </summary>
    public IHostEnv Environment;

    internal CommandHostBuilder()
    {
        Environment = new HostEnv();
    }

    /// <summary>
    ///     Configuration populated from configuration sources
    /// </summary>
    public IConfiguration Configuration => _config ?? new ConfigurationManager();

    /// <summary>
    ///     Initializes a <see cref="CommandHostBuilder" />
    /// </summary>
    public static CommandHostBuilder Create()
    {
        var host = new CommandHostBuilder();
        host.Configure();
        return host;
    }

    /// <summary>
    ///     Sets up a basic working environment with Serilog logging and Microsoft.Extensions.Configuration support
    /// </summary>
    private void Configure()
    {
        _config = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", true)
            .AddJsonFile($"appsettings.{Environment.Environment}.json", true)
            .Build();

        _loggerConfiguration.ReadFrom.Configuration(_config);
        //Log.Logger = _loggerConfiguration.ReadFrom.Configuration(_config).CreateLogger();
        //_services.AddLogging(l => l.AddSerilog(Log.Logger));
        _services.AddSingleton(Environment);
    }

    /// <summary>
    ///     Allows configuration of the command line parser. See
    ///     <see href="https://github.com/commandlineparser/commandline/wiki/T_CommandLine_ParserSettings">
    ///         CommandLine ParserSettings
    ///     </see>
    /// </summary>
    public void ConfigureParser(Action<ParserSettings> configuration)
    {
        _parser = new Parser(configuration);
    }

    /// <summary>
    ///     Allows configuration of Serilog without the requirement of a configuration file.
    /// </summary>
    public void ConfigureLogging(Action<LoggerConfiguration> configuration)
    {
        configuration(_loggerConfiguration);
    }

    /// <summary>
    ///     Registers a synchronous command <typeparamref name="TCommand" /> that will be invoked with
    ///     <see cref="CommandHost.Run" /> with it's command line argument specification <typeparamref name="TArgs" />
    ///     and provides configuration (options) <typeparamref name="TOptions" /> from configuration file to the command
    /// </summary>
    /// <typeparam name="TCommand">The command to be executed at the console.</typeparam>
    /// <typeparam name="TArgs">The command line arguments type to provide to command.</typeparam>
    /// <typeparam name="TOptions">The options type to be configured.</typeparam>
    public void RegisterCommand<TCommand, TArgs, TOptions>()
        where TCommand : ICommand<TArgs>
        where TArgs : ICommandArgs
        where TOptions : class
    {
        Guard.Against.Null(Configuration, nameof(Configuration));

        var config = Configuration
            .GetSection(AppConstants.Configuration.CommandSection)
            .GetSection(typeof(TOptions).Name);

        _services.Configure<TOptions>(config);
        RegisterCommand<TCommand, TArgs>();
    }

    /// <summary>
    ///     Registers a synchronous command <typeparamref name="TCommand" /> that will be invoked with
    ///     <see cref="CommandHost.Run" /> with it's command line argument specification <typeparamref name="TArgs" />
    /// </summary>
    /// <typeparam name="TCommand">The command to be executed at the console.</typeparam>
    /// <typeparam name="TArgs">The command line arguments type to provide to command.</typeparam>
    public void RegisterCommand<TCommand, TArgs>()
        where TCommand : ICommand<TArgs>
        where TArgs : ICommandArgs
    {
        _commands.Add(typeof(TCommand), typeof(TArgs), true);
        _services.AddScoped(typeof(TCommand));
    }

    /// <summary>
    ///     Registers an asynchronous command <typeparamref name="TCommandAsync" /> that will be invoked with
    ///     <see cref="CommandHost.RunAsync(string[])" /> with it's command line argument specification
    ///     <typeparamref name="TArgs" /> and provides configuration (options) <typeparamref name="TOptions" /> from
    ///     configuration file to the command
    /// </summary>
    /// <typeparam name="TCommandAsync">The command to be executed at the console.</typeparam>
    /// <typeparam name="TArgs">The command line arguments type to provide to command.</typeparam>
    /// <typeparam name="TOptions">The options type to be configured.</typeparam>
    public void RegisterAsyncCommand<TCommandAsync, TArgs, TOptions>()
        where TCommandAsync : ICommandAsync<TArgs>
        where TArgs : ICommandArgs
        where TOptions : class
    {
        Guard.Against.Null(Configuration, nameof(Configuration));

        var config = Configuration
            .GetSection(AppConstants.Configuration.CommandSection)
            .GetSection(typeof(TOptions).Name);

        _services.Configure<TOptions>(config);
        RegisterAsyncCommand<TCommandAsync, TArgs>();
    }

    /// <summary>
    ///     Registers an asynchronous command <typeparamref name="TCommandAsync" /> that will be invoked with
    ///     <see cref="CommandHost.RunAsync(string[])" /> with it's command line argument specification
    ///     <typeparamref name="TArgs" />
    /// </summary>
    /// <typeparam name="TCommandAsync">The command to be executed at the console.</typeparam>
    /// <typeparam name="TArgs">The command line arguments type to provide to command.</typeparam>
    public void RegisterAsyncCommand<TCommandAsync, TArgs>()
        where TCommandAsync : ICommandAsync<TArgs>
        where TArgs : ICommandArgs
    {
        _commands.Add(typeof(TCommandAsync), typeof(TArgs), false);
        _services.AddScoped(typeof(TCommandAsync));
    }

    /// <summary>
    ///     Register third-party services with the host
    /// </summary>
    /// <param name="services"></param>
    public void RegisterServices(Action<IServiceCollection> services)
    {
        services(_services);
    }

    /// <summary>
    ///     Register third-party services with the host
    /// </summary>
    /// <param name="services"></param>
    public void RegisterServices(Action<IServiceCollection, IConfiguration> services)
    {
        Guard.Against.Null(_config, nameof(_config), "Configuration has not been set");
        services(_services, _config);
    }

    /// <summary>
    ///     Initializes a <see cref="CommandHost" />
    /// </summary>
    public CommandHost Build()
    {
        Log.Logger = _loggerConfiguration.CreateLogger();
        _services.AddLogging(l => l.AddSerilog(Log.Logger));

        _commands.MakeReadOnly();

        _services.AddSingleton(Configuration);

        var provider = _services.BuildServiceProvider();
        var parser = _parser ?? Parser.Default;

        return new CommandHost(_commands, provider, parser);
    }
}