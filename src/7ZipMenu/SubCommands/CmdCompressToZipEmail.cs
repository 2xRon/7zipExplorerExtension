#nullable enable
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices.Marshalling;


namespace SevenZipMenu.SubCommands;

[GeneratedComClass]
internal partial class CmdCompressToZipEmail : ExplorerCommandBase
{
    protected override bool HasDynamicTitle => true;

    protected override string? GetDynamicTitle(IShellItemArray? psia) =>
        $"Compress to \"{GetArchiveStem(psia)}.zip\" and email";

    protected override int OnInvoke(IShellItemArray? psia)
    {
        string gui = SevenZipUtils.Get7zGUIPath();
        if (gui.Length == 0) return unchecked((int)0x80004005);
        string archive = Path.Combine(GetWorkDir(psia), GetArchiveStem(psia)) + ".zip";
        var args = new List<string> { "a", "-tzip", "-seml", archive };
        args.AddRange(GetAllFilePaths(psia));
        return LaunchProcess(gui, args);
    }
}
