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
using helps.Droid.Helpers;

namespace helps.Droid
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.MyToolbar")]
    public class DashboardActivity : Main
    {
        protected override int LayoutResource
        {
            get { return Resource.Layout.Activity_Dashboard; }
        }

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

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


        [Java.Interop.Export()]
        public void MyWorkshopBookings(View view)
        {
            var intent = new Intent(this, typeof(MyWorkshopBookingsActivity));
            StartActivity(intent);
        }

        [Java.Interop.Export()]
        public void MakeBooking(View view)
        {
            var intent = new Intent(this, typeof(MakeBookingActivity));
            StartActivity(intent);
        }

        [Java.Interop.Export()]
        public void Settings(View view)
        {
            var intent = new Intent(this, typeof(SettingsActivity));
            StartActivity(intent);
        }
    }
}