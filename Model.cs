using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;

namespace Cryptocurrency
{
    // class for data retrieval
    public class Model
    {
        // personal key to data
        // cleared for privacy
        private static string API_KEY = "";

        //constructor
        public Model()
        {
            try
            {
                // try reach server
                Dictionary<string, string> parameters = new Dictionary<string, string>();
                var response = makeAPICall("/ping", parameters);
                Debug.WriteLine("\nInitialization:\n" + response + "\n");
            }
            catch (WebException e)
            {
                Debug.WriteLine("\nInitialization:\n" + e.Message + "\n");
            }
        }

        // connect to API
        public static string makeAPICall(string endpoints, Dictionary<string, string> parameters)
        {
            string baseURL = "https://api.coingecko.com/api/v3";
            baseURL += endpoints;

            var client = new WebClient();

            // if API KEY is provided
            if (API_KEY != string.Empty)
            {
                client.Headers.Add("x_cg_pro_api_key", API_KEY);
            }

            client.Headers.Add("Accept", "application/json");
            client.Headers.Add("User-Agent", "CryptoManagerApp");

            // add parameters to the query
            string query = "";
            if (parameters != null)
            {
                foreach (var pair in parameters)
                {
                    string key = Uri.EscapeDataString(pair.Key);
                    string value = Uri.EscapeDataString(pair.Value);
                    query += (query == "") ? $"?{key}={value}" : $"&{key}={value}";
                }

                Debug.WriteLine(baseURL + query);
                return client.DownloadString(baseURL + query);
            }
            else
            {
                Debug.WriteLine(baseURL);
                return client.DownloadString(baseURL);
            }
        }
    }
}
