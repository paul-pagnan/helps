using System;
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


namespace helps.Shared
{
    public class HelpsService
    {
        public static HttpClient helpsClient;
        
        public const string helpsApplicationURL = @"http://helps.pagnan.com.au/";
        public const string helpsApplicationKey = @"94n4NXGofY2Esdd36GlQ3JR66T102bXI";

        static HelpsService() {
            //CurrentPlatform.Init();
            helpsClient = new HttpClient();
            helpsClient.BaseAddress = new Uri(helpsApplicationURL);
            helpsClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            helpsClient.DefaultRequestHeaders.Add("AppKey", helpsApplicationKey);
            Task.Factory.StartNew(Purge);
        }

        public static async void Purge()
        {
            await helpsClient.GetAsync("api/workshop/workshopSets/as");
        }
    }
}
