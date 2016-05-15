using System.Collections.Generic;
using System.Linq;
using HtmlAgilityPack;
using RestSharp;
using SteamSharpCore.restsharp;
using SteamSharpCore.steamStore.models;

namespace SteamSharpCore.steamStore
{
    //Class for interacting with the Steam Store
    internal class SteamStore : RestRequestBase
    {
        public SteamStore()
        {
            BaseUrl = "http://store.steampowered.com/api/";
        }

        public SteamStoreGame GetGameData(string steamAppid)
        {
            var request = new RestRequest {Resource = "appdetails"};
            //Added to the base url in Rest client
            request.AddQueryParameter("appids", steamAppid); //Set the parameter. This is added with a leading ?. such: BASE_URL/appdetails?appids=STRING_GIVEN
            request.RequestFormat = DataFormat.Json; //Set the format that we want the api to return. JSON is default for steam, but it never hurts
            request.RootElement = steamAppid; //Set the Root of the DOM of the returned JSON. This is the point where the deserializer will start

            return Execute<SteamStoreGame>(request);
        }

        public List<SteamStoreGame.Tag> GetTags(string gameId)
        {
            var src = new HtmlWeb();
            HtmlDocument document = src.Load("http://store.steampowered.com/app/" + gameId);
            //A lot happens here
            //First an array of htmlnodes are created
            //The array is filled using the ToArray() function
            //The data is scraped from the html document using the provided xpath
            HtmlNode[] nodes = document.DocumentNode.SelectNodes("//*[@id=\"game_highlights\"]/div[1]/div/div[4]/div/div[2]//a").ToArray();

            var gameTagsList = new List<SteamStoreGame.Tag>();

            foreach (HtmlNode item in nodes)
            {
                var tag = new SteamStoreGame.Tag();
                string tagDescription = item.InnerHtml;
                tag.description = tagDescription.Trim();
                gameTagsList.Add(tag);
                
            }

            return gameTagsList;
        }

        public SteamStoreGame GetSteamStoreGameById(string gameId)
        {

            //Get the data from the steam store for the game
            var storeGame = GetGameData(gameId);
            //storeGame.data.tags = GetTags(gameId);

            return storeGame;
        }
    }
}
