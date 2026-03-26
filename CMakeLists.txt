cmake_minimum_required(VERSION 3.20)
project(7ZipExplorerExtension LANGUAGES CXX)

set(CMAKE_CXX_STANDARD 17)
set(CMAKE_CXX_STANDARD_REQUIRED ON)

add_library(7ZipMenu SHARED
    src/dllmain.cpp
    src/ExplorerCommand.cpp
    src/RootCommand.cpp
    src/SubCommands.cpp
    src/CommandEnumerator.cpp
    src/SevenZipUtils.cpp
    src/7ZipMenu.rc
    src/Source.def
)

target_include_directories(7ZipMenu PRIVATE src)

target_link_libraries(7ZipMenu PRIVATE
    shlwapi.lib
    shell32.lib
    ole32.lib
    propsys.lib
    runtimeobject.lib
)

target_compile_definitions(7ZipMenu PRIVATE
    UNICODE
    _UNICODE
    WIN32_LEAN_AND_MEAN
)

# Static CRT — no runtime dependency when loaded into Explorer
set_property(TARGET 7ZipMenu PROPERTY
    MSVC_RUNTIME_LIBRARY "MultiThreaded$<$<CONFIG:Debug>:Debug>")

# Embed the DLL manifest for MSIX identity
target_sources(7ZipMenu PRIVATE src/7ZipMenu.rc)
