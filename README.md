# SteamMaster
The "Master" git containing all the combined needed code for the Steam Recommender System.

## Each is individually locatable at
https://github.com/a312b/SteamSharp

https://github.com/a312b/UI


# Requires MongoDB 
## Set up
1. Install [MongoDB](https://www.mongodb.org/downloads#production) for your platform.
2. A setup guide for Windows can be found [here](https://docs.mongodb.org/manual/tutorial/install-mongodb-on-windows/).
3. To use the library and the database provided, specify the path to the folder containing the data.
Assuming your dir is at C:\Source\
```
C:\mongodb\bin\mongod.exe --dbpath C:\Source\SteamSharp\Database\data
```
