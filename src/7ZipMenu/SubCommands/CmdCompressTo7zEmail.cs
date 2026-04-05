#nullable enable
using System.IO;
using System.Runtime.InteropServices.Marshalling;


namespace SevenZipMenu.SubCommands;

[GeneratedComClass]
internal partial class CmdCompressTo7zEmail : ExplorerCommandBase
{
    protected override bool HasDynamicTitle => true;

    protected override string? GetDynamicTitle(IShellItemArray? psia) =>
        $"Compress to \"{GetArchiveStem(psia)}.7z\" and email";

    protected override int OnInvoke(IShellItemArray? psia)
    {
        string gui = SevenZipUtils.Get7zGUIPath();
        if (gui.Length == 0) return unchecked((int)0x80004005);
        string workDir = GetWorkDir(psia);
        string stem = GetArchiveStem(psia);
        string args = $"a -t7z -seml \"{Path.Combine(workDir, stem)}.7z\" " + BuildFileArgs(psia);
        return LaunchProcess(gui, args, workDir);
    }
}
