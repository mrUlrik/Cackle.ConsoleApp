using Cackle.ConsoleApp.Internal;

// ReSharper disable MemberCanBePrivate.Global

namespace Cackle.ConsoleApp.Features;

/// <summary>
///     Provides basic information about the running environment
/// </summary>
public static class HostEnv
{
    private static string? _environment;
    private static string? _baseDirectory;
    private static string? _workingDirectory;

    /// <summary>
    ///     The base directory of the currently running application.
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
    ///     Gets the current working directory of the application, does not include a backslash.
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
    ///     Name of running environment.
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
    ///     If true, running environment name is <see cref="AppConstants.Environment.Production" />.
    /// </summary>
    public static bool IsProduction => IsEnvironment(AppConstants.Environment.Production);

    /// <summary>
    ///     If true, running environment name is <see cref="AppConstants.Environment.Development" />.
    /// </summary>
    public static bool IsDevelopment => IsEnvironment(AppConstants.Environment.Development);

    /// <summary>
    ///     Compares the running environment name to the provided value.
    /// </summary>
    /// <param name="environmentName">Environment name to verify.</param>
    /// <returns>True if the provided value equals the running environment.</returns>
    public static bool IsEnvironment(string environmentName)
    {
        return string.Equals(Environment, environmentName, StringComparison.OrdinalIgnoreCase);
    }
}