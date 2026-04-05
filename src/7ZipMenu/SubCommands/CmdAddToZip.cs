#nullable enable
using System.IO;
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
        string workDir = GetWorkDir(psia);
        string stem = GetArchiveStem(psia);
        string args = $"a -tzip \"{Path.Combine(workDir, stem)}.zip\" " + BuildFileArgs(psia);
        return LaunchProcess(gui, args, workDir);
    }
}
