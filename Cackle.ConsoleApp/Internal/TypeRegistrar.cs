// This code is derived from portions of the Spectre.Console.Cli.Extensions.DependencyInjection
// library, which is licensed under the MIT License. See LICENSE file for details.
// See: https://github.com/WCOMAB/Spectre.Console.Cli.Extensions.DependencyInjection

using Microsoft.Extensions.DependencyInjection;
using Spectre.Console.Cli;

namespace Cackle.ConsoleApp.Internal;

/// <summary>
///     An internal type registrar that bridges the Spectre.Console.CLI framework's type registration mechanism with the
///     Microsoft Dependency Injection <see cref="IServiceCollection" />.
/// </summary>
/// <param name="services">The <see cref="IServiceCollection" /> to register types with.</param>
internal sealed class TypeRegistrar(IServiceCollection services) : ITypeRegistrar, IDisposable
{
    private readonly List<IDisposable> _providers = [];
    private bool _disposed;

    /// <inheritdoc />
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <inheritdoc />
    public ITypeResolver Build()
    {
        // Build the service provider from the registered services and track it for disposal.
        var provider = services.BuildServiceProvider();
        _providers.Add(provider);

        // Return a new TypeResolver that uses the built service provider.
        return new TypeResolver(provider);
    }

    /// <inheritdoc />
    public void Register(Type service, Type implementation)
    {
        services.AddSingleton(service, implementation);
    }

    /// <inheritdoc />
    public void RegisterInstance(Type service, object implementation)
    {
        services.AddSingleton(service, implementation);
    }

    /// <inheritdoc />
    public void RegisterLazy(Type service, Func<object> func)
    {
        ArgumentNullException.ThrowIfNull(func);
        services.AddSingleton(service, _ => func());
    }

    /// <summary>
    ///     Releases managed resources.
    /// </summary>
    /// <param name="disposing">A boolean value indicating whether to dispose of managed resources.</param>
    private void Dispose(bool disposing)
    {
        // Check if the object has already been disposed.
        if (_disposed) return;

        // If disposing is true, dispose all created service providers.
        if (disposing)
            foreach (var provider in _providers)
                provider.Dispose();

        _disposed = true;
    }

    /// <summary>
    ///     Finalizes an instance of the <see cref="TypeRegistrar" /> class. This finalizer will run only if the Dispose method
    ///     was not called.
    /// </summary>
    ~TypeRegistrar()
    {
        Dispose(false);
    }
}