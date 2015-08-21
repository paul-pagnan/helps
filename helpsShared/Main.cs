using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using helps.Shared.DataObjects;

using Microsoft.WindowsAzure.MobileServices;
using Microsoft.WindowsAzure.MobileServices.Sync;
using Microsoft.WindowsAzure.MobileServices.SQLiteStore;


namespace helps.Shared
{
    public class Main
    {

        public MobileServiceClient client;

        public const string applicationURL = @"https://helps25.azure-mobile.net/";
        public const string applicationKey = @"EcJyqLPpfEiVHyiAwKGmrIKvCQXjtL23";

        public async void Init()
        {
            //CurrentPlatform.Init();
            client = new MobileServiceClient(applicationURL, applicationKey);
        }
    }
}
