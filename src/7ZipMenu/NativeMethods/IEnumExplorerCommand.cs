#nullable enable
using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;

namespace SevenZipMenu;

[GeneratedComInterface]
[Guid("a88826f8-186f-4987-aade-ea0cef8fbfe8")]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
internal partial interface IEnumExplorerCommand
{
    // pceltFetched is a raw uint* (not `out uint`) because the IEnumXxx::Next
    // contract permits callers to pass NULL when celt == 1. An `out uint` makes
    // the generated stub dereference unconditionally, which would access-violate
    // on a NULL pointer; the implementation null-checks before writing.
    [PreserveSig]
    unsafe int Next(
        uint celt,
        [MarshalUsing(CountElementName = nameof(celt))][Out]
        IExplorerCommand?[] pUICommand,
        uint* pceltFetched);

    [PreserveSig]
    int Skip(uint celt);

    [PreserveSig]
    int Reset();

    [PreserveSig]
    int Clone(out IEnumExplorerCommand? ppEnum);
}
