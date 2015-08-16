using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.MobileServices;
using Microsoft.WindowsAzure.MobileServices.Sync;
using Microsoft.WindowsAzure.MobileServices.SQLiteStore;
using System.IO;
using Newtonsoft.Json.Linq;


namespace helps
{
    [Activity(MainLauncher = true,
               Icon = "@drawable/ic_launcher", Label = "@string/app_name",
               Theme = "@style/AppTheme")]
    public class SignInActivity : Activity
    {
        private MobileServiceClient client;

        const string applicationURL = @"https://helps.azure-mobile.net/";
        const string applicationKey = @"kHDdsuIrdkMYobtTRBwJLqFzhBOHFJ90";

        protected override async void OnCreate(Bundle bundle)
        {

                      // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Activity_Sign_In);

            CurrentPlatform.Init();

            // Create the Mobile Service Client instance, using the provided
            // Mobile Service URL and key
            client = new MobileServiceClient(applicationURL, applicationKey);

         

            base.OnCreate(bundle);

            // Create your application here
        }

        [Java.Interop.Export()]
        public async void Login(View view)
        {

            try { 
                JToken response = await client.InvokeApiAsync("SignIn", "{ 'studentId': '11972080', 'password': 'sample string 2'}");
            }
            catch (Exception ex)
            {

            }

            var intent = new Intent(this, typeof(ToDoActivity));
            //StartActivity(intent);
        }
    }
}