using System;
using System.Collections.Generic;
using MongoDB.Bson;
using SteamSharpCore.steamStore.models;

namespace Database.lib.converter.models
{
    public class Game
    {
        public ObjectId _id { get; set; }
        //The main class. Contains all the information about a Steam game. Data is pulled from Steam Store, Steamspy and Steam Store Api.
        //The data is stored in JSON like format in the database. A game can be retrieved by its AppId from the database.
        //Note: All games have 2 id's. One is the Steam App Id, the other is the Id givin by the Database. Use the Steam App Id in most cases.
        public string Title { get; set; }
        public List<string> Developer { get;  set; }

        //Note: The developer and publisher can be the same. Ex: Valve
        public List<string> Publisher { get;  set; }

        //Note these Id's have varying length.
        public int SteamAppId { get;  set; }
        public string Description { get; set; }

        //String is converted from the Steam Store Api to a DateTime object. This can be used for sorting.
        public bool Released { get; set; }
        public string ReleaseDate { get;  set; }
        public int AveragePlayTime { get;  set; }
        public int AveragePlayTime2Weeks { get;  set; }

        //Need user rating (is it int or string?)
        //The Steam store has is_free = true if game is free. This is translated to Price = 0.
        //Steam Store Api provides the price as an Int like 999 which translates to 9.99. Convetions is handled on retrieval.
        //Price does not include discounts as of now!
        public double Price { get;  set; }
        public int AgeRating { get;  set; }
        public Uri CoverImage { get;  set; }
        public Uri StoreLink { get;  set; }

        //Genre is not the same as Catagory. Genre contains "Action". Catagory contains "Multiplayer"
        public List<SteamStoreGame.Genre> Genres { get;  set; }
        public List<SteamStoreGame.Category> Categories { get;  set; }

        //Games Tags. These are pulled using a HTML request. 
        public List<SteamStoreGame.Tag> Tags { get; set; }

        public int OwnerCount { get;  set; }
        public bool Windows { get; set; }
        public bool Mac { get; set; }
        public bool Linux { get; set; }

    }
}
