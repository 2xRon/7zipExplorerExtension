#nullable enable
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

        var paths = GetAllFilePaths(psia);
        foreach (var p in paths)
        {
            string stem = StripArchiveExtensions(Path.GetFileName(p));
            string parentDir = GetParentDirectory(p);
            string outDir = Path.Combine(parentDir, stem);
            string args = $"x -y -o\"{outDir}\" \"{p}\"";
            LaunchProcess(gui, args, parentDir);
        }
        return 0;
    }
}
