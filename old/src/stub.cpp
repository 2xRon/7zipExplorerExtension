// Stub executable required by MSIX packaging schema.
// This is never launched — the extension is a COM DLL loaded by Explorer.
#include <windows.h>

int WINAPI wWinMain(HINSTANCE, HINSTANCE, LPWSTR, int)
{
    return 0;
}
