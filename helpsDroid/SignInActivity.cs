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
using helps.Shared;
using helps.Shared.DataObjects;

namespace helps.Droid
{
    [Activity(MainLauncher = true, Icon = "@drawable/ic_launcher", Label = "@string/app_name", Theme = "@style/AppTheme.SignIn")]
    public class SignInActivity : Main
    {
        protected override async void OnCreate(Bundle bundle)
        {
            Xamarin.Forms.Forms.Init(this, bundle);
            Init();
            //Check if the user has an active session
           
            if(CurrentUser != null)
                SwitchActivity();

            SkipLogin();

            SetContentView(Resource.Layout.Activity_Sign_In);
            base.OnCreate(bundle);
        }

        public async void SkipLogin()
        {
            LoginRequest request = new LoginRequest
            {
                StudentId = "11972080",
                Password = "password123"
            };

            var Response = await AuthSvc.Login(request);

            if (Response.Success)
            {
                SwitchActivity();
            }
            else
                ShowDialog(Response.Message, "Login Failure");
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
                SwitchActivity();
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

        private void SwitchActivity()
        {
            var intent = new Intent(this, typeof(DashboardActivity));
            if (!AuthSvc.CurrentUser().HasLoggedIn)
                intent = new Intent(this, typeof(DetailsInputActivity));
            StartActivity(intent);
            Finish();
        }
    }
}