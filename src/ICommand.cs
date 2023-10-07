namespace Cackle.ConsoleApp;

/// <summary>
///     A synchronous console command with command line argument specifications
/// </summary>
/// <typeparam name="TArgs">See <see cref="ICommandArgs" /></typeparam>
public interface ICommand<in TArgs> where TArgs : ICommandArgs
{
    /// <summary>
    ///     Entry method for a command
    /// </summary>
    /// <param name="args">
    ///     Command line arguments configured using
    ///     <see href="https://github.com/commandlineparser/commandline">Command Line Parser</see>
    /// </param>
    /// <returns>Application exit code</returns>
    int Invoke(TArgs args);
}