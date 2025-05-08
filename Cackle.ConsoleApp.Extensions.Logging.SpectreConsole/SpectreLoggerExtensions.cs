using Cackle.ConsoleApp.Internal.FluentInterfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Configuration;

namespace Cackle.ConsoleApp.Extensions.Logging.SpectreConsole;

/// <summary>
///     Extension methods for adding and configuring the Spectre.Console logging provider to the
///     Microsoft.Extensions.Logging framework.
/// </summary>
public static class SpectreLoggerExtensions
{
    /// <summary>
    ///     An internal helper method to get the "FormatterOptions" section from the logger provider configuration.
    /// </summary>
    /// <param name="providerConfiguration">The <see cref="ILoggerProviderConfiguration{SpectreLoggerProvider}" />.</param>
    /// <returns>The <see cref="IConfiguration" /> section for "FormatterOptions".</returns>
    internal static IConfiguration GetFormatterOptionsSection(
        this ILoggerProviderConfiguration<SpectreLoggerProvider> providerConfiguration)
    {
        return providerConfiguration.Configuration.GetSection("FormatterOptions");
    }

    /// <summary>
    ///     Configures logging for the console host builder using the provided configuration action.
    /// </summary>
    /// <param name="builder">The <see cref="IConfigureConsoleHostBuilder" /> instance.</param>
    /// <param name="configure">An action to configure the <see cref="ILoggingBuilder" />.</param>
    /// <returns>The <see cref="IConfigureConsoleHostBuilder" /> instance for chaining.</returns>
    /// <remarks>
    ///     This method delegates to the standard
    ///     <see cref="LoggingServiceCollectionExtensions.AddLogging(IServiceCollection, Action{ILoggingBuilder})" /> method.
    /// </remarks>
    public static IConfigureConsoleHostBuilder ConfigureLogging(this IConfigureConsoleHostBuilder builder,
        Action<ILoggingBuilder> configure)
    {
        return builder.ConfigureServices(services => services.AddLogging(configure));
    }

    /// <summary>
    ///     Adds the Spectre.Console logging provider to the console host builder with default configuration based on the
    ///     "Logging" section of the application's configuration.
    /// </summary>
    /// <param name="builder">The <see cref="IConfigureConsoleHostBuilder" /> instance.</param>
    /// <returns>The <see cref="IConfigureConsoleHostBuilder" /> instance for chaining.</returns>
    public static IConfigureConsoleHostBuilder AddSpectreConsoleLogging(this IConfigureConsoleHostBuilder builder)
    {
        // Configure services to add logging and then add the SpectreConsole logger.
        builder.ConfigureServices((services, config) =>
            services.AddLogging(loggingBuilder =>
                loggingBuilder.AddSpectreConsole(config)));

        return builder;
    }

    /// <summary>
    ///     Adds the Spectre.Console logging provider to the logging builder, using the specified configuration.
    /// </summary>
    /// <param name="builder">The <see cref="ILoggingBuilder" /> instance.</param>
    /// <param name="config">The <see cref="IConfiguration" /> containing logging settings (the "Logging" section).</param>
    /// <returns>The <see cref="ILoggingBuilder" /> instance for chaining.</returns>
    public static ILoggingBuilder AddSpectreConsole(this ILoggingBuilder builder, IConfiguration config)
    {
        // Add configuration for logging from the specified configuration section.
        builder.AddConfiguration(config.GetSection("Logging"));

        // Register the SpectreLoggerProvider as a singleton service and it's associated SpectreLoggerOptions.
        builder.Services.AddSingleton<ILoggerProvider, SpectreLoggerProvider>();
        builder.Services.ConfigureOptions<SpectreLoggerConfigureOptions>();

        // Register the SpectreFormatter as a singleton service and it's associated SpectreFormatterOptions.
        builder.Services.AddSingleton<SpectreFormatter>();
        builder.Services.ConfigureOptions<SpectreFormatterConfigureOptions>();
        return builder;
    }
}