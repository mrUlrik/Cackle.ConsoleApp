using System.Collections;
using System.Data;

namespace Cackle.ConsoleApp.Internal;

/// <summary>
///     Collection of command descriptors
/// </summary>
public class CommandCollection : ICommandCollection
{
    /// <summary>
    ///     Maximum number of commands allowed in the collection
    /// </summary>
    private const short MaxSize = 16;

    private readonly List<CommandDescriptor> _commands = new();

    /// <inheritdoc />
    public IEnumerator<CommandDescriptor> GetEnumerator()
    {
        return _commands.GetEnumerator();
    }

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    /// <inheritdoc />
    public void Add(Type command, Type args, bool isSynchronous)
    {
        _commands.Add(new CommandDescriptor(command, args, isSynchronous));
    }

    /// <inheritdoc />
    public void Add(CommandDescriptor item)
    {
        CheckSize();
        CheckReadOnly();
        _commands.Add(item);
    }

    /// <inheritdoc />
    public void Clear()
    {
        CheckReadOnly();
        _commands.Clear();
    }

    /// <inheritdoc />
    public bool Contains(CommandDescriptor item)
    {
        return _commands.Contains(item);
    }

    /// <inheritdoc />
    public void CopyTo(CommandDescriptor[] array, int arrayIndex)
    {
        _commands.CopyTo(array, arrayIndex);
    }

    /// <inheritdoc />
    public bool Remove(CommandDescriptor item)
    {
        CheckReadOnly();
        return _commands.Remove(item);
    }

    /// <inheritdoc />
    public int Count => _commands.Count;

    /// <inheritdoc />
    public bool IsReadOnly { get; private set; }

    /// <inheritdoc />
    public int IndexOf(CommandDescriptor item)
    {
        return _commands.IndexOf(item);
    }

    /// <inheritdoc />
    public void Insert(int index, CommandDescriptor item)
    {
        CheckSize();
        CheckReadOnly();
        _commands.Insert(index, item);
    }

    /// <inheritdoc />
    public void RemoveAt(int index)
    {
        CheckReadOnly();
        _commands.RemoveAt(index);
    }

    /// <inheritdoc />
    public CommandDescriptor this[int index]
    {
        get => _commands[index];
        set
        {
            if (index > MaxSize) ThrowSizeException();
            CheckReadOnly();
            _commands[index] = value;
        }
    }

    /// <summary>
    ///     Makes this collection read-only
    /// </summary>
    public void MakeReadOnly()
    {
        IsReadOnly = true;
    }

    private void CheckSize()
    {
        if (Count >= MaxSize) ThrowSizeException();
    }

    private void CheckReadOnly()
    {
        if (IsReadOnly) ThrowReadOnlyException();
    }

    private static void ThrowReadOnlyException()
    {
        throw new ReadOnlyException();
    }

    private static void ThrowSizeException()
    {
        throw new IndexOutOfRangeException();
    }
}