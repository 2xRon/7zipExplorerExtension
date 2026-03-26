#pragma once
#include "ExplorerCommand.h"
#include "CommandEnumerator.h"

// ---- Separator ----
class CmdSeparator : public ExplorerCommandBase
{
protected:
    EXPCMDFLAGS GetCommandFlags() override { return ECF_ISSEPARATOR; }
};

// ---- Archive-only commands (visible only for archive files) ----

class CmdOpenArchive : public ExplorerCommandBase
{
protected:
    LPCWSTR GetStaticTitle() override { return L"Open archive"; }
    LPCWSTR GetIconResource() override;
    EXPCMDSTATE GetCommandState(IShellItemArray* psia) override;
    HRESULT OnInvoke(IShellItemArray* psia) override;
};

class CmdExtractFiles : public ExplorerCommandBase
{
protected:
    LPCWSTR GetStaticTitle() override { return L"Extract files..."; }
    EXPCMDSTATE GetCommandState(IShellItemArray* psia) override;
    HRESULT OnInvoke(IShellItemArray* psia) override;
};

class CmdExtractHere : public ExplorerCommandBase
{
protected:
    LPCWSTR GetStaticTitle() override { return L"Extract Here"; }
    EXPCMDSTATE GetCommandState(IShellItemArray* psia) override;
    HRESULT OnInvoke(IShellItemArray* psia) override;
};

class CmdExtractToFolder : public ExplorerCommandBase
{
protected:
    bool HasDynamicTitle() override { return true; }
    std::wstring GetDynamicTitle(IShellItemArray* psia) override;
    EXPCMDSTATE GetCommandState(IShellItemArray* psia) override;
    HRESULT OnInvoke(IShellItemArray* psia) override;
};

class CmdTestArchive : public ExplorerCommandBase
{
protected:
    LPCWSTR GetStaticTitle() override { return L"Test archive"; }
    EXPCMDSTATE GetCommandState(IShellItemArray* psia) override;
    HRESULT OnInvoke(IShellItemArray* psia) override;
};

// ---- Always-visible commands ----

class CmdAddToArchive : public ExplorerCommandBase
{
protected:
    LPCWSTR GetStaticTitle() override { return L"Add to archive..."; }
    HRESULT OnInvoke(IShellItemArray* psia) override;
};

class CmdCompressAndEmail : public ExplorerCommandBase
{
protected:
    LPCWSTR GetStaticTitle() override { return L"Compress and email..."; }
    HRESULT OnInvoke(IShellItemArray* psia) override;
};

class CmdAddTo7z : public ExplorerCommandBase
{
protected:
    bool HasDynamicTitle() override { return true; }
    std::wstring GetDynamicTitle(IShellItemArray* psia) override;
    HRESULT OnInvoke(IShellItemArray* psia) override;
};

class CmdCompressTo7zEmail : public ExplorerCommandBase
{
protected:
    bool HasDynamicTitle() override { return true; }
    std::wstring GetDynamicTitle(IShellItemArray* psia) override;
    HRESULT OnInvoke(IShellItemArray* psia) override;
};

class CmdAddToZip : public ExplorerCommandBase
{
protected:
    bool HasDynamicTitle() override { return true; }
    std::wstring GetDynamicTitle(IShellItemArray* psia) override;
    HRESULT OnInvoke(IShellItemArray* psia) override;
};

class CmdCompressToZipEmail : public ExplorerCommandBase
{
protected:
    bool HasDynamicTitle() override { return true; }
    std::wstring GetDynamicTitle(IShellItemArray* psia) override;
    HRESULT OnInvoke(IShellItemArray* psia) override;
};

