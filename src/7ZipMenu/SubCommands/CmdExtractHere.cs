#nullable enable
using System.Runtime.InteropServices.Marshalling;


namespace SevenZipMenu.SubCommands;

[GeneratedComClass]
internal partial class CmdExtractHere : ExplorerCommandBase
{
    protected override string? GetStaticTitle() => "Extract Here";

    protected override EXPCMDSTATE GetCommandState(IShellItemArray? psia) =>
        SevenZipUtils.HasArchiveItem(psia) ? EXPCMDSTATE.ECS_ENABLED : EXPCMDSTATE.ECS_HIDDEN;

    protected override int OnInvoke(IShellItemArray? psia)
    {
        string gui = SevenZipUtils.Get7zGUIPath();
        if (gui.Length == 0) return unchecked((int)0x80004005);
        string workDir = GetWorkDir(psia);
        string args = $"x -y -o\"{workDir}\" " + BuildFileArgs(psia);
        return LaunchProcess(gui, args, workDir);
    }
}
