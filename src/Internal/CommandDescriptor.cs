using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Ardalis.GuardClauses;

namespace Cackle.ConsoleApp.Internal;

/// <summary>
///     Identifies a command and it's command line argument specifications
/// </summary>
[DebuggerDisplay("CommandType = {CommandType}")]
public class CommandDescriptor
{
    private const DynamicallyAccessedMemberTypes AccessedMemberTypes =
        DynamicallyAccessedMemberTypes.PublicConstructors |
        DynamicallyAccessedMemberTypes.PublicMethods;

    /// <summary>
    ///     Initialize new instance of <see cref="CommandDescriptor" />
    /// </summary>
    /// <param name="commandType"><see cref="Type" /> of command</param>
    /// <param name="argumentType"><see cref="Type" /> of command line argument specification</param>
    /// <param name="isSynchronous">Identifies if the command should be executed synchronously or asynchronously</param>
    public CommandDescriptor([DynamicallyAccessedMembers(AccessedMemberTypes)] Type commandType, Type argumentType,
        bool isSynchronous)
    {
        Guard.Against.Null(commandType, nameof(commandType));
        Guard.Against.Null(argumentType, nameof(argumentType));

        CommandType = commandType;
        ArgumentType = argumentType;
        IsSynchronous = isSynchronous;
    }

    /// <summary>
    ///     The <see cref="Type" /> of the command
    /// </summary>
    public Type CommandType { get; }

    /// <summary>
    ///     The <see cref="Type" /> command line argument specifications for associated <see cref="CommandType" />
    /// </summary>
    public Type ArgumentType { get; }

    /// <summary>
    ///     Identifies if the command should be executed synchronously or asynchronously
    /// </summary>
    public bool IsSynchronous { get; }
}