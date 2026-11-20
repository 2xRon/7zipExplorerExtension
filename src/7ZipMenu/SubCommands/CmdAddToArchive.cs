#nullable enable
using System.Collections.Generic;
using System.Runtime.InteropServices.Marshalling;


namespace SevenZipMenu.SubCommands;

[GeneratedComClass]
internal partial class CmdAddToArchive : ExplorerCommandBase
{
    protected override string? GetStaticTitle() => "Add to archive...";

    protected override int OnInvoke(IShellItemArray? psia)
    {
        string gui = SevenZipUtils.Get7zGUIPath();
        if (gui.Length == 0) return unchecked((int)0x80004005);
        // 7-Zip takes the first file name after the command as the archive, so a
        // default archive name must come before the selected items.
        var args = new List<string> { "a", "-ad", GetTargetArchivePath(psia, ".7z") };
        args.AddRange(GetAllFilePaths(psia));
        return LaunchProcess(gui, args);
    }
}
