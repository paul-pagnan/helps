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
    [Activity(Label = "Register", Icon = "@drawable/ic_launcher")]
    public class RegisterActivity : Main
    {
        private ActionMenuView amvMenu;
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.Activity_Register);

            var t = FindViewById<Toolbar>(Resource.Id.Ttoolbar);

            //Toolbar will now take on default actionbar characteristics
       
            ActionBar.Title = "Sign Up";

            client = new MobileServiceClient(applicationURL, applicationKey);


           amvMenu = t.FindViewById<ActionMenuView>(Resource.Id.amvMenu);

            //        amvMenu.SetOnMenuItemClickListener(new ActionMenuView.OnMenuItemClickListener() {
            //  @Override
            //  public boolean onMenuItemClick(MenuItem menuItem)
            //    {
            //        return onOptionsItemSelected(menuItem);
            //    }
            //});

            SetActionBar(t);
            setPadding(t);

            ActionBar.Title = "";
            ActionBar.SetDisplayHomeAsUpEnabled(true);
        }


        public bool onCreateOptionsMenu(IMenu menu)
        {

            MenuInflater.Inflate(Resource.Menu.simple, menu);
            return base.OnCreateOptionsMenu(menu);
        }

        public bool onOptionsItemSelected(IMenuItem item)
        {
            // Do your actions here
            return true;
        }

    }
}