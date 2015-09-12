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
using helps.Shared;
using helps.Shared.DataObjects;

namespace helps.Droid
{
    [Activity(Label = "Register", WindowSoftInputMode = SoftInput.AdjustPan, Theme = "@style/AppTheme.SignIn")]
    public class RegisterActivity : Main
    {
        protected override void OnCreate(Bundle bundle)
        {
            Init();
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.Activity_Register);
            AuthSvc = new AuthService();

            var t = FindViewById<Toolbar>(Resource.Id.TtoolbarTransparent);
            
            SetActionBar(t);
            setPadding(t);
            ActionBar.SetDisplayHomeAsUpEnabled(true);
            ActionBar.SetDisplayShowHomeEnabled(true);
        }

        [Java.Interop.Export()]
        public async void Register(View view)
        {
            ProgressDialog dialog = CreateProgressDialog("Registering...", this);
            dialog.Show();

            RegisterRequest request = new RegisterRequest
            {
                FirstName = FindViewById<EditText>(Resource.Id.registerFirstName).Text,
                LastName = FindViewById<EditText>(Resource.Id.registerLastName).Text,
                StudentId = FindViewById<EditText>(Resource.Id.registerStudentId).Text,
                Email = FindViewById<EditText>(Resource.Id.registerEmail).Text,
                Password = FindViewById<EditText>(Resource.Id.registerPassword).Text
            };
            
            AuthResult Response = await AuthSvc.Register(request);
            dialog.Hide();

            if (Response.Success)
            {
                AlertDialog.Builder builder = new AlertDialog.Builder(this, Resource.Style.LightDialog);
                builder.SetTitle("Successfully Registered");
                builder.SetMessage("Please check your emails to confirm your email address");
                builder.SetCancelable(false);
                builder.SetPositiveButton("OK", delegate { Finish(); });
                builder.Show();
            }
            else
            {
                ShowDialog(Response.Message, Response.Title);
            }
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            Finish();
            return base.OnOptionsItemSelected(item);
        }

    }
}