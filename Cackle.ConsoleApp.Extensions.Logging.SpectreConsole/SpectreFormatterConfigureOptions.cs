// This code is derived from the Microsoft.Extensions.Logging library,
// which is licensed under the MIT License. See LICENSE file for details.

using System.Runtime.Versioning;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Configuration;
using Microsoft.Extensions.Options;

namespace Cackle.ConsoleApp.Extensions.Logging.SpectreConsole;

/// <summary>
///     Configures the options for the <see cref="SpectreFormatterOptions" /> used by the Spectre Console Logger.
/// </summary>
/// <remarks>
///     This class retrieves the formatter options section from the logger provider configuration
///     and applies it to the <see cref="SpectreFormatterOptions" />.
/// </remarks>
/// <param name="providerConfiguration">
///     Provides access to the configuration section for the <see cref="SpectreLoggerProvider" />.
/// </param>
internal sealed class SpectreFormatterConfigureOptions(
    ILoggerProviderConfiguration<SpectreLoggerProvider> providerConfiguration)
    : IConfigureOptions<SpectreFormatterOptions>
{
    /// <summary>
    ///     The configuration section for the formatter options.
    /// </summary>
    private readonly IConfiguration _configuration = providerConfiguration.GetFormatterOptionsSection();

    /// <summary>
    ///     Configures the <see cref="SpectreFormatterOptions" /> using the provided configuration.
    /// </summary>
    /// <param name="options">The options instance to configure.</param>
    public void Configure(SpectreFormatterOptions options)
    {
        options.Configure(_configuration);
    }
}