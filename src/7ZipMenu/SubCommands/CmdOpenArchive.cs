#nullable enable
using System.Runtime.InteropServices.Marshalling;


namespace SevenZipMenu.SubCommands;

[GeneratedComClass]
internal partial class CmdOpenArchive : ExplorerCommandBase
{
    protected override string? GetStaticTitle() => "Open archive";

    protected override string? GetIconResource()
    {
        string icon = SevenZipUtils.GetIconString();
        return icon.Length == 0 ? null : icon;
    }

    protected override EXPCMDSTATE GetCommandState(IShellItemArray? psia) =>
        SevenZipUtils.HasArchiveItem(psia) ? EXPCMDSTATE.ECS_ENABLED : EXPCMDSTATE.ECS_HIDDEN;

    protected override int OnInvoke(IShellItemArray? psia)
    {
        string fm = SevenZipUtils.Get7zFMPath();
        if (fm.Length == 0) return unchecked((int)0x80004005);

        var paths = GetAllFilePaths(psia);
        foreach (var p in paths)
            LaunchProcess(fm, QuotePath(p), GetParentDirectory(p));
        return 0;
    }
}
