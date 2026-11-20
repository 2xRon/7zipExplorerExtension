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

        foreach (List<string> archives in GroupByParent(GetArchivePaths(psia)))
        {
            var args = new List<string> { "x", "-o" + GetParentDirectory(archives[0]) };
            AddArchiveSelection(args, archives);
            LaunchProcess(gui, args);
        }
        return 0;
    }
}
