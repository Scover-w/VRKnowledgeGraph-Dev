@echo off
setlocal enableDelayedExpansion

set "folderPath=%~1"
if "%folderPath%"=="" set "folderPath=%CD%"

set "totalLines=0"

for /R "%folderPath%" %%G in (*.cs) do (
    for /f %%a in ('find /v /c "" ^< "%%G"') do (
        set /a "totalLines+=%%a"
    )
)


echo Total Lines: !totalLines!

endlocal

pause
