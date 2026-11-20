#nullable enable
using System.Collections.Generic;
using System.Runtime.InteropServices.Marshalling;


namespace SevenZipMenu.SubCommands;

[GeneratedComClass]
internal partial class CmdAddToZip : ExplorerCommandBase
{
    protected override bool HasDynamicTitle => true;

    protected override string? GetDynamicTitle(IShellItemArray? psia) =>
        $"Add to \"{GetArchiveStem(psia)}.zip\"";

    protected override int OnInvoke(IShellItemArray? psia)
    {
        string gui = SevenZipUtils.Get7zGUIPath();
        if (gui.Length == 0) return unchecked((int)0x80004005);
        var args = new List<string> { "a", "-tzip", GetTargetArchivePath(psia, ".zip") };
        args.AddRange(GetAllFilePaths(psia));
        return LaunchProcess(gui, args);
    }
}
