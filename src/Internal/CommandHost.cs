﻿using Ardalis.GuardClauses;
using Cackle.ConsoleApp.Features;
using CommandLine;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace Cackle.ConsoleApp.Internal;

/// <summary>
///     Provides framework for a console application with advanced argument parsing and baked in features. See
///     <see cref="CommandHostBuilder" /> to build.
/// </summary>
public class CommandHost : IDisposable, IAsyncDisposable
{
    /// <summary>
    ///     Commands registered during build
    /// </summary>
    private readonly ICommandCollection _commands;

    /// <summary>
    ///     Provides logging
    /// </summary>
    private readonly ILogger _log = Log.ForContext<CommandHost>();

    /// <summary>
    ///     Configured <see cref="Parser" />
    /// </summary>
    private readonly Parser _parser;

    /// <summary>
    ///     Allows for a safe, early termination of asynchronous command execution
    /// </summary>
    private readonly CancellationTokenSource _stopping = new();

    internal CommandHost(ICommandCollection commands, IServiceProvider services, Parser parser)
    {
        _commands = commands;
        Services = services;
        _parser = parser;
    }

    /// <summary>
    ///     Services available to the application.
    /// </summary>
    public IServiceProvider Services { get; }

    /// <summary>
    ///     Tracks the state of the application
    /// </summary>
    public CancellationToken HostStopping => _stopping.Token;

    /// <inheritdoc cref="IAsyncDisposable.DisposeAsync" />
    public async ValueTask DisposeAsync()
    {
        await CastAndDispose(_parser);
        await CastAndDispose(_stopping);

        GC.SuppressFinalize(this);

        return;

        static async ValueTask CastAndDispose(IDisposable resource)
        {
            if (resource is IAsyncDisposable resourceAsyncDisposable)
                await resourceAsyncDisposable.DisposeAsync();
            else
                resource.Dispose();
        }
    }

    /// <inheritdoc cref="IDisposable.Dispose" />
    public void Dispose()
    {
        _parser.Dispose();
        _stopping.Dispose();

        GC.SuppressFinalize(this);
    }

    /// <summary>
    ///     Run the registered commands
    /// </summary>
    /// <param name="args">Command line arguments provided by application</param>
    /// <returns>Exit code</returns>
    public int Run(string[] args)
    {
        LogEnvironment();
        
        var availableCommands = _commands.Where(r => r.IsSynchronous);
        var options = availableCommands.Select(c => c.ArgumentType).ToArray();
        Guard.Against.Zero(options.Length, nameof(options), "No synchronous commands registered");

        var parserResult = _parser.ParseArguments(args, options);

        if (parserResult is not Parsed<object>) return 1;

        foreach (var command in _commands)
        {
            if (command.ArgumentType != parserResult.Value.GetType()) continue;
            var service = Services.GetRequiredService(command.CommandType);

            var method = service.GetType().GetMethod(nameof(ICommand<ICommandArgs>.Invoke));
            Guard.Against.Null(method, nameof(method));

            if (method.Invoke(service, [parserResult.Value]) is not int result)
                throw new TypeLoadException("Received unexpected type upon command completion");

            return result;
        }

        _log.Fatal("No action taken, no commands matched parser");
        return 1;
    }

    /// <summary>
    ///     Run the registered asynchronous commands
    /// </summary>
    /// <param name="args">Command line arguments provided by application</param>
    /// <returns>Exit code</returns>
    public async Task<int> RunAsync(string[] args)
    {
        Console.CancelKeyPress += ConsoleOnCancelKeyPress;

        var exitCode = await RunAsync(args, HostStopping);

        Console.CancelKeyPress -= ConsoleOnCancelKeyPress;

        return exitCode;
    }

    /// <summary>
    ///     Run the registered asynchronous commands
    /// </summary>
    /// <param name="args">Command line arguments provided by application</param>
    /// <param name="ct">Provides a safe early termination of command</param>
    /// <returns>Exit code</returns>
    private async Task<int> RunAsync(string[] args, CancellationToken ct)
    {
        LogEnvironment();
        
        var availableCommands = _commands.Where(r => !r.IsSynchronous);
        var options = availableCommands.Select(c => c.ArgumentType).ToArray();
        Guard.Against.Zero(options.Length, nameof(options), "No asynchronous commands registered");

        var parserResult = _parser.ParseArguments(args, options);
        if (parserResult is not Parsed<object>) return 1;

        foreach (var command in _commands)
        {
            if (command.ArgumentType != parserResult.Value.GetType()) continue;
            var service = Services.GetRequiredService(command.CommandType);

            var method = service.GetType().GetMethod(nameof(ICommandAsync<ICommandArgs>.InvokeAsync));
            Guard.Against.Null(method, nameof(method));

            if (method.Invoke(service, [parserResult.Value, ct]) is not Task<int> result)
                throw new TypeLoadException("Received unexpected type upon command completion");

            try
            {
                return await result;
            }
            catch (OperationCanceledException)
            {
                _log.Fatal("Host has been terminated early");
            }
        }

        _log.Fatal("No action taken, no commands matched parser");
        return 1;
    }

    /// <summary>
    ///     Prints a line to the logging provider reporting the currently running environment.
    /// </summary>
    private void LogEnvironment()
    {
        _log.Information("Host Environment: {env}", HostEnv.Environment);
    }

    /// <summary>
    ///     User interrupted the application, log and close
    /// </summary>
    /// <param name="sender">Event Sender</param>
    /// <param name="e">See <see cref="ConsoleCancelEventArgs " /></param>
    private void ConsoleOnCancelKeyPress(object? sender, ConsoleCancelEventArgs e)
    {
        _log.Information("User requested application to stop before completion");
        _stopping.Cancel();
        e.Cancel = true;
    }
}