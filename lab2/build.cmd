@echo off
if "%~1"=="" (
	echo "build <version>"
	pause
	exit
)

if exist %1 rmdir %1 /S /Q
mkdir %1

cd src/frontend/
dotnet publish -c Release -o %1
move %1 ../../%1/Frontend
cd ../../

cd src/backend/
dotnet publish -c Release -o %1
move %1 ../../%1/Backend
cd ../../

cd src/
@echo off
xcopy config ..\%1\config\
xcopy run.cmd ..\%1\
xcopy stop.cmd ..\%1\
cd ../

echo BUILD FINISHED