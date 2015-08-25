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
<<<<<<< HEAD
    [Activity(Label = "Forgot Password", WindowSoftInputMode = SoftInput.AdjustPan, Theme = "@style/AppTheme.MyToolbar")]
=======
    [Activity(Label = "Register", Icon = "@drawable/ic_launcher", WindowSoftInputMode = SoftInput.AdjustPan, Theme = "@style/AppTheme.MyToolbar")]
>>>>>>> 14fb71a... Add forgot password module. TODO: create web gui for forgot password
    public class ForgotPasswordActivity : Main
    {

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.Activity_ForgotPassword);

            var t = FindViewById<Toolbar>(Resource.Id.Ttoolbar);

            SetActionBar(t);
            setPadding(t);
            ActionBar.SetDisplayHomeAsUpEnabled(true);
            ActionBar.SetDisplayShowHomeEnabled(true);
        }

        [Java.Interop.Export()]
        public async void ForgotPassword(View view)
        {
            ProgressDialog dialog = CreateProgressDialog("Please wait...", this);
            dialog.Show();

            EditText StudentId = FindViewById<EditText>(Resource.Id.forgotStudentId);
            AuthService ForgotPasswordService = new AuthService();
            AuthResult Response = await ForgotPasswordService.ForgotPassword(StudentId.Text);
            dialog.Hide();

            if (Response.Success)
            {

                AlertDialog.Builder builder = new AlertDialog.Builder(this);
                builder.SetTitle("Password Reset Sent");
                builder.SetMessage("Please check your emails to reset your password");
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