using Spectre.Console.Cli;

namespace Cackle.ConsoleApp;

/// <summary>
///     Represents the host for the console application, encapsulating the Spectre.Console <see cref="CommandApp" />.
/// </summary>
public class ConsoleHost
{
    private readonly CommandApp _app;

    /// <summary>
    ///     Initializes a new instance of the <see cref="ConsoleHost" /> class.
    /// </summary>
    /// <param name="app">The configured <see cref="CommandApp" /> instance.</param>
    internal ConsoleHost(CommandApp app)
    {
        _app = app;
    }

    /// <summary>
    ///     Runs the console application with the specified arguments.
    /// </summary>
    /// <param name="args">The command line arguments.</param>
    /// <returns>The exit code of the application.</returns>
    public int Run(string[] args)
    {
        return _app.Run(args);
    }

    /// <summary>
    ///     Runs the console application asynchronously with the specified arguments.
    /// </summary>
    /// <param name="args">The command line arguments.</param>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation, with the exit code of the application.</returns>
    public Task<int> RunAsync(string[] args)
    {
        return _app.RunAsync(args);
    }
}