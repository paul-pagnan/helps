﻿using System;

using System.Net.Http;
using System.Net.Http.Headers;
using Xamarin.Forms;
using System.Collections.Generic;
using System.Net.Http.Formatting;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace helps.Shared
{
    public class HelpsService : Main
    {
        public static bool CurrentlyUpdating = false;

        public static HttpClient helpsClient;

        public const string helpsApplicationURL = @"http://helps.pagnan.com.au/";
        public const string helpsApplicationKey = @"94n4NXGofY2Esdd36GlQ3JR66T102bXI";

        public const string DateFormat = "yyy-MM-ddThh:mm";

        static HelpsService()
        {

            helpsClient = new HttpClient();
            helpsClient.BaseAddress = new Uri(helpsApplicationURL);
            helpsClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            helpsClient.DefaultRequestHeaders.Add("AppKey", helpsApplicationKey);
        }

        public static async void Purge()
        {
            await helpsClient.GetAsync("api/workshop/workshopSets/as");
        }

        public static void TestConnection()
        {
            var networkConnection = DependencyService.Get<INetworkConnection>();
            networkConnection.CheckNetworkConnection();
            if (!networkConnection.IsConnected)
                throw new System.Net.WebException();
        }


        public IEnumerable<JsonMediaTypeFormatter> Formatters()
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
