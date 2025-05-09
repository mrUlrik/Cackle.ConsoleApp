using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Cackle.ConsoleApp.Internal;

namespace Cackle.ConsoleApp;

/// <summary>
///     Provides access to host environment information.
/// </summary>
[SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
public static class HostEnv
{
    private static string? _environment;
    private static string? _baseDirectory;
    private static string? _workingDirectory;
    private static string? _processName;

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
    /// <remarks>
    ///     This property attempts to retrieve the current working directory. If an exception occurs (e.g.,
    ///     <see cref="UnauthorizedAccessException" /> or <see cref="NotSupportedException" />), it defaults to returning the
    ///     current directory as '.'.
    /// </remarks>
    public static string WorkingDirectory
    {
        get
        {
            if (_workingDirectory is not null) return _workingDirectory;

            _workingDirectory = System.Environment.CurrentDirectory;
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
            GetProcessName();
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
    ///     The name of the current process.
    /// </summary>
    public static string ProcessName
    {
        get
        {
            if (_processName is not null) return _processName;

            _processName = GetProcessName();
            return _processName;
        }
    }

    /// <summary>
    ///     Checks if the current environment matches the specified <paramref name="environmentName" />.
    /// </summary>
    /// <param name="environmentName">The environment name to check against.</param>
    /// <returns><see langword="true" /> if the current environment matches; otherwise, <see langword="false" />.</returns>
    public static bool IsEnvironment(string environmentName)
    {
        return string.Equals(Environment, environmentName, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    ///     Retrieves the name of the current process.
    /// </summary>
    /// <remarks>
    ///     This method attempts to determine the process name by inspecting the main module of the current process. If the
    ///     process is not running under the "dotnet" host, it uses the file name of the main module. Otherwise, it falls back
    ///     to the entry assembly's location. If neither approach succeeds, an exception is thrown.
    /// </remarks>
    /// <returns>A <see cref="string" /> representing the name of the current process.</returns>
    /// <exception cref="InvalidOperationException">Thrown if the process name cannot be determined.</exception>
    private static string GetProcessName()
    {
        var process = Process.GetCurrentProcess();
        if (process.MainModule is not null)
        {
            // Extract the file name without its extension from the main module's file name and if the file name is not
            // "dotnet" (case-insensitive comparison), use the full file name of the main module.
            var fileName = Path.GetFileNameWithoutExtension(process.MainModule.FileName);
            if (!string.Equals(fileName, "dotnet", StringComparison.InvariantCultureIgnoreCase))
                return Path.GetFileName(process.MainModule.FileName);
        }

        // Attempt to retrieve the entry assembly of the application and if the entry assembly's location is available,
        // extract and cache its file name.
        var assembly = Assembly.GetEntryAssembly();
        if (assembly?.Location is null) throw new InvalidOperationException(Strings.Message_ProcessName);

        return Path.GetFileName(assembly.Location);
    }
}