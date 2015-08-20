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

using Microsoft.WindowsAzure.MobileServices;
using Microsoft.WindowsAzure.MobileServices.Sync;
using Microsoft.WindowsAzure.MobileServices.SQLiteStore;


namespace helps.Droid
{
    [Activity(Label = "Register", Icon = "@drawable/ic_launcher", Theme = "@style/AppTheme.MyToolbar")]
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

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            Finish();
            return base.OnOptionsItemSelected(item);
        }

    }
}