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
using helps.Shared.DataObjects;
using helps.Shared.Database;

namespace helps.Droid
{
    [Activity(MainLauncher = true,
               Label = "@string/app_name",
               Theme = "@style/AppTheme.SignIn")]
    public class SignInActivity : Main
    {
        private AuthService AuthSvc;
        protected override async void OnCreate(Bundle bundle)
        {
            Xamarin.Forms.Forms.Init(this, bundle);
            AuthSvc = new AuthService();     
            SetContentView(Resource.Layout.Activity_Sign_In);
            base.OnCreate(bundle);
        }

        [Java.Interop.Export()]
        public async void Login(View view)
        {
            ProgressDialog dialog = CreateProgressDialog("Logging In...", this);
            dialog.Show();
               
            LoginRequest request = new LoginRequest
            {
                StudentId = FindViewById<EditText>(Resource.Id.loginStudentId).Text,
                Password = FindViewById<EditText>(Resource.Id.loginPassword).Text
            };

            var Response = await AuthSvc.Login(request);
            dialog.Dismiss();

            if (Response.Success)
            {
                var intent = new Intent(this, typeof(ToDoActivity));
                StartActivity(intent);
            }
            else
                ShowDialog(Response.Message, "Login Failure");
        }

        [Java.Interop.Export()]
        public async void ShowRegister(View view)
        {
            var intent = new Intent(this, typeof(RegisterActivity));
            StartActivity(intent);
        }

        [Java.Interop.Export()]
        public async void ShowForgotPassword(View view)
        {
            var intent = new Intent(this, typeof(ForgotPasswordActivity));
            StartActivity(intent);
        }
    }
}