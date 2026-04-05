#pragma once
#include "framework.h"
#include "SevenZipUtils.h"

// Base class for all IExplorerCommand implementations.
// Subclasses override virtual methods to customize behavior.
class ExplorerCommandBase : public RuntimeClass<
    RuntimeClassFlags<ClassicCom>,
    IExplorerCommand>
{
public:
    // IExplorerCommand
    IFACEMETHODIMP GetTitle(IShellItemArray* psiItemArray, LPWSTR* ppszName) override;
    IFACEMETHODIMP GetIcon(IShellItemArray* psiItemArray, LPWSTR* ppszIcon) override;
    IFACEMETHODIMP GetToolTip(IShellItemArray* psiItemArray, LPWSTR* ppszInfotip) override;
    IFACEMETHODIMP GetCanonicalName(GUID* pguidCommandName) override;
    IFACEMETHODIMP GetState(IShellItemArray* psiItemArray, BOOL fOkToBeSlow, EXPCMDSTATE* pCmdState) override;
    IFACEMETHODIMP GetFlags(EXPCMDFLAGS* pFlags) override;
    IFACEMETHODIMP EnumSubCommands(IEnumExplorerCommand** ppEnum) override;
    IFACEMETHODIMP Invoke(IShellItemArray* psiItemArray, IBindCtx* pbc) override;

    // Utility methods (public so free helper functions can use them)
    static std::wstring GetFirstFilePath(IShellItemArray* psiItemArray);
    static std::wstring GetFirstDisplayName(IShellItemArray* psiItemArray);
    static std::vector<std::wstring> GetAllFilePaths(IShellItemArray* psiItemArray);
    static std::wstring StripArchiveExtensions(const std::wstring& filename);
    static std::wstring GetParentDirectory(const std::wstring& path);
    static std::wstring QuotePath(const std::wstring& path);

    // Launch a process and return immediately
    static HRESULT LaunchProcess(const std::wstring& exePath, const std::wstring& args,
                                  const std::wstring& workingDir = L"");

protected:
    // Override these in subclasses
    virtual std::wstring GetDynamicTitle(IShellItemArray* psiItemArray) { return L""; }
    virtual LPCWSTR GetStaticTitle() { return L""; }
    virtual LPCWSTR GetIconResource() { return nullptr; }
    virtual EXPCMDFLAGS GetCommandFlags() { return ECF_DEFAULT; }
    virtual EXPCMDSTATE GetCommandState(IShellItemArray* psiItemArray) { return ECS_ENABLED; }
    virtual bool HasDynamicTitle() { return false; }
    virtual HRESULT OnInvoke(IShellItemArray* psiItemArray) { return E_NOTIMPL; }
};
