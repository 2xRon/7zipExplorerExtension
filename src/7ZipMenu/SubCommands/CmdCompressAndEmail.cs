#nullable enable
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
        string args = "a -seml " + BuildFileArgs(psia);
        return LaunchProcess(gui, args, GetWorkDir(psia));
    }
}
