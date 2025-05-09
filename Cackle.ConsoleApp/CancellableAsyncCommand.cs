using Cackle.ConsoleApp.Internal;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Cackle.ConsoleApp;

/// <summary>
///     An abstract asynchronous command that supports cancellation via a <see cref="CancellationToken" /> and handles
///     Ctrl+C (CancelKeyPress) events gracefully. This command uses <see cref="EmptyCommandSettings" />.
/// </summary>
public abstract class CancellableAsyncCommand : ICommand<EmptyCommandSettings>, IDisposable
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
    Task<int> ICommand<EmptyCommandSettings>.Execute(CommandContext context, EmptyCommandSettings settings)
    {
        try
        {
            // Execute the asynchronous command with the provided context and cancellation token.
            return ExecuteAsync(context, _cts.Token);
        }
        catch (OperationCanceledException ex)
        {
            // If the operation is cancelled, write a message to the console.
            AnsiConsole.MarkupLine(ResMan.GetString("Message_OperationCancelled"));
            return Task.FromResult(ex.HResult);
        }
        finally
        {
            // Ensure resources are disposed of after execution.
            Dispose();
        }
    }

    /// <inheritdoc />
    Task<int> ICommand.Execute(CommandContext context, CommandSettings settings)
    {
        // Explicitly cast the command to its generic interface to call the generic Execute method.
        return ((ICommand<EmptyCommandSettings>)this).Execute(context, (EmptyCommandSettings)settings);
    }

    /// <inheritdoc />
    ValidationResult ICommand.Validate(CommandContext context, CommandSettings settings)
    {
        return ValidationResult.Success();
    }

    /// <inheritdoc />
    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    ///     Executes the command asynchronously. This method must be implemented by derived classes.
    /// </summary>
    /// <param name="context">The command context.</param>
    /// <param name="cancellationToken">
    ///     A <see cref="CancellationToken" /> that can be used to cancel the asynchronous
    ///     operation.
    /// </param>
    protected abstract Task<int> ExecuteAsync(CommandContext context, CancellationToken cancellationToken);

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