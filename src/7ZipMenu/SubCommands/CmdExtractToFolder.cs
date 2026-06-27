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
        string? name = GetFirstDisplayName(psia);
        if (string.IsNullOrEmpty(name)) return "Extract to folder\\";
        string stem = StripArchiveExtensions(name);
        return $"Extract to \"{stem}\\\"";
    }

    protected override EXPCMDSTATE GetCommandState(IShellItemArray? psia) =>
        SevenZipUtils.HasArchiveItem(psia) ? EXPCMDSTATE.ECS_ENABLED : EXPCMDSTATE.ECS_HIDDEN;

    protected override int OnInvoke(IShellItemArray? psia)
    {
        string gui = SevenZipUtils.Get7zGUIPath();
        if (gui.Length == 0) return unchecked((int)0x80004005);

        foreach (var p in GetAllFilePaths(psia))
        {
            string stem = StripArchiveExtensions(Path.GetFileName(p));
            string outDir = Path.Combine(GetParentDirectory(p), stem);
            LaunchProcess(gui, new List<string> { "x", "-y", "-o" + outDir, p });
        }
        return 0;
    }
}
