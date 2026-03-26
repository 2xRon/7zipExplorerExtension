#pragma once

#ifndef WIN32_LEAN_AND_MEAN
#define WIN32_LEAN_AND_MEAN
#endif
#include <windows.h>
#include <shobjidl_core.h>
#include <shlwapi.h>
#include <shlobj_core.h>
#include <strsafe.h>
#include <wrl/module.h>
#include <wrl/implements.h>
#include <wrl/client.h>

#include <string>
#include <vector>
#include <unordered_set>
#include <algorithm>
#include <filesystem>

#pragma comment(lib, "shlwapi.lib")
#pragma comment(lib, "shell32.lib")
#pragma comment(lib, "ole32.lib")
#pragma comment(lib, "propsys.lib")

using namespace Microsoft::WRL;
