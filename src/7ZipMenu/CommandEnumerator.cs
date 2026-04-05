#nullable enable
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.Marshalling;

namespace SevenZipMenu;

[GeneratedComClass]
internal partial class CommandEnumerator : IEnumExplorerCommand
{
    private readonly List<IExplorerCommand> _commands = new();
    private int _current;

    public void AddCommand(IExplorerCommand command)
    {
        _commands.Add(command);
    }

    public int Next(uint celt, IExplorerCommand?[] pUICommand, out uint pceltFetched)
    {
        uint fetched = 0;

        for (uint i = 0; i < celt && _current < _commands.Count; i++, _current++)
        {
            pUICommand[i] = _commands[_current];
            fetched++;
        }

        pceltFetched = fetched;
        return fetched == celt ? 0 : 1; // S_OK : S_FALSE
    }

    public int Skip(uint celt)
    {
        _current = (int)Math.Min(_current + celt, (uint)_commands.Count);
        return _current < _commands.Count ? 0 : 1; // S_OK : S_FALSE
    }

    public int Reset()
    {
        _current = 0;
        return 0; // S_OK
    }

    public int Clone(out IEnumExplorerCommand? ppEnum)
    {
        var clone = new CommandEnumerator();
        foreach (var cmd in _commands)
            clone.AddCommand(cmd);
        clone._current = _current;
        ppEnum = clone;
        return 0; // S_OK
    }
}
