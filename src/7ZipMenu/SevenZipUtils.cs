#nullable enable
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.Win32;

namespace SevenZipMenu;

internal static class SevenZipUtils
{
    private static string? s_cachedInstallPath;
    private static bool s_pathCached;

    private static readonly HashSet<string> ArchiveExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".7z", ".zip", ".rar", ".tar", ".gz", ".gzip", ".tgz",
        ".bz2", ".bzip2", ".tbz2", ".tbz", ".xz", ".txz",
        ".lzma", ".lz", ".zst", ".zstd",
        ".cab", ".iso", ".img",
        ".wim", ".swm", ".esd",
        ".arj", ".cpio", ".rpm", ".deb",
        ".lzh", ".lha",
        ".dmg", ".hfs",
        ".vhd", ".vhdx", ".vmdk", ".vdi",
        ".fat", ".ntfs", ".ext", ".ext2", ".ext3", ".ext4",
        ".squashfs", ".cramfs",
        ".z", ".taz",
        ".jar", ".war", ".ear",
        ".apk", ".aab",
        ".xpi", ".crx",
        ".nupkg",
        ".001",
        ".zipx",
        ".xpi",
        ".docx", ".xlsx", ".pptx",
    };

    public static string GetInstallPath()
    {
        if (s_pathCached)
            return s_cachedInstallPath ?? string.Empty;

        s_pathCached = true;

        string[] valueNames = ["Path64", "Path"];
        RegistryKey[] roots = [Registry.LocalMachine, Registry.CurrentUser];

        foreach (var root in roots)
        {
            foreach (var valueName in valueNames)
            {
                string? path = ReadRegistryString(root, @"SOFTWARE\7-Zip", valueName);
                if (!string.IsNullOrEmpty(path))
                {
                    if (!path.EndsWith('\\'))
                        path += '\\';
                    if (File.Exists(path + "7z.exe"))
                    {
                        s_cachedInstallPath = path;
                        return s_cachedInstallPath;
                    }
                }
            }
        }

        string[] programFolders =
        [
            Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles),
            Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86),
        ];

        foreach (var folder in programFolders)
        {
            if (string.IsNullOrEmpty(folder)) continue;
            string candidate = Path.Combine(folder, "7-Zip") + "\\";
            if (File.Exists(candidate + "7z.exe"))
            {
                s_cachedInstallPath = candidate;
                return s_cachedInstallPath;
            }
        }

        return string.Empty;
    }

    public static string Get7zExePath()
    {
        string dir = GetInstallPath();
        return dir.Length == 0 ? string.Empty : dir + "7z.exe";
    }

    public static string Get7zGUIPath()
    {
        string dir = GetInstallPath();
        return dir.Length == 0 ? string.Empty : dir + "7zG.exe";
    }

    public static string Get7zFMPath()
    {
        string dir = GetInstallPath();
        return dir.Length == 0 ? string.Empty : dir + "7zFM.exe";
    }

    public static bool IsArchiveExtension(string extension) =>
        ArchiveExtensions.Contains(extension);

    public static bool HasArchiveItem(IShellItemArray? psia)
    {
        if (psia is null) return false;
        try
        {
            psia.GetCount(out uint count);
            for (uint i = 0; i < count; i++)
            {
                psia.GetItemAt(i, out IShellItem item);
                item.GetDisplayName(SIGDN.SIGDN_FILESYSPATH, out nint namePtr);
                string path = Marshal.PtrToStringUni(namePtr) ?? string.Empty;
                Marshal.FreeCoTaskMem(namePtr);

                string ext = Path.GetExtension(path);
                if (!string.IsNullOrEmpty(ext) && IsArchiveExtension(ext))
                    return true;
            }
        }
        catch { }
        return false;
    }

    public static string GetIconString()
    {
        string dir = GetInstallPath();
        return dir.Length == 0 ? string.Empty : dir + "7z.dll,0";
    }

    private static string? ReadRegistryString(RegistryKey root, string subKey, string valueName)
    {
        try
        {
            using var key = root.OpenSubKey(subKey);
            return key?.GetValue(valueName) as string;
        }
        catch { return null; }
    }
}
