using Spectre.Console.Cli;

namespace Cackle.ConsoleApp.Internal;

/// <summary>
///     An internal type resolver that bridges the Spectre.Console.CLI framework with the Microsoft Dependency Injection
///     <see cref="IServiceProvider" />.
/// </summary>
internal sealed class TypeResolver : ITypeResolver, IDisposable
{
    private readonly IServiceProvider _provider;

    /// <summary>
    ///     Initializes a new instance of the <see cref="TypeResolver" /> class.
    /// </summary>
    /// <param name="provider">The <see cref="IServiceProvider" /> to resolve types from.</param>
    internal TypeResolver(IServiceProvider provider)
    {
        _provider = provider;
    }

    /// <inheritdoc />
    public void Dispose()
    {
        // If the underlying service provider is disposable, dispose of it.
        if (_provider is IDisposable disposable) disposable.Dispose();
    }

    /// <inheritdoc />
    public object? Resolve(Type? type)
    {
        // If the requested type is null, return null. Otherwise, attempt to resolve the type from the service provider.
        return type == null ? null : _provider.GetService(type);
    }
}