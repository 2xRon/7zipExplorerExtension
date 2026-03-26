#pragma once
#include "framework.h"

namespace SevenZipUtils
{
    // Find 7z.exe, 7zG.exe, 7zFM.exe paths
    std::wstring GetInstallPath();
    std::wstring Get7zExePath();
    std::wstring Get7zGUIPath();
    std::wstring Get7zFMPath();

    // Check if a file extension is a known archive format
    bool IsArchiveExtension(const std::wstring& extension);

    // Check if any item in the shell item array is an archive
    bool HasArchiveItem(IShellItemArray* psiItemArray);

    // Get the 7-Zip icon string (e.g., "C:\Program Files\7-Zip\7z.dll,0")
    std::wstring GetIconString();
}
