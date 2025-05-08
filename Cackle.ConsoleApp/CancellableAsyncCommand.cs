using Cackle.ConsoleApp.Internal;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Cackle.ConsoleApp;

/// <summary>
///     An abstract asynchronous command that supports cancellation via a <see cref="CancellationToken" /> and handles
///     Ctrl+C (CancelKeyPress) events gracefully.
/// </summary>
/// <typeparam name="TSettings">The settings type for the command.</typeparam>
public abstract class CancellableAsyncCommand<TSettings> : ICommand<TSettings>, IDisposable
    where TSettings : CommandSettings
{
    private readonly CancellationTokenSource _cts = new();
    private bool _disposedValue;

    /// <summary>
    ///     Initializes a new instance of the <see cref="CancellableAsyncCommand{TSettings}" /> class and subscribes to the
    ///     <see cref="Console.CancelKeyPress" /> event.
    /// </summary>
    protected CancellableAsyncCommand()
    {
        // Subscribe to the Console.CancelKeyPress event to handle Ctrl+C.
        Console.CancelKeyPress += ConsoleOnCancelKeyPress;
    }

    /// <inheritdoc />
    ValidationResult ICommand.Validate(CommandContext context, CommandSettings settings)
    {
        return Validate(context, (TSettings)settings);
    }

    /// <inheritdoc />
    Task<int> ICommand.Execute(CommandContext context, CommandSettings settings)
    {
        return ((ICommand<TSettings>)this).Execute(context, (TSettings)settings);
    }

    /// <inheritdoc />
    Task<int> ICommand<TSettings>.Execute(CommandContext context, TSettings settings)
    {
        try
        {
            return ExecuteAsync(context, settings, _cts.Token);
        }
        catch (OperationCanceledException ex)
        {
            AnsiConsole.MarkupLine(ResMan.GetString("Message_OperationCancelled"));
            return Task.FromResult(ex.HResult);
        }
        finally
        {
            Dispose();
        }
    }

    /// <inheritdoc />
    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    ///     Handles the <see cref="Console.CancelKeyPress" /> event.
    ///     Cancels the ongoing operation and prevents the default process termination.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="ConsoleCancelEventArgs" /> instance containing the event data.</param>
    private void ConsoleOnCancelKeyPress(object? sender, ConsoleCancelEventArgs e)
    {
        // Write a message to the console indicating that a cancellation request was received.
        AnsiConsole.WriteLine(ResMan.GetString("Message_CancelKeyPress"));

        // Cancel the associated CancellationToken.
        _cts.Cancel();

        // Prevent the default behavior of terminating the process immediately
        e.Cancel = true;
    }

    /// <summary>
    ///     Validates the command settings. This method can be overridden in derived classes.
    /// </summary>
    /// <param name="context">The command context.</param>
    /// <param name="settings">The command settings.</param>
    /// <returns>A <see cref="ValidationResult" /> indicating whether the settings are valid.</returns>
    protected virtual ValidationResult Validate(CommandContext context, TSettings settings)
    {
        return ValidationResult.Success();
    }

    /// <summary>
    ///     Executes the command asynchronously. This method must be implemented by derived classes.
    /// </summary>
    /// <param name="context">The command context.</param>
    /// <param name="settings">The command settings.</param>
    /// <param name="cancellationToken">
    ///     A <see cref="CancellationToken" /> that can be used to cancel the asynchronous operation.
    /// </param>
    public abstract Task<int> ExecuteAsync(CommandContext context, TSettings settings,
        CancellationToken cancellationToken);

    /// <summary>
    ///     Releases unmanaged and optionally managed resources.
    /// </summary>
    /// <param name="disposing">
    ///     <see langword="true" /> to release both managed and unmanaged resources;
    ///     <see langword="false" /> to release only unmanaged resources.
    /// </param>
    protected virtual void Dispose(bool disposing)
    {
        // Check if the object has already been disposed.
        if (_disposedValue) return;

        // If disposing is true, dispose of managed resources.
        if (disposing)
        {
            _cts.Dispose();
            Console.CancelKeyPress -= ConsoleOnCancelKeyPress;
        }

        _disposedValue = true;
    }
}