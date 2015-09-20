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
using Android.Support.V4.View;
using com.refractored;
using Java.Interop;
using Fragment = Android.Support.V4.App.Fragment;
using FragmentManager = Android.Support.V4.App.FragmentManager;
using Java.Lang;
using helps.Droid.Helpers;
using helps.Droid.Adapters;


namespace helps.Droid
{
    [Activity(Label = "My Bookings", Theme = "@style/AppTheme.MyToolbar")]
    public class MyBookingsActivity : MyBookingsFragment
    {
        private MyPagerAdapter adapter;
        private ViewPager pager;
        private PagerSlidingTabStrip tabs;

        protected override int LayoutResource
        {
            get { return Resource.Layout.Activity_MyBookings; }
        }
        
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);          
            adapter = new MyPagerAdapter(SupportFragmentManager, this.LayoutInflater);
            pager = FindViewById<ViewPager>(Resource.Id.pager);
            tabs = FindViewById<PagerSlidingTabStrip>(Resource.Id.tabs);
            pager.Adapter = adapter;
            tabs.SetViewPager(pager);

            ActionBar.SetDisplayHomeAsUpEnabled(true);
            ActionBar.SetDisplayShowHomeEnabled(true);
        }
    }

    class MyPagerAdapter : Android.Support.V4.App.FragmentPagerAdapter
    {
        private readonly string[] Titles =
        {
            "Current", "Past"
        };

        private LayoutInflater layoutInflater;

        public MyPagerAdapter(FragmentManager fm, LayoutInflater layoutInflater) : base(fm)
        {
            this.layoutInflater = layoutInflater;
        }

        public override ICharSequence GetPageTitleFormatted(int position)
        {
            return new Java.Lang.String(Titles[position]);
        }
        

        public override int Count
        {
            get { return Titles.Length; }
        }

        public override Android.Support.V4.App.Fragment GetItem(int position)
        {
            return TabFragment.NewInstance(position);
        }
    }
}