@echo off

echo Running project...
start "Frontend" /d "Frontend" dotnet Frontend.dll
start "Backend" /d "Backend" dotnet Backend.dll
pause
exit 0