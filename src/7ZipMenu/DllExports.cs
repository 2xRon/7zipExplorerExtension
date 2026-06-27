#nullable enable
using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;

namespace SevenZipMenu;

internal static class DllExports
{
    private static readonly StrategyBasedComWrappers s_comWrappers = new();

    [UnmanagedCallersOnly(EntryPoint = "DllGetClassObject")]
    public static unsafe int DllGetClassObject(Guid* rclsid, Guid* riid, nint* ppv)
    {
        *ppv = 0;

        if (*rclsid != SevenZipRootCommand.CLSID)
            return unchecked((int)0x80040111); // CLASS_E_CLASSNOTAVAILABLE

        try
        {
            var factory = new SevenZipClassFactory();
            nint comPtr = s_comWrappers.GetOrCreateComInterfaceForObject(factory, CreateComInterfaceFlags.None);

            Guid iid = *riid;
            int hr = Marshal.QueryInterface(comPtr, in iid, out nint result);
            Marshal.Release(comPtr);

            if (hr < 0)
                return hr;

            *ppv = result;
            return 0; // S_OK
        }
        catch (Exception ex)
        {
            return Marshal.GetHRForException(ex);
        }
    }

    // This is a Native AOT in-process server: the .NET runtime is statically
    // linked into this DLL and does not support being torn down (its GC and
    // finalizer threads keep running). Letting the host FreeLibrary us would
    // unmap a live runtime and crash, so we always report S_FALSE ("do not
    // unload"). The module stays resident for the host process's lifetime,
    // which is the standard, safe policy for an in-proc .NET COM server.
    [UnmanagedCallersOnly(EntryPoint = "DllCanUnloadNow")]
    public static int DllCanUnloadNow() => 1; // S_FALSE - never unload
}
