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
            
            base.OnCreate(bundle);
        }

        [Java.Interop.Export()]
        public async void Login(View view)
        {
            EditText studentId = FindViewById<EditText>(Resource.Id.loginStudentId); 
            EditText password = FindViewById<EditText>(Resource.Id.loginPassword);
            AuthService LoginService = new AuthService();
            try
            {
                JToken response = await LoginService.Login(studentId.Text, password.Text);
                var intent = new Intent(this, typeof(ToDoActivity));
                StartActivity(intent);
            }
            catch (Exception ex)
            {
                ShowDialog(ex.Message, "Login Failure");
            }
        }

        [Java.Interop.Export()]
        public async void ShowRegister(View view)
        {
            var intent = new Intent(this, typeof(RegisterActivity));
            StartActivity(intent);
        }
    }
}