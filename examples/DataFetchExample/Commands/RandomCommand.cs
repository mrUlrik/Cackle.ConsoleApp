using Cackle.ConsoleApp;
using DataFetchExample.Arguments;
using DataFetchExample.Configuration;
using DataFetchExample.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Serilog;

namespace DataFetchExample.Commands;

/// <summary>
///     Fetch information about a random number using a minimum and maximum specified in the appsettings.json
/// </summary>
internal class RandomCommand(
    ILogger<FetchCommand> log,
    IOptions<RandomConfig> config,
    NumbersApi api
) : ICommandAsync<RandomArgs>
{
    public async Task<int> InvokeAsync(RandomArgs args, CancellationToken cancellationToken)
    {
        var response = await api.GetRandomAsync(
            config.Value.MinInteger, 
            config.Value.MaxInteger, 
            args.Type,
            cancellationToken);

        if (response is null)
        {
            log.LogError("{error}", Resource.NoResponse);
            return 1;
        }
         
        log.LogInformation("{success}", response.Text);
        return 0;
    }
}