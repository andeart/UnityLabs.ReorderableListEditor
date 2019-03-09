@echo off
setlocal enabledelayedexpansion

set solutionDir=%1
set targetName=%2
set configurationName=%3
set targetDir=%4

cd /d %solutionDir%..

for /f "tokens=*" %%p in (.\Tools\unity-dirs.txt) do (
    set targetCopyDir=%solutionDir%..\%%p\%targetName%

    if "%configurationName%"=="Release" (
        if not exist "!targetCopyDir!" mkdir "!targetCopyDir!"
        cd /d "!targetCopyDir!"
        for %%a in (*.dll*) do (
            del "%%a"
        )
        for %%a in (*.pdb*) do (
            del "%%a"
        )
        for %%a in (*.xml*) do (
            del "%%a"
        )
        for %%a in (*.mdb*) do (
            del "%%a"
        )
        
        cd /d %targetDir%
        copy "%targetName%.dll" "!targetCopyDir!" /y
        copy "%targetName%.pdb" "!targetCopyDir!" /y
        copy "%targetName%.xml" "!targetCopyDir!" /y
    )
)



