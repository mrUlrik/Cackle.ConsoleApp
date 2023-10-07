namespace Cackle.ConsoleApp.Internal;

/// <summary>
///     Constant strings used in the application
/// </summary>
internal static class AppConstants
{
    /// <summary>
    ///     Strings regarding environment
    /// </summary>
    internal static class Environment
    {
        /// <summary>
        ///     Running environment: Production
        /// </summary>
        internal static string Production = "Production";

        /// <summary>
        ///     Running environment: Development
        /// </summary>
        internal static string Development = "Development";
    }

    /// <summary>
    ///     Strings regarding configuration
    /// </summary>
    internal static class Configuration
    {
        /// <summary>
        ///     The <see cref="Microsoft.Extensions.Configuration.IConfigurationSection" />, starting at root, where the
        ///     configuration sections for each command are stored
        /// </summary>
        internal static string CommandSection = "Commands";
    }
}