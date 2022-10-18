#include "trace.h"
#define TRACE_BUFFER_SIZE 512

namespace dbg
{
    void trace(
        const wchar_t* msg,
        ...)
    {
        //
        // Keep two extra characters for the line terminator
        // and the null terminator.
        //
        wchar_t buffer[TRACE_BUFFER_SIZE + 2] = {};
        va_list args;

        va_start(args, msg);
        _vsnwprintf_s(buffer, _countof(buffer) - 2, _TRUNCATE, msg, args);
        va_end(args);

        buffer[wcslen(buffer) + 1] = L'\0';
        buffer[wcslen(buffer)] = L'\n';

        OutputDebugString(buffer);
    }
}