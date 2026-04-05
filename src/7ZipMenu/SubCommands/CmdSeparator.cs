#nullable enable
using System.Runtime.InteropServices.Marshalling;

namespace SevenZipMenu.SubCommands;

[GeneratedComClass]
internal partial class CmdSeparator : ExplorerCommandBase
{
    protected override EXPCMDFLAGS GetCommandFlags() => EXPCMDFLAGS.ECF_ISSEPARATOR;
}
