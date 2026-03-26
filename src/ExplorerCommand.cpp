#include "ExplorerCommand.h"

// ---- IExplorerCommand default implementations ----

IFACEMETHODIMP ExplorerCommandBase::GetTitle(IShellItemArray* psiItemArray, LPWSTR* ppszName)
{
    if (HasDynamicTitle())
    {
        std::wstring title = GetDynamicTitle(psiItemArray);
        return SHStrDupW(title.c_str(), ppszName);
    }
    return SHStrDupW(GetStaticTitle(), ppszName);
}

IFACEMETHODIMP ExplorerCommandBase::GetIcon(IShellItemArray* psiItemArray, LPWSTR* ppszIcon)
{
    LPCWSTR icon = GetIconResource();
    if (icon)
        return SHStrDupW(icon, ppszIcon);
    *ppszIcon = nullptr;
    return E_NOTIMPL;
}

IFACEMETHODIMP ExplorerCommandBase::GetToolTip(IShellItemArray*, LPWSTR* ppszInfotip)
{
    *ppszInfotip = nullptr;
    return E_NOTIMPL;
}

IFACEMETHODIMP ExplorerCommandBase::GetCanonicalName(GUID* pguidCommandName)
{
    *pguidCommandName = GUID_NULL;
    return S_OK;
}

IFACEMETHODIMP ExplorerCommandBase::GetState(IShellItemArray* psiItemArray, BOOL, EXPCMDSTATE* pCmdState)
{
    *pCmdState = GetCommandState(psiItemArray);
    return S_OK;
}

IFACEMETHODIMP ExplorerCommandBase::GetFlags(EXPCMDFLAGS* pFlags)
{
    *pFlags = GetCommandFlags();
    return S_OK;
}

IFACEMETHODIMP ExplorerCommandBase::EnumSubCommands(IEnumExplorerCommand** ppEnum)
{
    *ppEnum = nullptr;
    return E_NOTIMPL;
}

IFACEMETHODIMP ExplorerCommandBase::Invoke(IShellItemArray* psiItemArray, IBindCtx*)
{
    return OnInvoke(psiItemArray);
}

// ---- Utility methods ----

std::wstring ExplorerCommandBase::GetFirstFilePath(IShellItemArray* psiItemArray)
{
    if (!psiItemArray) return L"";

    DWORD count = 0;
    psiItemArray->GetCount(&count);
    if (count == 0) return L"";

    ComPtr<IShellItem> item;
    if (FAILED(psiItemArray->GetItemAt(0, &item))) return L"";

    PWSTR path = nullptr;
    if (FAILED(item->GetDisplayName(SIGDN_FILESYSPATH, &path))) return L"";

    std::wstring result(path);
    CoTaskMemFree(path);
    return result;
}

std::wstring ExplorerCommandBase::GetFirstDisplayName(IShellItemArray* psiItemArray)
{
    if (!psiItemArray) return L"";

    DWORD count = 0;
    psiItemArray->GetCount(&count);
    if (count == 0) return L"";

    ComPtr<IShellItem> item;
    if (FAILED(psiItemArray->GetItemAt(0, &item))) return L"";

    PWSTR name = nullptr;
    if (FAILED(item->GetDisplayName(SIGDN_NORMALDISPLAY, &name))) return L"";

    std::wstring result(name);
    CoTaskMemFree(name);
    return result;
}

std::vector<std::wstring> ExplorerCommandBase::GetAllFilePaths(IShellItemArray* psiItemArray)
{
    std::vector<std::wstring> paths;
    if (!psiItemArray) return paths;

    DWORD count = 0;
    psiItemArray->GetCount(&count);

    for (DWORD i = 0; i < count; i++)
    {
        ComPtr<IShellItem> item;
        if (SUCCEEDED(psiItemArray->GetItemAt(i, &item)))
        {
            PWSTR path = nullptr;
            if (SUCCEEDED(item->GetDisplayName(SIGDN_FILESYSPATH, &path)))
            {
                paths.emplace_back(path);
                CoTaskMemFree(path);
            }
        }
    }
    return paths;
}

std::wstring ExplorerCommandBase::StripArchiveExtensions(const std::wstring& filename)
{
    namespace fs = std::filesystem;
    std::wstring name = fs::path(filename).stem().wstring();

    // Handle compound extensions like .tar.gz, .tar.bz2, .tar.xz
    std::wstring lower = name;
    std::transform(lower.begin(), lower.end(), lower.begin(), ::towlower);

    if (lower.size() > 4)
    {
        std::wstring ext = fs::path(name).extension().wstring();
        std::transform(ext.begin(), ext.end(), ext.begin(), ::towlower);
        if (ext == L".tar")
        {
            name = fs::path(name).stem().wstring();
        }
    }
    return name;
}

std::wstring ExplorerCommandBase::GetParentDirectory(const std::wstring& path)
{
    return std::filesystem::path(path).parent_path().wstring();
}

std::wstring ExplorerCommandBase::QuotePath(const std::wstring& path)
{
    return L"\"" + path + L"\"";
}

HRESULT ExplorerCommandBase::LaunchProcess(const std::wstring& exePath, const std::wstring& args,
                                            const std::wstring& workingDir)
{
    std::wstring cmdLine = L"\"" + exePath + L"\" " + args;

    STARTUPINFOW si = { sizeof(si) };
    PROCESS_INFORMATION pi = {};

    BOOL ok = CreateProcessW(
        nullptr,
        cmdLine.data(),
        nullptr, nullptr, FALSE,
        0, nullptr,
        workingDir.empty() ? nullptr : workingDir.c_str(),
        &si, &pi);

    if (!ok)
        return HRESULT_FROM_WIN32(GetLastError());

    CloseHandle(pi.hProcess);
    CloseHandle(pi.hThread);
    return S_OK;
}
