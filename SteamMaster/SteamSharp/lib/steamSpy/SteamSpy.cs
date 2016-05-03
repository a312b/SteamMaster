using RestSharp;
using SteamSharpCore.restsharp;
using SteamSharpCore.steamSpy.models;

namespace SteamSharpCore.steamSpy
{
    internal class SteamSpy : RestRequestBase
    {
        public SteamSpy()
        {
            BaseUrl = "http://www.steamspy.com/api.php";
        }
        public SteamSpyData GetSteamSpyDataById(string steamId)
        {
            var request = new RestRequest();
            request.AddParameter("request", "appdetails", ParameterType.QueryString);
            request.AddParameter("appid", steamId, ParameterType.QueryString);
            request.RequestFormat = DataFormat.Json; //Set the format that we want the api to return. JSON is default for steam, but it never hurts

            return Execute<SteamSpyData>(request);

        }
    }
}
