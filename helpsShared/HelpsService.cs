using System;

using System.Net.Http;
using System.Net.Http.Headers;
using Xamarin.Forms;
using System.Collections.Generic;
using System.Net.Http.Formatting;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace helps.Shared
{
    /// <summary>
    /// HelpsService defines and initialises the helps API client as well as provides a number of helper methods for the child services to use
    /// </summary>
    public class HelpsService : Main
    {
        protected readonly static HttpClient helpsClient;

        private const string helpsApplicationURL = @"http://helps.pagnan.com.au/";
        private const string helpsApplicationKey = @"94n4NXGofY2Esdd36GlQ3JR66T102bXI";
        protected const string DateFormat = "yyy-MM-ddThh:mm";

        /// <summary>
        /// Initialise the Helps API client
        /// </summary>
        static HelpsService()
        {
            helpsClient = new HttpClient();
            helpsClient.BaseAddress = new Uri(helpsApplicationURL);
            helpsClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            helpsClient.DefaultRequestHeaders.Add("AppKey", helpsApplicationKey);
        }

        /// <summary>
        /// Send an initial request to the server to wake it up. Without this, the first request can take upwards of 10 seconds to return.
        /// </summary>
        public static async void Purge()
        {
            await helpsClient.GetAsync("api/workshop/workshopSets/as");
        }

        /// <summary>
        /// Test the internet connection. Throws System.Net.WebException
        /// </summary>
        protected static void TestConnection()
        {
            var networkConnection = DependencyService.Get<INetworkConnection>();
            networkConnection.CheckNetworkConnection();
            if (!networkConnection.IsConnected)
                throw new System.Net.WebException();
        }

        /// <summary>
        /// Helper method to define if the user has connection to the internet
        /// </summary>
        /// <returns></returns>
        protected static bool IsConnected()
        {
            var networkConnection = DependencyService.Get<INetworkConnection>();
            networkConnection.CheckNetworkConnection();
            return networkConnection.IsConnected;
        }

        protected static IEnumerable<JsonMediaTypeFormatter> Formatters()
        {
            return new[] {new JsonMediaTypeFormatter {
                  SerializerSettings = new JsonSerializerSettings {
                      Converters = new List<JsonConverter> {
                        new MyDateTimeConvertor()
                       }
                     }
                  }
            };
        }
    }

    public class MyDateTimeConvertor : DateTimeConverterBase
    {
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            return reader.Value;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
        }
    }
}
