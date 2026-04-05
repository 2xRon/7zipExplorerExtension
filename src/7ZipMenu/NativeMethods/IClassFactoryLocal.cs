#nullable enable
using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;

namespace SevenZipMenu;

// CsWin32's generated IClassFactory is sealed and can't be implemented.
// Define our own [GeneratedComInterface] version.
[GeneratedComInterface]
[Guid("00000001-0000-0000-C000-000000000046")]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
internal partial interface IClassFactoryLocal
{
    [PreserveSig]
    unsafe int CreateInstance(nint pUnkOuter, Guid* riid, out nint ppvObject);

    [PreserveSig]
    int LockServer([MarshalAs(UnmanagedType.Bool)] bool fLock);
}
