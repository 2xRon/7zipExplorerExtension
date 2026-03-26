#include "RootCommand.h"
#include "SubCommands.h"
#include "CommandEnumerator.h"

LPCWSTR SevenZipRootCommand::GetIconResource()
{
    static std::wstring icon = SevenZipUtils::GetIconString();
    return icon.empty() ? nullptr : icon.c_str();
}

IFACEMETHODIMP SevenZipRootCommand::EnumSubCommands(IEnumExplorerCommand** ppEnum)
{
    auto enumerator = Make<CommandEnumerator>();

    // Group 1: Archive operations
    enumerator->AddCommand(Make<CmdOpenArchive>());
    enumerator->AddCommand(Make<CmdSeparator>());

    // Group 2: Extract operations
    enumerator->AddCommand(Make<CmdExtractFiles>());
    enumerator->AddCommand(Make<CmdExtractHere>());
    enumerator->AddCommand(Make<CmdExtractToFolder>());
    enumerator->AddCommand(Make<CmdTestArchive>());
    enumerator->AddCommand(Make<CmdSeparator>());

    // Group 3: Compress operations
    enumerator->AddCommand(Make<CmdAddToArchive>());
    enumerator->AddCommand(Make<CmdCompressAndEmail>());
    enumerator->AddCommand(Make<CmdAddTo7z>());
    enumerator->AddCommand(Make<CmdCompressTo7zEmail>());
    enumerator->AddCommand(Make<CmdAddToZip>());
    enumerator->AddCommand(Make<CmdCompressToZipEmail>());

    return enumerator.CopyTo(ppEnum);
}

// Register the class factory with WRL
CoCreatableClass(SevenZipRootCommand);
