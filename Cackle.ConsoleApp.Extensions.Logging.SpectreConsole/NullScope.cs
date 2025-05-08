namespace Cackle.ConsoleApp.Extensions.Logging.SpectreConsole;

/// <summary>
///     Represents an empty scope without any logic.
/// </summary>
public sealed class NullScope : IDisposable
{
    /// <summary>
    ///     Prevents a default instance of the <see cref="NullScope" /> class from being created.
    /// </summary>
    private NullScope()
    {
    }

    /// <summary>
    ///     Gets the singleton instance of the <see cref="NullScope" /> class.
    /// </summary>
    public static NullScope Instance { get; } = new();

    /// <summary>
    ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    public void Dispose()
    {
    }
}