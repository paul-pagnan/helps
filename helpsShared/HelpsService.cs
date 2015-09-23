﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using helps.Shared.DataObjects;

using Microsoft.WindowsAzure.MobileServices;
using Microsoft.WindowsAzure.MobileServices.Sync;
using Microsoft.WindowsAzure.MobileServices.SQLiteStore;
using helps.Shared.Database;

using System.Net.Http;
using System.Net.Http.Headers;
using System.Diagnostics;
using Connectivity.Plugin;
using Xamarin.Forms;

namespace helps.Shared
{
    public class HelpsService : Main
    {
        public static bool CurrentlyUpdating = false;

        public static HttpClient helpsClient;
        
        public const string helpsApplicationURL = @"http://helps.pagnan.com.au/";
        public const string helpsApplicationKey = @"94n4NXGofY2Esdd36GlQ3JR66T102bXI";

        static HelpsService() {
            
            helpsClient = new HttpClient();
            helpsClient.BaseAddress = new Uri(helpsApplicationURL);
            helpsClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            helpsClient.DefaultRequestHeaders.Add("AppKey", helpsApplicationKey);
        }

        public static async void Purge()
        {
            await helpsClient.GetAsync("api/workshop/workshopSets/as");
        }

        public void TestConnection()
        {
            var networkConnection = DependencyService.Get<INetworkConnection>();
            try
            {
                networkConnection.CheckNetworkConnection();
            } catch(Exception ex)
            {
                var a = "as";
            }
            if (!networkConnection.IsConnected)
                throw new System.Net.WebException();
        }

    }
}
