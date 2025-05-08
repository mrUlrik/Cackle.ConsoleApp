using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Configuration;
using Microsoft.Extensions.Options;

namespace Cackle.ConsoleApp.Extensions.Logging.SpectreConsole;

/// <summary>
///     An internal class that configures <see cref="SpectreLoggerOptions" /> using the application's configuration.
/// </summary>
internal sealed class SpectreLoggerConfigureOptions : IConfigureOptions<SpectreLoggerOptions>
{
    private readonly IConfiguration _configuration;

    /// <summary>
    ///     Initializes a new instance of the <see cref="SpectreLoggerConfigureOptions" /> class.
    /// </summary>
    /// <param name="providerConfiguration">
    ///     The <see cref="ILoggerProviderConfiguration{SpectreLoggerProvider}" /> from which to extract the configuration.
    /// </param>
    public SpectreLoggerConfigureOptions(ILoggerProviderConfiguration<SpectreLoggerProvider> providerConfiguration)
    {
        _configuration = providerConfiguration.Configuration;
    }

    /// <inheritdoc />
    public void Configure(SpectreLoggerOptions options)
    {
        _configuration.Bind(options);
    }
}