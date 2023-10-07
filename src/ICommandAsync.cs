namespace Cackle.ConsoleApp;

/// <summary>
///     An asynchronous console command with command line argument specifications
/// </summary>
/// <typeparam name="TArgs">See <see cref="ICommandArgs" /></typeparam>
public interface ICommandAsync<in TArgs> where TArgs : ICommandArgs
{
    /// <summary>
    ///     Entry method for a command
    /// </summary>
    /// <param name="args">
    ///     Command line arguments configured using
    ///     <see href="https://github.com/commandlineparser/commandline">Command Line Parser</see>
    /// </param>
    /// <param name="ct">Allows for a safe, early termination of request</param>
    /// <returns>Application exit code</returns>
    Task<int> InvokeAsync(TArgs args, CancellationToken ct);
}



// Comments add to test tag