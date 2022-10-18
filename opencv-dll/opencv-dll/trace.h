#pragma once
#include <stdio.h>
#include <wtypes.h>
#include <cstdarg>

namespace dbg
{
    // Formats a message and sends it to the debugger using the OutputDebugString API.
    void trace(
        const wchar_t* msg,
        ...
    );
}