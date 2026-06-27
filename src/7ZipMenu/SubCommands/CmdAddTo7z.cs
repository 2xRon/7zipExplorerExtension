#nullable enable
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices.Marshalling;


namespace SevenZipMenu.SubCommands;

[GeneratedComClass]
internal partial class CmdAddTo7z : ExplorerCommandBase
{
    protected override bool HasDynamicTitle => true;

    protected override string? GetDynamicTitle(IShellItemArray? psia) =>
        $"Add to \"{GetArchiveStem(psia)}.7z\"";

    protected override int OnInvoke(IShellItemArray? psia)
    {
        string gui = SevenZipUtils.Get7zGUIPath();
        if (gui.Length == 0) return unchecked((int)0x80004005);
        string archive = Path.Combine(GetWorkDir(psia), GetArchiveStem(psia)) + ".7z";
        var args = new List<string> { "a", "-t7z", archive };
        args.AddRange(GetAllFilePaths(psia));
        return LaunchProcess(gui, args);
    }
}
