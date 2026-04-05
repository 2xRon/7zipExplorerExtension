#nullable enable
using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;
using System.Threading;

namespace SevenZipMenu;

internal static class DllExports
{
    private static int s_objectCount;
    private static readonly StrategyBasedComWrappers s_comWrappers = new();

    internal static void IncrementObjectCount() => Interlocked.Increment(ref s_objectCount);
    internal static void DecrementObjectCount() => Interlocked.Decrement(ref s_objectCount);

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

    [UnmanagedCallersOnly(EntryPoint = "DllCanUnloadNow")]
    public static int DllCanUnloadNow()
    {
        return s_objectCount == 0 ? 0 : 1; // S_OK : S_FALSE
    }
}
