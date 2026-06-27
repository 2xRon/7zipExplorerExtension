#nullable enable
using System.Collections.Generic;
using System.Runtime.InteropServices.Marshalling;


namespace SevenZipMenu.SubCommands;

[GeneratedComClass]
internal partial class CmdCompressAndEmail : ExplorerCommandBase
{
    protected override string? GetStaticTitle() => "Compress and email...";

    protected override int OnInvoke(IShellItemArray? psia)
    {
        string gui = SevenZipUtils.Get7zGUIPath();
        if (gui.Length == 0) return unchecked((int)0x80004005);
        var args = new List<string> { "a", "-seml" };
        args.AddRange(GetAllFilePaths(psia));
        return LaunchProcess(gui, args);
    }
}
