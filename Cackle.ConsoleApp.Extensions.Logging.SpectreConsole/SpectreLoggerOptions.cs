// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Cackle.ConsoleApp.Internal;
using Microsoft.Extensions.Logging;

namespace Cackle.ConsoleApp.Extensions.Logging.SpectreConsole;

/// <summary>
///     Options for a <see cref="SpectreLogger" />.
/// </summary>
public class SpectreLoggerOptions
{
    /// <summary>
    ///     Backing field to <see cref="MaxQueueLength" />.
    /// </summary>
    private int _maxQueuedMessages = 2500;

    /// <summary>
    ///     Backing field to <see cref="QueueFullMode" />.
    /// </summary>
    private SpectreLoggerQueueFullMode _queueFullMode = SpectreLoggerQueueFullMode.Wait;

    /// <summary>
    ///     Gets or sets the log level for the logger.
    /// </summary>
    public LogLevel LogLevel { get; set; } = LogLevel.Information;

    /// <summary>
    ///     Gets or sets value indicating the minimum level of messages that get written to <c>Console.Error</c>.
    /// </summary>
    public LogLevel LogToStandardErrorThreshold { get; set; } = LogLevel.Error;

    /// <summary>
    ///     Gets or sets the desired console logger behavior when the queue becomes full.
    /// </summary>
    /// <value>
    ///     The default value is <see langword="wait" />.
    /// </value>
    public SpectreLoggerQueueFullMode QueueFullMode
    {
        get => _queueFullMode;
        set
        {
            if (value != SpectreLoggerQueueFullMode.Wait && value != SpectreLoggerQueueFullMode.DropWrite)
                throw new ArgumentOutOfRangeException(ResMan.Format("QueueModeNotSupported", nameof(value)));
            _queueFullMode = value;
        }
    }

    /// <summary>
    ///     Gets or sets the maximum number of enqueued messages.
    /// </summary>
    /// <value>
    ///     The default value is 2500.
    /// </value>
    public int MaxQueueLength
    {
        get => _maxQueuedMessages;
        set
        {
            if (value <= 0)
                throw new ArgumentOutOfRangeException(ResMan.Format("MaxQueueLengthBadValue", nameof(value)));

            _maxQueuedMessages = value;
        }
    }
}