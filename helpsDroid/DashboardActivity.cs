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
using Android.Graphics;

namespace helps.Droid
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.MyToolbar")]
    public class DashboardActivity : Main
    {
        protected override async void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            Init();
            SetContentView(Resource.Layout.Activity_Dashboard);
            var t = FindViewById<Toolbar>(Resource.Id.Ttoolbar);
            SetActionBar(t);
            setPadding(t);

            Color color = Resources.GetColor(Resource.Color.tint);
            FindViewById<Button>(Resource.Id.btnMakeBooking).Background.SetColorFilter(color, PorterDuff.Mode.SrcAtop);
            FindViewById<Button>(Resource.Id.btnMyBooking).Background.SetColorFilter(color, PorterDuff.Mode.SrcAtop);
            FindViewById<Button>(Resource.Id.btnSettings).Background.SetColorFilter(color, PorterDuff.Mode.SrcAtop);
        }


        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.logout, menu);
            return base.OnCreateOptionsMenu(menu);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            Logout();
            return base.OnOptionsItemSelected(item);
        }
    }
}