# SteamMaster
The "Master" git containing all the combined needed code for the Steam Recommender System.

## Parts individually locatable at
https://github.com/a312b/SteamSharp (Legacy)

https://github.com/a312b/UI (Legacy)


# Instructions

This application requires [MongoDB](https://www.mongodb.org/downloads#production). It was developed with version 3.2.4 and may not support later releases.

Setup batch files are included in the Mongo folder. The batch files assume you installed Mongo to the default installataion directory (C:\Program Files\MongoDB)

## How to setup:
If you are running the database for the first time, the database needs to be restored.

Simply run MongoRestore.bat and it should take care of everything. The database it generates takes up roughly 300mB of disk space.

If you need to run the database again, simply run MongoStart.bat

The database (mongod.exe) MUST be running when you try to run the program, or it will nok work.

The program requires an active internet connection.

The interface prompts for a Steam 64 ID which is a unique string that identifies a specific steam profile.

If you simply want to test the program and don't possess such an ID, activating the text box and pressing the "ALT" key will insert a test ID.


## Alternative
Alternative manual setup guide for Windows provided by MongoDB can be found [here](https://docs.mongodb.org/manual/tutorial/install-mongodb-on-windows/)
