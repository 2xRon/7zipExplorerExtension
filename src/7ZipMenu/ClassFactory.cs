#nullable enable
using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;

namespace SevenZipMenu;

[GeneratedComClass]
internal partial class SevenZipClassFactory : IClassFactoryLocal
{
    private static readonly StrategyBasedComWrappers s_comWrappers = new();

    public unsafe int CreateInstance(nint pUnkOuter, Guid* riid, out nint ppvObject)
    {
        ppvObject = 0;

        if (pUnkOuter != 0)
            return unchecked((int)0x80040110); // CLASS_E_NOAGGREGATION

        try
        {
            var command = new SevenZipRootCommand();
            nint comPtr = s_comWrappers.GetOrCreateComInterfaceForObject(command, CreateComInterfaceFlags.None);

            Guid iid = *riid;
            int hr = Marshal.QueryInterface(comPtr, in iid, out nint result);
            Marshal.Release(comPtr);

            if (hr < 0)
                return hr;

            ppvObject = result;
            DllExports.IncrementObjectCount();
            return 0; // S_OK
        }
        catch (Exception ex)
        {
            return Marshal.GetHRForException(ex);
        }
    }

    public int LockServer(bool fLock)
    {
        if (fLock)
            DllExports.IncrementObjectCount();
        else
            DllExports.DecrementObjectCount();
        return 0; // S_OK
    }
}
