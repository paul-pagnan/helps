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
using helps.Shared;

namespace helps.Droid
{
    [Activity(MainLauncher = true,
               Icon = "@drawable/ic_launcher", Label = "@string/app_name",
               Theme = "@style/AppTheme.SignIn")]
    public class SignInActivity : Main
    {
        
        protected override async void OnCreate(Bundle bundle)
        {
            SetContentView(Resource.Layout.Activity_Sign_In);
            
            CurrentPlatform.Init();
            client = new MobileServiceClient(applicationURL, applicationKey);
            base.OnCreate(bundle);
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

        [Java.Interop.Export()]
        public async void ShowRegister(View view)
        {
            var intent = new Intent(this, typeof(RegisterActivity));
            StartActivity(intent);
        }
    }
}