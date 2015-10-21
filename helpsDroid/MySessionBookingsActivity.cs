using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Support.V4.View;
using com.refractored;
using Java.Interop;
using Java.Lang;
using helps.Droid.Helpers;
using helps.Droid.Adapters;

namespace helps.Droid
{
    [Activity(Label = "Session Bookings", Theme = "@style/AppTheme.MyToolbar")]
    public class MySessionBookingsActivity : MyBookingsFragment
    {
        private MyPagerAdapter adapter;
        private ViewPager pager;
        private PagerSlidingTabStrip tabs;

        protected override int LayoutResource
        {
            get { return Resource.Layout.Activity_Bookings; }
        }
        
        protected async override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            ActionBar.SetDisplayHomeAsUpEnabled(true);
            ActionBar.SetDisplayShowHomeEnabled(true);

            adapter = new MyPagerAdapter(FragmentManager, this.LayoutInflater, false);
            pager = FindViewById<ViewPager>(Resource.Id.pager);
            tabs = FindViewById<PagerSlidingTabStrip>(Resource.Id.tabs);
            pager.Adapter = adapter;
            tabs.SetViewPager(pager);
        }
    }

}