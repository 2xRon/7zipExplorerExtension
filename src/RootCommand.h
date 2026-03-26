#pragma once
#include "ExplorerCommand.h"

// {E45E7D43-4F1C-4B5C-9A8D-6D3B7A9E1C2F}
// Root CLSID — registered in the sparse package manifest
static const CLSID CLSID_SevenZipRootCommand =
    { 0xe45e7d43, 0x4f1c, 0x4b5c, { 0x9a, 0x8d, 0x6d, 0x3b, 0x7a, 0x9e, 0x1c, 0x2f } };

class __declspec(uuid("E45E7D43-4F1C-4B5C-9A8D-6D3B7A9E1C2F")) SevenZipRootCommand : public ExplorerCommandBase
{
public:
    // IExplorerCommand overrides
    IFACEMETHODIMP GetCanonicalName(GUID* pguidCommandName) override
    {
        *pguidCommandName = CLSID_SevenZipRootCommand;
        return S_OK;
    }

    IFACEMETHODIMP EnumSubCommands(IEnumExplorerCommand** ppEnum) override;

protected:
    LPCWSTR GetStaticTitle() override { return L"7-Zip"; }
    LPCWSTR GetIconResource() override;
    EXPCMDFLAGS GetCommandFlags() override { return ECF_HASSUBCOMMANDS; }
};
