using Cackle.ConsoleApp;
using CommandLine;

namespace DataFetchExample.Arguments;

[Verb("random", HelpText = "Fetch information about a random number")]
internal class RandomArgs(string type) : ICommandArgs
{
    [Option('t', "type", Default = "trivia", HelpText = "TypeHelp", ResourceType = typeof(Resource))]
    public string Type => type;
}