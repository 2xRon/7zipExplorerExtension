#include "CommandEnumerator.h"

IFACEMETHODIMP CommandEnumerator::Next(ULONG celt, IExplorerCommand** pUICommand, ULONG* pceltFetched)
{
    ULONG fetched = 0;

    for (ULONG i = 0; i < celt && m_current < m_commands.size(); i++, m_current++)
    {
        m_commands[m_current].CopyTo(&pUICommand[i]);
        fetched++;
    }

    if (pceltFetched)
        *pceltFetched = fetched;

    return (fetched == celt) ? S_OK : S_FALSE;
}

IFACEMETHODIMP CommandEnumerator::Skip(ULONG celt)
{
    m_current = min(m_current + celt, static_cast<ULONG>(m_commands.size()));
    return (m_current < m_commands.size()) ? S_OK : S_FALSE;
}

IFACEMETHODIMP CommandEnumerator::Reset()
{
    m_current = 0;
    return S_OK;
}

IFACEMETHODIMP CommandEnumerator::Clone(IEnumExplorerCommand** ppEnum)
{
    *ppEnum = nullptr;
    auto clone = Make<CommandEnumerator>();
    for (auto& cmd : m_commands)
        clone->AddCommand(cmd);
    clone->m_current = m_current;
    return clone.CopyTo(ppEnum);
}
