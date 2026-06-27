#nullable enable
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

    public unsafe int Next(uint celt, IExplorerCommand?[] pUICommand, uint* pceltFetched)
    {
        uint fetched = 0;

        for (uint i = 0; i < celt && _current < _commands.Count; i++, _current++)
        {
            pUICommand[i] = _commands[_current];
            fetched++;
        }

        // pceltFetched may be NULL when celt == 1 (IEnumXxx::Next contract).
        if (pceltFetched != null)
            *pceltFetched = fetched;
        return fetched == celt ? 0 : 1; // S_OK : S_FALSE
    }

    public int Skip(uint celt)
    {
        // S_OK only if the full celt elements were skipped; S_FALSE if we ran out
        // first (IEnumXxx::Skip contract). Comparing against the remaining count
        // avoids conflating "skipped exactly to the end" with "skipped fewer".
        uint remaining = (uint)(_commands.Count - _current);
        if (celt <= remaining)
        {
            _current += (int)celt;
            return 0; // S_OK
        }

        _current = _commands.Count;
        return 1; // S_FALSE
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
