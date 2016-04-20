using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using HtmlAgilityPack;
using RestSharp;
using SteamSharp.restsharp;
using SteamSharp.steamStore.models;

namespace SteamSharp.steamStore
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
            //Implements a CookieAwareWebClient that downloads the html with a cookie to string so it's able to be passed to HtmlAgilityPack's LoadHtml
            WebClient wc = new CookieAwareWebClient();
            string getpage = wc.DownloadString("http://store.steampowered.com/app/" + gameId);

            var document = new HtmlDocument();
            document.LoadHtml(getpage);
            //A lot happens here
            //First an array of htmlnodes are created
            //The array is filled using the ToArray() function
            //The data is scraped from the html document using the provided xpath

            var gameTagsList = new List<SteamStoreGame.Tag>();

            if (document.DocumentNode.SelectNodes("//*[@id=\"game_highlights\"]/div[1]/div/div[4]/div/div[2]//a") !=
                null)
            {
                HtmlNode[] nodes =
                    document.DocumentNode.SelectNodes("//*[@id=\"game_highlights\"]/div[1]/div/div[4]/div/div[2]//a")
                        .ToArray();

                foreach (HtmlNode item in nodes)
                {
                    var tag = new SteamStoreGame.Tag();
                    string tagDescription = item.InnerHtml;
                    tag.description = tagDescription.Trim();
                    gameTagsList.Add(tag);

                }
            }

            return gameTagsList;
        }

        //Overrides the GetWebRequest in WebRequest and adds the necessary cookie to avoid ageCheck
        public class CookieAwareWebClient : WebClient
        {
            private readonly CookieContainer cookieContainer = new CookieContainer();
            private readonly Cookie ageCookie = new Cookie("birthtime", "441759601");

            protected override WebRequest GetWebRequest(Uri address)
            {
                ageCookie.Domain = "store.steampowered.com";
                ageCookie.Expires = DateTime.MaxValue;
                ageCookie.Path = "/";
                cookieContainer.Add(ageCookie);
                WebRequest request = base.GetWebRequest(address);
                HttpWebRequest webRequest = request as HttpWebRequest;
                if (webRequest != null)
                {
                    webRequest.CookieContainer = cookieContainer;
                }
                return request;
            }
        }
        public SteamStoreGame GetSteamStoreGameById(string gameId)
        {

            //Get the data from the steam store for the game
            var storeGame = GetGameData(gameId);
            storeGame.data.tags = GetTags(gameId);

            return storeGame;
        }
    }
}
