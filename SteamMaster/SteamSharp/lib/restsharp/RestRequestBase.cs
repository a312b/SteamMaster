using System;
using RestSharp;

namespace SteamSharp.restsharp
{
    internal abstract class RestRequestBase
    {
        public string BaseUrl { get; set; }

        //Execute the a rest request provided as parameter
        public T Execute<T>(RestRequest request) where T : new()
        {
            var client = new RestClient {BaseUrl = new Uri(BaseUrl)};

            var response = client.Execute<T>(request);

            if (response.ErrorException != null)
            {
                const string message = "Error retrieving response.  Check inner details for more info.";
                var steamStoreException = new ApplicationException(message, response.ErrorException);
                throw steamStoreException;
            }

            return response.Data;
        }
    }
}
