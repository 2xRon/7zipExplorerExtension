#nullable enable
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices.Marshalling;


namespace SevenZipMenu.SubCommands;

[GeneratedComClass]
internal partial class CmdExtractFiles : ExplorerCommandBase
{
    protected override string? GetStaticTitle() => "Extract files...";

    protected override EXPCMDSTATE GetCommandState(IShellItemArray? psia) =>
        SevenZipUtils.HasArchiveItem(psia) ? EXPCMDSTATE.ECS_ENABLED : EXPCMDSTATE.ECS_HIDDEN;

    protected override int OnInvoke(IShellItemArray? psia)
    {
        string gui = SevenZipUtils.Get7zGUIPath();
        if (gui.Length == 0) return unchecked((int)0x80004005);

        foreach (List<string> archives in GroupByParent(GetArchivePaths(psia)))
        {
            // Without -ad 7zG extracts straight into its working directory, which is
            // the 7-Zip install folder; the dialog lets the user pick the target.
            string outDir = Path.Combine(
                GetParentDirectory(archives[0]),
                GetExtractFolderName(archives));
            var args = new List<string> { "x", "-ad", "-o" + outDir };
            AddArchiveSelection(args, archives);
            LaunchProcess(gui, args);
        }
        return 0;
    }
}
