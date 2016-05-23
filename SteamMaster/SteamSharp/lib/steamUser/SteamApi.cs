using RestSharp;
using SteamSharpCore.restsharp;
using SteamSharpCore.steamUser.models;
using static System.String;

namespace SteamSharpCore.steamUser
{
    internal class SteamApi : RestRequestBase
    {
        public SteamApi(string steamKey)
        {
            SteamDelveoperKey = steamKey;
            BaseUrl = " http://api.steampowered.com/";
        }

        private string SteamDelveoperKey { get; }

        public SteamUser GetSteamUserData(string[] steamUserIds)
        {
            var idString = Join(",", steamUserIds); //Convert to a comma seperated string (This is the format required by the api)
            var request = new RestRequest {Resource = "ISteamUser/GetPlayerSummaries/v0002/" };
            request.AddParameter("key", SteamDelveoperKey, ParameterType.QueryString);
            request.AddParameter("steamids", idString, ParameterType.QueryString);
            request.RequestFormat = DataFormat.Json; //Set the format that we want the api to return. JSON is default for steam, but it never hurts
            request.RootElement = "response";

            return Execute<SteamUser>(request);

        }

        public UserGameTime GetGameTimeForUserById(string steamUserId)
        {
            var request = new RestRequest { Resource = "IPlayerService/GetOwnedGames/v0001/" };
            request.AddParameter("key", SteamDelveoperKey, ParameterType.QueryString);
            request.AddParameter("steamid", steamUserId, ParameterType.QueryString);
            request.RequestFormat = DataFormat.Json; //Set the format that we want the api to return. JSON is default for steam, but it never hurts
            request.RootElement = "response";

            return Execute<UserGameTime>(request);
        }
    }
}
