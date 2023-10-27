using Cackle.ConsoleApp;
using DataFetchExample.Arguments;
using DataFetchExample.Services;
using Microsoft.Extensions.Logging;

namespace DataFetchExample.Commands;

/// <summary>
///     Fetch information about a number then print to the console
/// </summary>
internal class FetchCommand(ILogger<FetchCommand> log, IHostEnv env, NumbersApi api) : ICommandAsync<FetchArgs>
{
    public async Task<int> InvokeAsync(FetchArgs args, CancellationToken cancellationToken)
    {
        log.LogInformation("Operating in {env}", env.Environment);

        var response = await api.GetNumberAsync(args.Number, args.Type, cancellationToken);
        if (response is null)
        {
            log.LogError("{error}", Resource.NoResponse);
            return 1;
        }

        log.LogInformation("{success}", response.Text);
        return 0;
    }
}