#include "SevenZipUtils.h"

namespace SevenZipUtils
{
    static std::wstring s_cachedInstallPath;
    static bool s_pathCached = false;

    static std::wstring ReadRegistryString(HKEY hRoot, LPCWSTR subKey, LPCWSTR valueName)
    {
        HKEY hKey = nullptr;
        if (RegOpenKeyExW(hRoot, subKey, 0, KEY_READ, &hKey) != ERROR_SUCCESS)
            return L"";

        WCHAR buffer[MAX_PATH] = {};
        DWORD size = sizeof(buffer);
        DWORD type = 0;
        LSTATUS status = RegQueryValueExW(hKey, valueName, nullptr, &type,
                                           reinterpret_cast<LPBYTE>(buffer), &size);
        RegCloseKey(hKey);

        if (status == ERROR_SUCCESS && (type == REG_SZ || type == REG_EXPAND_SZ))
            return std::wstring(buffer);
        return L"";
    }

    static bool FileExists(const std::wstring& path)
    {
        DWORD attrs = GetFileAttributesW(path.c_str());
        return (attrs != INVALID_FILE_ATTRIBUTES) && !(attrs & FILE_ATTRIBUTE_DIRECTORY);
    }

    std::wstring GetInstallPath()
    {
        if (s_pathCached)
            return s_cachedInstallPath;

        s_pathCached = true;

        // Try registry locations
        const LPCWSTR regPaths[] = {
            L"SOFTWARE\\7-Zip",
        };
        const LPCWSTR valueNames[] = { L"Path64", L"Path" };
        const HKEY roots[] = { HKEY_LOCAL_MACHINE, HKEY_CURRENT_USER };

        for (HKEY root : roots)
        {
            for (LPCWSTR regPath : regPaths)
            {
                for (LPCWSTR val : valueNames)
                {
                    std::wstring path = ReadRegistryString(root, regPath, val);
                    if (!path.empty())
                    {
                        // Ensure trailing backslash
                        if (path.back() != L'\\')
                            path += L'\\';

                        if (FileExists(path + L"7z.exe"))
                        {
                            s_cachedInstallPath = path;
                            return s_cachedInstallPath;
                        }
                    }
                }
            }
        }

        // Try well-known paths
        const LPCWSTR knownPaths[] = {
            L"C:\\Program Files\\7-Zip\\",
            L"C:\\Program Files (x86)\\7-Zip\\",
        };

        for (LPCWSTR p : knownPaths)
        {
            if (FileExists(std::wstring(p) + L"7z.exe"))
            {
                s_cachedInstallPath = p;
                return s_cachedInstallPath;
            }
        }

        return L"";
    }

    std::wstring Get7zExePath()
    {
        std::wstring dir = GetInstallPath();
        return dir.empty() ? L"" : dir + L"7z.exe";
    }

    std::wstring Get7zGUIPath()
    {
        std::wstring dir = GetInstallPath();
        return dir.empty() ? L"" : dir + L"7zG.exe";
    }

    std::wstring Get7zFMPath()
    {
        std::wstring dir = GetInstallPath();
        return dir.empty() ? L"" : dir + L"7zFM.exe";
    }

    bool IsArchiveExtension(const std::wstring& extension)
    {
        static const std::unordered_set<std::wstring> archiveExts = {
            L".7z", L".zip", L".rar", L".tar", L".gz", L".gzip", L".tgz",
            L".bz2", L".bzip2", L".tbz2", L".tbz", L".xz", L".txz",
            L".lzma", L".lz", L".zst", L".zstd",
            L".cab", L".iso", L".img",
            L".wim", L".swm", L".esd",
            L".arj", L".cpio", L".rpm", L".deb",
            L".lzh", L".lha",
            L".dmg", L".hfs",
            L".vhd", L".vhdx", L".vmdk", L".vdi",
            L".fat", L".ntfs", L".ext", L".ext2", L".ext3", L".ext4",
            L".squashfs", L".cramfs",
            L".z", L".taz",
            L".jar", L".war", L".ear",
            L".apk", L".aab",
            L".xpi", L".crx",
            L".nupkg",
            L".001",
            L".zipx",
        };

        std::wstring lower = extension;
        std::transform(lower.begin(), lower.end(), lower.begin(), ::towlower);
        return archiveExts.count(lower) > 0;
    }

    bool HasArchiveItem(IShellItemArray* psiItemArray)
    {
        if (!psiItemArray) return false;

        DWORD count = 0;
        psiItemArray->GetCount(&count);

        for (DWORD i = 0; i < count; i++)
        {
            ComPtr<IShellItem> item;
            if (SUCCEEDED(psiItemArray->GetItemAt(i, &item)))
            {
                PWSTR name = nullptr;
                if (SUCCEEDED(item->GetDisplayName(SIGDN_FILESYSPATH, &name)))
                {
                    std::wstring path(name);
                    CoTaskMemFree(name);

                    std::filesystem::path p(path);
                    if (IsArchiveExtension(p.extension().wstring()))
                        return true;
                }
            }
        }
        return false;
    }

    std::wstring GetIconString()
    {
        std::wstring dir = GetInstallPath();
        if (dir.empty()) return L"";
        return dir + L"7z.dll,0";
    }
}
