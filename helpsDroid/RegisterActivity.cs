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
    [Activity(Label = "Register", Icon = "@drawable/ic_launcher", WindowSoftInputMode = SoftInput.AdjustPan, Theme = "@style/AppTheme.MyToolbar")]
    public class RegisterActivity : Main
    {

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.Activity_Register);

            var t = FindViewById<Toolbar>(Resource.Id.Ttoolbar);

            //t.InflateMenu(Resource.Menu.simple);

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

            EditText FirstName = FindViewById<EditText>(Resource.Id.registerFirstName);
            EditText LastName = FindViewById<EditText>(Resource.Id.registerLastName);
            EditText StudentId = FindViewById<EditText>(Resource.Id.registerStudentId);
            EditText Email = FindViewById<EditText>(Resource.Id.registerEmail);
            EditText Password = FindViewById<EditText>(Resource.Id.registerPassword);
            AuthService RegisterService = new AuthService();

            AuthResult Response = await RegisterService.Register(FirstName.Text, LastName.Text, Email.Text, StudentId.Text, Password.Text);
            dialog.Hide();

            if (Response.Success)
            {

                AlertDialog.Builder builder = new AlertDialog.Builder(this);
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