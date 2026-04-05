#nullable enable
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;

namespace SevenZipMenu;

[GeneratedComClass]
internal abstract partial class ExplorerCommandBase : IExplorerCommand
{
    private static readonly StrategyBasedComWrappers s_comWrappers = new();

    protected virtual string? GetStaticTitle() => null;
    protected virtual string? GetDynamicTitle(IShellItemArray? psia) => null;
    protected virtual bool HasDynamicTitle => false;
    protected virtual string? GetIconResource() => null;
    protected virtual EXPCMDFLAGS GetCommandFlags() => EXPCMDFLAGS.ECF_DEFAULT;
    protected virtual EXPCMDSTATE GetCommandState(IShellItemArray? psia) => EXPCMDSTATE.ECS_ENABLED;
    protected virtual int OnInvoke(IShellItemArray? psia) => unchecked((int)0x80004001); // E_NOTIMPL

    private static IShellItemArray? WrapShellItemArray(nint ptr)
    {
        if (ptr == 0) return null;
        return (IShellItemArray)s_comWrappers.GetOrCreateObjectForComInstance(ptr, CreateObjectFlags.None);
    }

    public int GetTitle(nint psiItemArray, out nint ppszName)
    {
        try
        {
            var psia = WrapShellItemArray(psiItemArray);
            string? title = HasDynamicTitle ? GetDynamicTitle(psia) : GetStaticTitle();
            ppszName = Marshal.StringToCoTaskMemUni(title ?? string.Empty);
            return 0;
        }
        catch
        {
            ppszName = 0;
            return unchecked((int)0x80004005);
        }
    }

    public int GetIcon(nint psiItemArray, out nint ppszIcon)
    {
        try
        {
            string? icon = GetIconResource();
            if (icon != null)
            {
                ppszIcon = Marshal.StringToCoTaskMemUni(icon);
                return 0;
            }
            ppszIcon = 0;
            return unchecked((int)0x80004001);
        }
        catch
        {
            ppszIcon = 0;
            return unchecked((int)0x80004005);
        }
    }

    public int GetToolTip(nint psiItemArray, out nint ppszInfotip)
    {
        ppszInfotip = 0;
        return unchecked((int)0x80004001);
    }

    public virtual int GetCanonicalName(out Guid pguidCommandName)
    {
        pguidCommandName = Guid.Empty;
        return 0;
    }

    public int GetState(nint psiItemArray, bool fOkToBeSlow, out EXPCMDSTATE pCmdState)
    {
        try
        {
            var psia = WrapShellItemArray(psiItemArray);
            pCmdState = GetCommandState(psia);
            return 0;
        }
        catch
        {
            pCmdState = EXPCMDSTATE.ECS_HIDDEN;
            return unchecked((int)0x80004005);
        }
    }

    public int GetFlags(out EXPCMDFLAGS pFlags)
    {
        pFlags = GetCommandFlags();
        return 0;
    }

    public virtual int EnumSubCommands(out IEnumExplorerCommand? ppEnum)
    {
        ppEnum = null;
        return unchecked((int)0x80004001);
    }

    public int Invoke(nint psiItemArray, nint pbc)
    {
        try
        {
            var psia = WrapShellItemArray(psiItemArray);
            return OnInvoke(psia);
        }
        catch
        {
            return unchecked((int)0x80004005);
        }
    }

    // ---- Utility methods ----

    internal static string? GetFirstFilePath(IShellItemArray? psia)
    {
        if (psia is null) return null;
        psia.GetCount(out uint count);
        if (count == 0) return null;
        psia.GetItemAt(0, out IShellItem item);
        item.GetDisplayName(SIGDN.SIGDN_FILESYSPATH, out nint namePtr);
        string result = Marshal.PtrToStringUni(namePtr) ?? string.Empty;
        Marshal.FreeCoTaskMem(namePtr);
        return result;
    }

    internal static string? GetFirstDisplayName(IShellItemArray? psia)
    {
        if (psia is null) return null;
        psia.GetCount(out uint count);
        if (count == 0) return null;
        psia.GetItemAt(0, out IShellItem item);
        item.GetDisplayName(SIGDN.SIGDN_NORMALDISPLAY, out nint namePtr);
        string result = Marshal.PtrToStringUni(namePtr) ?? string.Empty;
        Marshal.FreeCoTaskMem(namePtr);
        return result;
    }

    internal static List<string> GetAllFilePaths(IShellItemArray? psia)
    {
        var paths = new List<string>();
        if (psia is null) return paths;
        psia.GetCount(out uint count);
        for (uint i = 0; i < count; i++)
        {
            psia.GetItemAt(i, out IShellItem item);
            item.GetDisplayName(SIGDN.SIGDN_FILESYSPATH, out nint namePtr);
            paths.Add(Marshal.PtrToStringUni(namePtr) ?? string.Empty);
            Marshal.FreeCoTaskMem(namePtr);
        }
        return paths;
    }

    internal static string StripArchiveExtensions(string filename)
    {
        string name = Path.GetFileNameWithoutExtension(filename);
        string ext = Path.GetExtension(name);
        if (ext.Equals(".tar", StringComparison.OrdinalIgnoreCase))
            name = Path.GetFileNameWithoutExtension(name);
        return name;
    }

    internal static string GetParentDirectory(string path) =>
        Path.GetDirectoryName(path) ?? string.Empty;

    internal static string QuotePath(string path) => "\"" + path + "\"";

    internal static int LaunchProcess(string exePath, string args, string? workingDir = null)
    {
        try
        {
            var psi = new ProcessStartInfo
            {
                FileName = exePath,
                Arguments = args,
                WorkingDirectory = workingDir ?? string.Empty,
                UseShellExecute = false,
            };
            Process.Start(psi);
            return 0;
        }
        catch
        {
            return unchecked((int)0x80004005);
        }
    }

    protected static string BuildFileArgs(IShellItemArray? psia)
    {
        var paths = GetAllFilePaths(psia);
        var sb = new System.Text.StringBuilder();
        foreach (var p in paths)
        {
            sb.Append('"');
            sb.Append(p);
            sb.Append("\" ");
        }
        return sb.ToString();
    }

    protected static string GetArchiveStem(IShellItemArray? psia)
    {
        string? path = GetFirstFilePath(psia);
        if (string.IsNullOrEmpty(path)) return "archive";
        return StripArchiveExtensions(Path.GetFileName(path));
    }

    protected static string GetWorkDir(IShellItemArray? psia)
    {
        string? path = GetFirstFilePath(psia);
        if (string.IsNullOrEmpty(path)) return string.Empty;
        return GetParentDirectory(path);
    }
}
