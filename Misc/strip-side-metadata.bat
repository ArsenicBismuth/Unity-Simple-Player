@echo off
setlocal enabledelayedexpansion
REM Allow multiple files to be dropped onto the script

if "%~1"=="" (
    echo Please drag and drop one or more video files onto this script.
    pause
    exit /b 1
)

:processFiles
echo Processing: "%~1"
if "%~1"=="" goto end

set inputFile=%~1

echo Replace backslashes with forward slashes
set inputFile=%inputFile:\=/%
echo Input file: %inputFile%

echo Check if "[SIDE_DATA]" exists in the metadata
ffprobe -show_streams "%inputFile%" | findstr /c:"[SIDE_DATA]" >nul
if errorlevel 1 (
    echo No side data metadata found
) else (
    echo.
    echo Found side data metadata
    set outputFile=%~dpn1_stripped_meta%~x1
    echo Output file: !outputFile!
    echo Output file: %~dpn1_stripped_meta%~x1

    REM We use !outputFile! to allow for delayed expansion of the variable
    REM Delayed variable expansion is needed in batch scripts
    REM when you want to modify and access variables within a block of code
    
    REM Process with FFmpeg to strip specific metadata and write to a new file
    ffmpeg -i "%inputFile%" -map 0 -c copy -map_metadata 0 -metadata:s:v:0 side_data_type=none -metadata:s:v:1 side_data_type=none "!outputFile!"
)

shift
goto processFiles
:end
REM Pause for 3 seconds before closing
@REM timeout /t 3 /nobreak >nul
pause
exit /b 0