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
using helps.Droid.Helpers;

namespace helps.Droid
{ 
    [Activity(Label = "Forgot Password", WindowSoftInputMode = SoftInput.AdjustPan, Theme = "@style/AppTheme.SignIn")]
    public class ForgotPasswordActivity : Main
    {
        protected override int LayoutResource
        {
            get { return Resource.Layout.Activity_ForgotPassword; }
        }

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            ActionBar.SetDisplayHomeAsUpEnabled(true);
            ActionBar.SetDisplayShowHomeEnabled(true);
        }

        [Java.Interop.Export()]
        public async void ForgotPassword(View view)
        {
            ProgressDialog dialog = DialogHelper.CreateProgressDialog("Please wait...", this);
            dialog.Show();

            EditText StudentId = FindViewById<EditText>(Resource.Id.forgotStudentId);
            AuthService ForgotPasswordService = new AuthService();
            GenericResponse Response = await ForgotPasswordService.ForgotPassword(StudentId.Text);
            dialog.Hide();

            if (Response.Success)
            {

                AlertDialog.Builder builder = new AlertDialog.Builder(this, Resource.Style.LightDialog);
                builder.SetTitle("Password Reset Sent");
                builder.SetMessage("Please check your emails to reset your password");
                builder.SetCancelable(false);
                builder.SetPositiveButton("OK", delegate { Finish(); });
                builder.Show();
            }
            else
            {
                DialogHelper.ShowDialog(this, Response.Message, Response.Title);
            }
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            return base.OnOptionsItemSelected(item);
        }
    }
}