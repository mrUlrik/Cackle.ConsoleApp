// This code is derived from the Microsoft.Extensions.Logging library,
// which is licensed under the MIT License. See LICENSE file for details.

using Cackle.ConsoleApp.Internal;
using Spectre.Console;

namespace Cackle.ConsoleApp.Extensions.Logging.SpectreConsole;

/// <summary>
///     An internal processor that handles the asynchronous writing of log messages to the console using Spectre.Console.
///     It manages a queue of log messages and writes them on a dedicated thread.
/// </summary>
internal sealed class SpectreLoggerProcessor : IDisposable
{
    private readonly IAnsiConsole _console;
    private readonly IAnsiConsole _errorConsole;
    private readonly Queue<LogMessageEntry> _messageQueue;
    private readonly Thread _outputThread;

    private ExceptionSettings _exceptionSettings;
    private SpectreLoggerQueueFullMode _fullMode = SpectreLoggerQueueFullMode.Wait;
    private bool _isAddingCompleted;

    private int _maxQueuedMessages;
    private volatile int _messagesDropped;

    /// <summary>
    ///     Initializes a new instance of the <see cref="SpectreLoggerProcessor" /> class.
    /// </summary>
    /// <param name="console">The <see cref="IAnsiConsole" /> instance for standard output.</param>
    /// <param name="errorConsole">The <see cref="IAnsiConsole" /> instance for standard error output.</param>
    /// <param name="exceptionSettings">The <see cref="ExceptionSettings" /> for formatting exceptions.</param>
    /// <param name="fullMode">The <see cref="SpectreLoggerQueueFullMode" /> to determine behavior when the queue is full.</param>
    /// <param name="maxQueueLength">The maximum number of messages allowed in the queue.</param>
    public SpectreLoggerProcessor(IAnsiConsole console, IAnsiConsole errorConsole, ExceptionSettings exceptionSettings,
        SpectreLoggerQueueFullMode fullMode, int maxQueueLength)
    {
        _console = console;
        _errorConsole = errorConsole;
        _exceptionSettings = exceptionSettings;
        _messageQueue = new Queue<LogMessageEntry>();

        MaxQueueLength = maxQueueLength;
        FullMode = fullMode;

        // Create and start a dedicated background thread to process the log message queue.
        _outputThread = new Thread(ProcessLogQueue)
        {
            IsBackground = true,
            Name = "SpectreConsole Log Processor"
        };
        _outputThread.Start();
    }

    /// <summary>
    ///     Gets or sets the maximum number of messages that can be queued.
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if the value is less than or equal to zero.</exception>
    public int MaxQueueLength
    {
        get => _maxQueuedMessages;
        set
        {
            if (value <= 0)
                throw new ArgumentOutOfRangeException(ResMan.Format(
                    "MaxQueueLengthBadValue", nameof(value)));

            // Lock the queue to safely update the maximum length and signal waiting threads.
            lock (_messageQueue)
            {
                _maxQueuedMessages = value;
                Monitor.PulseAll(_messageQueue);
            }
        }
    }

    /// <summary>
    ///     Gets or sets the behavior when the log message queue is full.
    /// </summary>
    public SpectreLoggerQueueFullMode FullMode
    {
        get => _fullMode;
        set
        {
            // Lock the queue to safely update the full mode and signal waiting threads.
            lock (_messageQueue)
            {
                _fullMode = value;
                Monitor.PulseAll(_messageQueue);
            }
        }
    }

    /// <summary>
    ///     Gets or sets the <see cref="ExceptionSettings" /> used for formatting exceptions.
    /// </summary>
    public ExceptionSettings ExceptionSettings
    {
        get => _exceptionSettings;
        set
        {
            lock (_messageQueue)
            {
                _exceptionSettings = value;
                Monitor.PulseAll(_messageQueue);
            }
        }
    }

    /// <inheritdoc />
    public void Dispose()
    {
        // Indicate that no more messages will be added to the queue.
        CompleteAdding();

        try
        {
            // Wait for the output thread to finish processing the remaining messages with a timeout in case we're locked by user input.
            _outputThread.Join(1500);
        }
        catch (ThreadStateException)
        {
            // Ignore thread state exceptions, as the thread might have already terminated.
        }
    }

    /// <summary>
    ///     Processes the log message queue by dequeuing messages and writing them to the console. This method runs on the
    ///     dedicated output thread.
    /// </summary>
    private void ProcessLogQueue()
    {
        // Continue processing until no more messages can be dequeued (either the queue is empty and adding is complete).
        while (TryDequeue(out var message)) WriteMessage(message);
    }

    /// <summary>
    ///     Enqueues a log message entry to be processed. If the queue is full and the full mode is DropWrite, the message is
    ///     written directly instead of being queued.
    /// </summary>
    /// <param name="message">The <see cref="LogMessageEntry" /> to enqueue.</param>
    public void EnqueueMessage(LogMessageEntry message)
    {
        // Try to enqueue the message. If enqueueing fails (queue full and DropWrite mode), write the message immediately.
        if (!Enqueue(message)) WriteMessage(message);
    }

    /// <summary>
    ///     Attempts to enqueue a log message entry. Handles queue capacity and the QueueFullMode.
    /// </summary>
    /// <param name="message">The <see cref="LogMessageEntry" /> to enqueue.</param>
    /// <returns><see langword="true" /> if the message was enqueued; otherwise, <see langword="false" />.</returns>
    private bool Enqueue(LogMessageEntry message)
    {
        lock (_messageQueue)
        {
            // Wait while the queue is full and adding is not completed.
            while (_messageQueue.Count >= MaxQueueLength && !_isAddingCompleted)
            {
                // If the queue is full and the mode is DropWrite, increment the dropped message count and return true (message "handled").
                if (FullMode == SpectreLoggerQueueFullMode.DropWrite)
                {
                    Interlocked.Increment(ref _messagesDropped);
                    return true;
                }

                // Wait for space to become available in the queue.
                Monitor.Wait(_messageQueue);
            }

            // If adding is completed, no more messages should be enqueued.
            if (_isAddingCompleted) return false;

            // Check if the queue was empty before adding the current message.
            var startedEmpty = _messageQueue.Count == 0;

            // If messages were dropped, enqueue a warning message about the dropped messages.
            if (_messagesDropped > 0)
            {
                _messageQueue.Enqueue(new LogMessageEntry(
                    ResMan.Format("WarningMessageOnDrop", _messagesDropped),
                    true
                ));

                _messagesDropped = 0;
            }

            // Enqueue the actual log message.
            _messageQueue.Enqueue(message);

            // If the queue was empty, signal the output thread that there are messages to process.
            if (startedEmpty) Monitor.PulseAll(_messageQueue);

            return true;
        }
    }

    /// <summary>
    ///     Attempts to dequeue a log message entry from the queue.
    /// </summary>
    /// <param name="item">The dequeued <see cref="LogMessageEntry" /> if successful; otherwise, the default value.</param>
    /// <returns><see langword="true" /> if a message was dequeued; otherwise, <see langword="false" />.</returns>
    private bool TryDequeue(out LogMessageEntry item)
    {
        lock (_messageQueue)
        {
            // Wait while the queue is empty and adding is not completed.
            while (_messageQueue.Count == 0 && !_isAddingCompleted) Monitor.Wait(_messageQueue);

            if (_messageQueue.Count > 0)
            {
                item = _messageQueue.Dequeue();

                // If the queue was full before dequeuing, signal any threads waiting to enqueue.
                if (_messageQueue.Count == MaxQueueLength - 1)
                    Monitor.PulseAll(_messageQueue);

                return true;
            }

            // No messages in the queue and adding is complete.
            item = default;
            return false;
        }
    }

    /// <summary>
    ///     Writes a single log message entry to the appropriate console (standard output or standard error).
    /// </summary>
    /// <param name="message">The <see cref="LogMessageEntry" /> to write.</param>
    private void WriteMessage(LogMessageEntry message)
    {
        try
        {
            // Determine which console to use and write formatted message to chosen console.
            var console = message.LogAsError ? _errorConsole : _console;
            console.MarkupLine(message.Message.TrimEnd());

            // If there's an exception associated with the message, write it to the console using the configured settings.
            if (message.Exception is not null)
                console.WriteException(message.Exception, _exceptionSettings);
        }
        catch
        {
            // If an exception occurs during writing (e.g., console closed), stop adding more messages.
            CompleteAdding();
        }
    }

    /// <summary>
    ///     Signals that no more messages will be added to the queue.
    /// </summary>
    private void CompleteAdding()
    {
        lock (_messageQueue)
        {
            _isAddingCompleted = true;

            // Signal all waiting threads (both enqueue and dequeue) that adding is complete.
            Monitor.PulseAll(_messageQueue);
        }
    }
}