#pragma once
#include "framework.h"

class CommandEnumerator : public RuntimeClass<
    RuntimeClassFlags<ClassicCom>,
    IEnumExplorerCommand>
{
public:
    CommandEnumerator() : m_current(0) {}

    void AddCommand(ComPtr<IExplorerCommand> command)
    {
        m_commands.push_back(std::move(command));
    }

    // IEnumExplorerCommand
    IFACEMETHODIMP Next(ULONG celt, IExplorerCommand** pUICommand, ULONG* pceltFetched) override;
    IFACEMETHODIMP Skip(ULONG celt) override;
    IFACEMETHODIMP Reset() override;
    IFACEMETHODIMP Clone(IEnumExplorerCommand** ppEnum) override;

private:
    std::vector<ComPtr<IExplorerCommand>> m_commands;
    ULONG m_current;
};
