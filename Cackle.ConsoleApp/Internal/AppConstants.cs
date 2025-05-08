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
        internal const string Production = "Production";

        /// <summary>
        ///     Running environment: Development
        /// </summary>
        internal const string Development = "Development";

        /// <summary>
        ///     Name of the environment variable to fetch the running environment
        /// </summary>
        internal const string VariableName = "DOTNET_ENVIRONMENT";
    }
}