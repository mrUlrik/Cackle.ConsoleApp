namespace Cackle.ConsoleApp.Internal;

/// <summary>
///     Provides a collection of commands
/// </summary>
public interface ICommandCollection : IList<CommandDescriptor>
{
    /// <summary>
    ///     Add a command and it's command line argument specifications
    /// </summary>
    /// <param name="command">A <see cref="Type" /> that inherits <see cref="ICommand{TArgs}" /></param>
    /// <param name="args">A <see cref="Type" /> that inherits <see cref="ICommandArgs" /></param>
    /// <param name="isSynchronous">Determines if command will be run synchronously or asynchronously</param>
    void Add(Type command, Type args, bool isSynchronous);
}