using Cackle.ConsoleApp.Internal;

// ReSharper disable MemberCanBePrivate.Global

namespace Cackle.ConsoleApp.Features;

/// <summary>
///     Provides basic information about the running environment
/// </summary>
public class HostEnv : IHostEnv
{
    /// <summary>
    ///     Initializes a new <see cref="HostEnv" /> with information about the running environment
    /// </summary>
    public HostEnv(string? environmentName = null)
    {
        if (environmentName is null)
        {
            var env = System.Environment.GetEnvironmentVariable(AppConstants.Environment.VariableName);
            Environment = env ?? AppConstants.Environment.Production;
        }
        else
        {
            Environment = environmentName;
        }
    }

    /// <summary>
    ///     Name of running environment.
    /// </summary>
    public string Environment { get; }

    /// <summary>
    ///     If true, running environment name is <see cref="AppConstants.Environment.Production" />.
    /// </summary>
    public bool IsProduction => IsEnvironment(AppConstants.Environment.Production);

    /// <summary>
    ///     If true, running environment name is <see cref="AppConstants.Environment.Development" />.
    /// </summary>
    public bool IsDevelopment => IsEnvironment(AppConstants.Environment.Development);

    /// <summary>
    ///     Compares the running environment name to the provided value.
    /// </summary>
    /// <param name="environmentName">Environment name to verify.</param>
    /// <returns>True if the provided value equals the running environment.</returns>
    public bool IsEnvironment(string environmentName)
    {
        return string.Equals(Environment, environmentName, StringComparison.OrdinalIgnoreCase);
    }
}