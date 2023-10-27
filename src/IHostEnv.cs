namespace Cackle.ConsoleApp;

/// <summary>
///     Provides basic information about the running environment
/// </summary>
public interface IHostEnv
{
    /// <summary>
    ///     Name of running environment
    /// </summary>
    string Environment { get; }

    /// <summary>
    ///     If true, environment is Production
    /// </summary>
    bool IsProduction { get; }

    /// <summary>
    ///     If true, environment is Development
    /// </summary>
    bool IsDevelopment { get; }
}