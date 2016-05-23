@echo off
"C:\Program Files\MongoDB\Server\3.2\bin\mongorestore.exe" --collection Games --db SteamSharp "%~dp0Games.bson"