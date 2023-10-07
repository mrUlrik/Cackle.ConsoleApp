using System.Net.Http.Headers;
using Ardalis.GuardClauses;
using Cackle.ConsoleApp;
using DataFetchExample;
using DataFetchExample.Arguments;
using DataFetchExample.Commands;
using DataFetchExample.Configuration;
using DataFetchExample.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

// Create a host to handle services, configuration, and execution
var builder = CommandHostBuilder.Create();

// Register services needed for the commands
builder.RegisterServices((services, config) =>
{
    var baseAddress = config.GetValue<string>("BaseAddress");
    Guard.Against.Null(baseAddress, "BaseAddress", Resource.MissingBaseAddressException);
    services.AddHttpClient<NumbersApi>(client =>
    {
        client.BaseAddress = new Uri(baseAddress);
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
    });
});

// Register our command with it's argument specification
builder.RegisterAsyncCommand<FetchCommand, FetchArgs>();
builder.RegisterAsyncCommand<RandomCommand, RandomArgs, RandomConfig>();

// Finally, build the host and run it
return await builder.Build().RunAsync(args);