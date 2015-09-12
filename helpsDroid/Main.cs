using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Xamarin.Forms;
using Android.Widget;
using helps.Shared;
using helps.Shared.DataObjects;
using helps.Shared.Consts;

namespace helps.Droid
{
    public class Main : Activity
    {
        public AuthService AuthSvc;
        public HelpsService HelpsSvc;
        public User CurrentUser;
        public void Init()
        {
            AuthSvc = new AuthService();
            HelpsSvc = new HelpsService();
            CurrentUser = AuthSvc.CurrentUser();

            //test();            
           
        }

        public async void test()
        {
            var request = new HelpsRegisterRequest
            {
                StudentId = "11972080",
                DateOfBirth = DateTime.Now,
                Degree = Degree.UG,
                Status = Status.International,
                FirstLanguage = "English",
                CountryOrigin = "Australia"
            };
            try
            {
                var response = await HelpsSvc.RegisterStudent(request);
                var a = response.Message;
            }
            catch (Exception ex)
            {
                var e = ex;
            }
        }

        public void setPadding(Toolbar toolbar)
        {
            // Set the padding to match the Status Bar height
            toolbar.SetPadding(0, getStatusBarHeight(), 0, 0);
        }

        public int getStatusBarHeight()
        {
            int result = 0;
            
            int resourceId = Resources.GetIdentifier("status_bar_height", "dimen", "android");
            if (resourceId > 0)
            {
                result = Resources.GetDimensionPixelSize(resourceId);
            }
            return result;
        }

        public void ShowDialog(string message, string title)
        {
            AlertDialog.Builder builder = new AlertDialog.Builder(this, Resource.Style.LightDialog);
            builder.SetMessage(message);
            builder.SetTitle(title);
            builder.Create().Show();
        }
        
        public ProgressDialog CreateProgressDialog(string message, Activity activity)
        {
            ProgressDialog mProgressDialog = new ProgressDialog(activity, Resource.Style.LightDialog);
            mProgressDialog.SetMessage(message);
            mProgressDialog.SetCancelable(false);
            mProgressDialog.SetProgressStyle(ProgressDialogStyle.Spinner);
            return mProgressDialog; 
        }

        public void Logout()
        {
            AuthSvc.Logout();
            CurrentUser = null;
            var intent = new Intent(this, typeof(SignInActivity));
            StartActivity(intent);
            Finish();
        }
    }
}