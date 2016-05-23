@echo off
@echo Starting database
start cmd /k "%~dp0MongoStart.bat"
timeout 3
@echo Restoring files
"%~dp0MongoRestore.bat"
@echo done!
@echo Exiting in
timeout 3
