using Cackle.ConsoleApp;
using CommandLine;

namespace DataFetchExample.Arguments;

[Verb("fetch", HelpText = "Fetch fun information about a number")]
internal class FetchArgs(string type, int number) : ICommandArgs
{
    [Option('t', "type", Default = "trivia", HelpText = "TypeHelp", ResourceType = typeof(Resource))]
    public string Type => type;

    [Option('n', "number", HelpText = "NumberHelp", ResourceType = typeof(Resource), Required = true)]
    public int Number => number;
}