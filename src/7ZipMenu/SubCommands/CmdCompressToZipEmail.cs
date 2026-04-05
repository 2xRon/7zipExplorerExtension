#nullable enable
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
        string workDir = GetWorkDir(psia);
        string stem = GetArchiveStem(psia);
        string args = $"a -tzip -seml \"{Path.Combine(workDir, stem)}.zip\" " + BuildFileArgs(psia);
        return LaunchProcess(gui, args, workDir);
    }
}
