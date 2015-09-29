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
using System.Globalization;
using helps.Shared.DataObjects;
using helps.Shared.Consts;
using helps.Droid.Helpers;

namespace helps.Droid
{
    [Activity(Label = "My Information", WindowSoftInputMode = SoftInput.AdjustPan, Theme = "@style/AppTheme.MyToolbar")]
    public class ViewWorkshopActivity : Main
    {
        private TextView title;
        private TextView room;
        private TextView date;
        private TextView targetGroup;
        private TextView whatItCovers;
        private TextView placeAvailable;

        protected override int LayoutResource
        {
            get { return Resource.Layout.Activity_ViewSession; }
        }

        private void InitComponents()
        {
            title = FindViewById<TextView>(Resource.Id.textViewTitleValue);

            room = FindViewById<TextView>(Resource.Id.textViewRoomValue);

            date = FindViewById<TextView>(Resource.Id.textViewDateValue);

            targetGroup = FindViewById<TextView>(Resource.Id.textViewTargetGroupValue);

            whatItCovers = FindViewById<TextView>(Resource.Id.textViewWhatItCoversValue);

            placeAvailable = FindViewById<TextView>(Resource.Id.textViewPlaceAvailableValue);
        }

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
           
            InitComponents();
        }

    }
}