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


namespace helps
{
    [Activity(Label = "Register", Icon = "@drawable/ic_launcher", Theme = "@style/MyToolbar")]
    public class RegisterActivity : Main
    {
        private ActionMenuView amvMenu;
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.Activity_Register);

            var t = FindViewById<Toolbar>(Resource.Id.Ttoolbar);
            
            //t.InflateMenu(Resource.Menu.simple);

            SetActionBar(t);
            setPadding(t);
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.simple, menu);
            return base.OnCreateOptionsMenu(menu);
        }
        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            Toast.MakeText(this, "Top ActionBar pressed: " + item.TitleFormatted, ToastLength.Short).Show();
            return base.OnOptionsItemSelected(item);
        }

    }
}