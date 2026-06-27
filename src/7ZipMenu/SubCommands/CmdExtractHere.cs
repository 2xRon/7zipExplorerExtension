#nullable enable
using System.Collections.Generic;
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
        var args = new List<string> { "x", "-y", "-o" + GetWorkDir(psia) };
        args.AddRange(GetAllFilePaths(psia));
        return LaunchProcess(gui, args);
    }
}
