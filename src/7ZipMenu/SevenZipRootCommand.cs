#nullable enable
using SevenZipMenu.SubCommands;
using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;

namespace SevenZipMenu;

[GeneratedComClass]
internal partial class SevenZipRootCommand : ExplorerCommandBase
{
    public static readonly Guid CLSID = new("E45E7D43-4F1C-4B5C-9A8D-6D3B7A9E1C2F");

    protected override string? GetStaticTitle() => "7-Zip";

    protected override string? GetIconResource()
    {
        string icon = SevenZipUtils.GetIconString();
        return icon.Length == 0 ? null : icon;
    }

    protected override EXPCMDFLAGS GetCommandFlags() => EXPCMDFLAGS.ECF_HASSUBCOMMANDS;

    public override int GetCanonicalName(out Guid pguidCommandName)
    {
        pguidCommandName = CLSID;
        return 0; // S_OK
    }

    public override int EnumSubCommands(out IEnumExplorerCommand? ppEnum)
    {
        var enumerator = new CommandEnumerator();

        // Group 1: Archive operations
        enumerator.AddCommand(new CmdOpenArchive());
        enumerator.AddCommand(new CmdSeparator());

        // Group 2: Extract operations
        enumerator.AddCommand(new CmdExtractFiles());
        enumerator.AddCommand(new CmdExtractHere());
        enumerator.AddCommand(new CmdExtractToFolder());
        enumerator.AddCommand(new CmdTestArchive());
        enumerator.AddCommand(new CmdSeparator());

        //// Group 3: Compress operations
        enumerator.AddCommand(new CmdAddToArchive());
        enumerator.AddCommand(new CmdCompressAndEmail());
        enumerator.AddCommand(new CmdAddTo7z());
        enumerator.AddCommand(new CmdCompressTo7zEmail());
        enumerator.AddCommand(new CmdAddToZip());
        enumerator.AddCommand(new CmdCompressToZipEmail());

        ppEnum = enumerator;

        return 0; // S_OK
    }
}
