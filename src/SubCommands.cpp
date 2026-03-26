#include "SubCommands.h"

// ---- Helper: build args string with all selected file paths ----
static std::wstring BuildFileArgs(IShellItemArray* psia)
{
    auto paths = ExplorerCommandBase::GetAllFilePaths(psia);
    std::wstring args;
    for (auto& p : paths)
    {
        args += L"\"" + p + L"\" ";
    }
    return args;
}

// Get the stem name for archive naming (from first selected item)
static std::wstring GetArchiveStem(IShellItemArray* psia)
{
    std::wstring path = ExplorerCommandBase::GetFirstFilePath(psia);
    if (path.empty()) return L"archive";
    return ExplorerCommandBase::StripArchiveExtensions(
        std::filesystem::path(path).filename().wstring());
}

// Get working directory (parent of first file)
static std::wstring GetWorkDir(IShellItemArray* psia)
{
    std::wstring path = ExplorerCommandBase::GetFirstFilePath(psia);
    if (path.empty()) return L"";
    return ExplorerCommandBase::GetParentDirectory(path);
}

// ============================================================
// Archive-only commands: GetState hides them for non-archives
// ============================================================

static EXPCMDSTATE ArchiveOnlyState(IShellItemArray* psia)
{
    return SevenZipUtils::HasArchiveItem(psia) ? ECS_ENABLED : ECS_HIDDEN;
}

// ---- CmdOpenArchive ----

LPCWSTR CmdOpenArchive::GetIconResource()
{
    static std::wstring icon = SevenZipUtils::GetIconString();
    return icon.empty() ? nullptr : icon.c_str();
}

EXPCMDSTATE CmdOpenArchive::GetCommandState(IShellItemArray* psia) { return ArchiveOnlyState(psia); }

HRESULT CmdOpenArchive::OnInvoke(IShellItemArray* psia)
{
    std::wstring fm = SevenZipUtils::Get7zFMPath();
    if (fm.empty()) return E_FAIL;

    auto paths = GetAllFilePaths(psia);
    for (auto& p : paths)
        LaunchProcess(fm, L"\"" + p + L"\"", GetParentDirectory(p));
    return S_OK;
}

// ---- CmdExtractFiles ----

EXPCMDSTATE CmdExtractFiles::GetCommandState(IShellItemArray* psia) { return ArchiveOnlyState(psia); }

HRESULT CmdExtractFiles::OnInvoke(IShellItemArray* psia)
{
    std::wstring gui = SevenZipUtils::Get7zGUIPath();
    if (gui.empty()) return E_FAIL;

    std::wstring args = L"x " + BuildFileArgs(psia);
    return LaunchProcess(gui, args, GetWorkDir(psia));
}

// ---- CmdExtractHere ----

EXPCMDSTATE CmdExtractHere::GetCommandState(IShellItemArray* psia) { return ArchiveOnlyState(psia); }

HRESULT CmdExtractHere::OnInvoke(IShellItemArray* psia)
{
    std::wstring gui = SevenZipUtils::Get7zGUIPath();
    if (gui.empty()) return E_FAIL;

    std::wstring workDir = GetWorkDir(psia);
    std::wstring args = L"x -y -o\"" + workDir + L"\" " + BuildFileArgs(psia);
    return LaunchProcess(gui, args, workDir);
}

// ---- CmdExtractToFolder ----

std::wstring CmdExtractToFolder::GetDynamicTitle(IShellItemArray* psia)
{
    std::wstring name = GetFirstDisplayName(psia);
    if (name.empty()) return L"Extract to folder\\";
    std::wstring stem = StripArchiveExtensions(name);
    return L"Extract to \"" + stem + L"\\\"";
}

EXPCMDSTATE CmdExtractToFolder::GetCommandState(IShellItemArray* psia) { return ArchiveOnlyState(psia); }

HRESULT CmdExtractToFolder::OnInvoke(IShellItemArray* psia)
{
    std::wstring gui = SevenZipUtils::Get7zGUIPath();
    if (gui.empty()) return E_FAIL;

    auto paths = GetAllFilePaths(psia);
    for (auto& p : paths)
    {
        std::wstring stem = StripArchiveExtensions(
            std::filesystem::path(p).filename().wstring());
        std::wstring parentDir = GetParentDirectory(p);
        std::wstring outDir = parentDir + L"\\" + stem;
        std::wstring args = L"x -y -o\"" + outDir + L"\" \"" + p + L"\"";
        LaunchProcess(gui, args, parentDir);
    }
    return S_OK;
}

// ---- CmdTestArchive ----

EXPCMDSTATE CmdTestArchive::GetCommandState(IShellItemArray* psia) { return ArchiveOnlyState(psia); }

HRESULT CmdTestArchive::OnInvoke(IShellItemArray* psia)
{
    std::wstring gui = SevenZipUtils::Get7zGUIPath();
    if (gui.empty()) return E_FAIL;

    std::wstring args = L"t " + BuildFileArgs(psia);
    return LaunchProcess(gui, args, GetWorkDir(psia));
}

// ============================================================
// Always-visible commands
// ============================================================

// ---- CmdAddToArchive ----

HRESULT CmdAddToArchive::OnInvoke(IShellItemArray* psia)
{
    std::wstring gui = SevenZipUtils::Get7zGUIPath();
    if (gui.empty()) return E_FAIL;

    std::wstring args = L"a " + BuildFileArgs(psia);
    return LaunchProcess(gui, args, GetWorkDir(psia));
}

// ---- CmdCompressAndEmail ----

HRESULT CmdCompressAndEmail::OnInvoke(IShellItemArray* psia)
{
    std::wstring gui = SevenZipUtils::Get7zGUIPath();
    if (gui.empty()) return E_FAIL;

    std::wstring args = L"a -seml " + BuildFileArgs(psia);
    return LaunchProcess(gui, args, GetWorkDir(psia));
}

// ---- CmdAddTo7z ----

std::wstring CmdAddTo7z::GetDynamicTitle(IShellItemArray* psia)
{
    return L"Add to \"" + GetArchiveStem(psia) + L".7z\"";
}

HRESULT CmdAddTo7z::OnInvoke(IShellItemArray* psia)
{
    std::wstring gui = SevenZipUtils::Get7zGUIPath();
    if (gui.empty()) return E_FAIL;

    std::wstring workDir = GetWorkDir(psia);
    std::wstring stem = GetArchiveStem(psia);
    std::wstring args = L"a -t7z \"" + workDir + L"\\" + stem + L".7z\" " + BuildFileArgs(psia);
    return LaunchProcess(gui, args, workDir);
}

// ---- CmdCompressTo7zEmail ----

std::wstring CmdCompressTo7zEmail::GetDynamicTitle(IShellItemArray* psia)
{
    return L"Compress to \"" + GetArchiveStem(psia) + L".7z\" and email";
}

HRESULT CmdCompressTo7zEmail::OnInvoke(IShellItemArray* psia)
{
    std::wstring gui = SevenZipUtils::Get7zGUIPath();
    if (gui.empty()) return E_FAIL;

    std::wstring workDir = GetWorkDir(psia);
    std::wstring stem = GetArchiveStem(psia);
    std::wstring args = L"a -t7z -seml \"" + workDir + L"\\" + stem + L".7z\" " + BuildFileArgs(psia);
    return LaunchProcess(gui, args, workDir);
}

// ---- CmdAddToZip ----

std::wstring CmdAddToZip::GetDynamicTitle(IShellItemArray* psia)
{
    return L"Add to \"" + GetArchiveStem(psia) + L".zip\"";
}

HRESULT CmdAddToZip::OnInvoke(IShellItemArray* psia)
{
    std::wstring gui = SevenZipUtils::Get7zGUIPath();
    if (gui.empty()) return E_FAIL;

    std::wstring workDir = GetWorkDir(psia);
    std::wstring stem = GetArchiveStem(psia);
    std::wstring args = L"a -tzip \"" + workDir + L"\\" + stem + L".zip\" " + BuildFileArgs(psia);
    return LaunchProcess(gui, args, workDir);
}

// ---- CmdCompressToZipEmail ----

std::wstring CmdCompressToZipEmail::GetDynamicTitle(IShellItemArray* psia)
{
    return L"Compress to \"" + GetArchiveStem(psia) + L".zip\" and email";
}

HRESULT CmdCompressToZipEmail::OnInvoke(IShellItemArray* psia)
{
    std::wstring gui = SevenZipUtils::Get7zGUIPath();
    if (gui.empty()) return E_FAIL;

    std::wstring workDir = GetWorkDir(psia);
    std::wstring stem = GetArchiveStem(psia);
    std::wstring args = L"a -tzip -seml \"" + workDir + L"\\" + stem + L".zip\" " + BuildFileArgs(psia);
    return LaunchProcess(gui, args, workDir);
}

