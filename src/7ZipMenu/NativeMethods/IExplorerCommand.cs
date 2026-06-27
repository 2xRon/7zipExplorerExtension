#nullable enable
using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;

namespace SevenZipMenu;

// IShellItemArray is passed as raw nint (COM pointer) because the
// [GeneratedComInterface] source generator cannot marshal types from
// a different source generator (CsWin32). The ExplorerCommandBase
// wraps these pointers into managed IShellItemArray via ComWrappers.

[GeneratedComInterface]
[Guid("a08ce4d0-fa25-44ab-b57c-c7b1c323e0b9")]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
internal partial interface IExplorerCommand
{
    [PreserveSig]
    int GetTitle(nint psiItemArray, out nint ppszName);

    [PreserveSig]
    int GetIcon(nint psiItemArray, out nint ppszIcon);

    [PreserveSig]
    int GetToolTip(nint psiItemArray, out nint ppszInfotip);

    [PreserveSig]
    int GetCanonicalName(out Guid pguidCommandName);

    [PreserveSig]
    int GetState(
        nint psiItemArray,
        [MarshalAs(UnmanagedType.Bool)] bool fOkToBeSlow,
        out EXPCMDSTATE pCmdState);

    // NOTE: The vtable order below MUST match the Windows SDK IExplorerCommand
    // (shobjidl_core.h): GetState, Invoke, GetFlags, EnumSubCommands. Reordering
    // these (e.g. moving Invoke last) shifts the vtable slots and silently routes
    // Explorer's GetFlags/EnumSubCommands calls to the wrong methods.
    [PreserveSig]
    int Invoke(nint psiItemArray, nint pbc);

    [PreserveSig]
    int GetFlags(out EXPCMDFLAGS pFlags);

    [PreserveSig]
    int EnumSubCommands(out IEnumExplorerCommand? ppEnum);
}
