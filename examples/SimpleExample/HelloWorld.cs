using Cackle.ConsoleApp;
using CommandLine;

namespace SimpleExample;

/// <summary>
///     Print a greeting to the console
/// </summary>
internal class HelloWorld : ICommand<HelloWorldArgs>
{
    /// <summary>
    ///     Entry point into the commandAsync
    /// </summary>
    /// <param name="options">Command line arguments</param>
    /// <returns>Program exit code</returns>
    public int Invoke(HelloWorldArgs options)
    {
        if (options.Verbose) Console.WriteLine("Parsed user input, printing to screen...");
        Console.WriteLine($"Hello {options.Target}!");

        return 0;
    }
}

/// <summary>
///     Reply with a greeting
/// </summary>
[Verb("greeting", true)]
internal class HelloWorldArgs : ICommandArgs
{
    /// <summary>
    ///     Produce verbose output
    /// </summary>
    [Option('v', "verbose", Required = false, HelpText = "Set output to verbose messages.")]
    public bool Verbose { get; set; }

    /// <summary>
    ///     The target of your greeting
    /// </summary>
    [Option('t', "target", Required = true, HelpText = "Who do you want to say hello to?")]
    public string Target { get; set; } = default!;
}