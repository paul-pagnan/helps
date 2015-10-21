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
    [Activity(Label = "My Workshop Bookings", Theme = "@style/AppTheme.MyToolbar")]
    public class MyWorkshopBookingsActivity : MyBookingsFragment
    {
        private MyPagerAdapter adapter;
        private ViewPager pager;
        private PagerSlidingTabStrip tabs;

        protected override int LayoutResource
        {
            get { return Resource.Layout.Activity_MyWorkshopBookings; }
        }
        
        protected async override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            adapter = new MyPagerAdapter(FragmentManager, this.LayoutInflater);
            pager = FindViewById<ViewPager>(Resource.Id.pager);
            tabs = FindViewById<PagerSlidingTabStrip>(Resource.Id.tabs);
            pager.Adapter = adapter;
            tabs.SetViewPager(pager);
 
            ActionBar.SetDisplayHomeAsUpEnabled(true);
            ActionBar.SetDisplayShowHomeEnabled(true);
        }
    }

    class MyPagerAdapter : Android.Support.V13.App.FragmentPagerAdapter
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

        public override Fragment GetItem(int position)
        {
            return TabFragment.NewInstance(position, true);
        }
    }
}