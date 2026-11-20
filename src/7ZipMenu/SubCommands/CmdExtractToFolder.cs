#nullable enable
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices.Marshalling;


namespace SevenZipMenu.SubCommands;

[GeneratedComClass]
internal partial class CmdExtractToFolder : ExplorerCommandBase
{
    protected override bool HasDynamicTitle => true;

    protected override string? GetDynamicTitle(IShellItemArray? psia)
    {
        List<string> archives = GetArchivePaths(psia);
        if (archives.Count == 0) return "Extract to folder\\";
        return $"Extract to \"{GetExtractFolderName(archives)}\\\"";
    }

    protected override EXPCMDSTATE GetCommandState(IShellItemArray? psia) =>
        SevenZipUtils.HasArchiveItem(psia) ? EXPCMDSTATE.ECS_ENABLED : EXPCMDSTATE.ECS_HIDDEN;

    protected override int OnInvoke(IShellItemArray? psia)
    {
        string gui = SevenZipUtils.Get7zGUIPath();
        if (gui.Length == 0) return unchecked((int)0x80004005);

        foreach (string archive in GetArchivePaths(psia))
        {
            string stem = StripArchiveExtensions(Path.GetFileName(archive));
            string outDir = Path.Combine(GetParentDirectory(archive), stem);
            LaunchProcess(gui, new List<string> { "x", "-o" + outDir, archive });
        }
        return 0;
    }
}
