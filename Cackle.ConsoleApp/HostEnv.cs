using Cackle.ConsoleApp.Internal;

namespace Cackle.ConsoleApp;

/// <summary>
///     Provides access to host environment information.
/// </summary>
public static class HostEnv
{
    private static string? _environment;
    private static string? _baseDirectory;
    private static string? _workingDirectory;

    /// <summary>
    ///     Gets the base directory of the application.
    /// </summary>
    public static string BaseDirectory
    {
        get
        {
            if (_baseDirectory is not null) return _baseDirectory;
            _baseDirectory = AppContext.BaseDirectory;
            return _baseDirectory;
        }
    }

    /// <summary>
    ///     Gets the current working directory of the application.
    /// </summary>
    public static string WorkingDirectory
    {
        get
        {
            if (_workingDirectory is not null) return _workingDirectory;
            _workingDirectory = Directory.GetCurrentDirectory();
            return _workingDirectory;
        }
    }

    /// <summary>
    ///     Gets the current environment name. Defaults to "Production" if not explicitly set.
    /// </summary>
    public static string Environment
    {
        get
        {
            if (_environment is not null) return _environment;

            var env = System.Environment.GetEnvironmentVariable(AppConstants.Environment.VariableName);
            _environment = env ?? AppConstants.Environment.Production;
            return _environment;
        }
    }

    /// <summary>
    ///     Checks if the current environment is "Production".
    /// </summary>
    public static bool IsProduction => IsEnvironment(AppConstants.Environment.Production);

    /// <summary>
    ///     Checks if the current environment is "Development".
    /// </summary>
    public static bool IsDevelopment => IsEnvironment(AppConstants.Environment.Development);

    /// <summary>
    ///     Checks if the current environment matches the specified <paramref name="environmentName" />.
    /// </summary>
    /// <param name="environmentName">The environment name to check against.</param>
    /// <returns><see langword="true" /> if the current environment matches; otherwise, <see langword="false" />.</returns>
    public static bool IsEnvironment(string environmentName)
    {
        return string.Equals(Environment, environmentName, StringComparison.OrdinalIgnoreCase);
    }
}