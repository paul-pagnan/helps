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
using helps.Shared;
using helps.Shared.DataObjects;
using helps.Droid.Helpers;
using helps.Droid.Adapters;
using Android.Support.V4.Widget;
using System.Threading.Tasks;
using System.Threading;
using Java.Lang;

namespace helps.Droid
{ 
    [Activity(Label = "Workshops", Theme = "@style/AppTheme.MyToolbar")]
    public class MakeBookingActivity : Main
    {
        private ViewFlipper viewFlipper;
        private SwipeRefreshLayout categoryRefresher;
        private SwipeRefreshLayout workshopRefresher;

        private ListView workshopSetListView;
        private WorkshopCategoryListAdapter workshopSetListAdapter;

        protected override int LayoutResource
        {
            get { return Resource.Layout.Activity_MakeBooking; }
        }

        protected async override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            viewFlipper = FindViewById<ViewFlipper>(Resource.Id.bookingflipper);
            ActionBar.SetDisplayHomeAsUpEnabled(true);
            ActionBar.SetDisplayShowHomeEnabled(true);

            InitRefreshers();
            InitLists();
            //Get Local Data First, then update later
            await Task.Factory.StartNew(() => LoadData(true));
            FindViewById<ProgressBar>(Resource.Id.workshopSetLoading).Visibility = ViewStates.Gone;
            FindViewById<ProgressBar>(Resource.Id.workshopLoading).Visibility = ViewStates.Gone;
            NotifyListUpdate();
            await Task.Factory.StartNew(() => BackgroundRefresh());
            NotifyListUpdate();
        }

        private void InitRefreshers()
        {
            categoryRefresher = FindViewById<SwipeRefreshLayout>(Resource.Id.swipe1);
            categoryRefresher.Refresh += async (sender, e) =>
            {
                await Task.Factory.StartNew(() => LoadData(false, true));
                NotifyListUpdate();
                categoryRefresher.Refreshing = false;
            };

            workshopRefresher = FindViewById<SwipeRefreshLayout>(Resource.Id.swipe2);
            workshopRefresher.Refreshing = true;
            workshopRefresher.Refresh +=  (sender, e) =>
            {
                workshopRefresher.Refreshing = false;
            };
        }

        private void InitLists()
        {
            workshopSetListAdapter = new WorkshopCategoryListAdapter(this.LayoutInflater);
            workshopSetListView = FindViewById<ListView>(Resource.Id.workshopSetList);
            workshopSetListView.Adapter = workshopSetListAdapter;
        }

        private async void LoadData(bool localOnly, bool force = false)
        {
            workshopSetListAdapter.AddAll(await Services.Workshop.GetWorkshopSets(localOnly, force));
        }

        private void NotifyListUpdate()
        {
            workshopSetListAdapter.NotifyDataSetChanged();
            workshopRefresher.Refreshing = false;

            workshopSetListView.ItemClick += (sender, e) =>
            {
                InitWorkshopList(e.Id);
            };
        }

        private async void BackgroundRefresh()
        {
            var list = await Services.Workshop.GetWorkshopSets(false, false);
            workshopSetListAdapter.Clear();
            workshopSetListAdapter.AddAll(list);
        }

        private void InitWorkshopList(long id)
        {
            FlipView();
            var bookingsAdapter = new BookingsListAdapter(this.LayoutInflater, Resources, false);
            var bookingsListView = FindViewById<ListView>(Resource.Id.workshopList);
            bookingsListView.Adapter = bookingsAdapter;
        }

        public override void OnBackPressed()
        {
            if (Back())
                return;
            base.OnBackPressed();
        }

      
        public bool Back()
        {
            if (viewFlipper.DisplayedChild > 0)
                return FlipView();

            return false;
        }

        public bool FlipView()
        {
            int index = viewFlipper.DisplayedChild;
            int InAnimation = (index > 0) ? Resource.Animation.slide_in_from_left : Resource.Animation.slide_in_from_right;
            int OutAnimation = (index > 0) ? Resource.Animation.slide_out_to_right : Resource.Animation.slide_out_to_left;
                        
            viewFlipper.SetInAnimation(this, InAnimation);
            viewFlipper.SetOutAnimation(this, OutAnimation);
            if (index > 0)
                viewFlipper.ShowPrevious();
            else
                viewFlipper.ShowNext();
            return true;
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            return base.OnOptionsItemSelected(item);
        }
    }
}