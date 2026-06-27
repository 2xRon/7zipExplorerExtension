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
            return 0; // S_OK
        }
        catch (Exception ex)
        {
            return Marshal.GetHRForException(ex);
        }
    }

    // The module never unloads (see DllExports.DllCanUnloadNow), so there is no
    // lock count to maintain; just acknowledge the request.
    public int LockServer(bool fLock) => 0; // S_OK
}
