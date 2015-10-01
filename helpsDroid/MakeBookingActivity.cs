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

        private ListView workshopListView;
        private BookingsListAdapter workshopListAdapter;

        private int CurrentWorkshopSet;


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

            workshopSetListView.ItemClick += (sender, e) =>
            {
                FindViewById<TextView>(Resource.Id.noWorkshops).Visibility = ViewStates.Gone;
                FindViewById<ProgressBar>(Resource.Id.workshopLoading).Visibility = ViewStates.Visible;
                InitWorkshopList((int)e.Id);
            };

            workshopListView.ItemClick += (sender, e) =>
            {
                ViewWorkshop((int)e.Id);
            };


            FindViewById<RelativeLayout>(Resource.Id.mainLayout).Invalidate();
            //Get Local Data First, then update later
            await Task.Factory.StartNew(() => LoadSets(true));
            //Do a background Sync now
            await Task.Factory.StartNew(() => LoadSets(false, false));
            NotifyListUpdate();
        }

        private void ViewWorkshop(int id)
        {
            var intent = new Intent(ApplicationContext, typeof(ViewWorkshopActivity));
            intent.PutExtra("WorkshopId", id);
            StartActivity(intent);
        }

        private void InitRefreshers()
        {
            categoryRefresher = FindViewById<SwipeRefreshLayout>(Resource.Id.swipe1);
            categoryRefresher.Refresh += async (sender, e) =>
            {
                await Task.Factory.StartNew(() => LoadSets(false, true));
                NotifyListUpdate();
                categoryRefresher.Refreshing = false;
            };

            workshopRefresher = FindViewById<SwipeRefreshLayout>(Resource.Id.swipe2);
            workshopRefresher.Refreshing = true;
            workshopRefresher.Refresh +=  async (sender, e) =>
            {
                await Task.Factory.StartNew(() => LoadWorkshops(CurrentWorkshopSet, false, true));
                NotifyListUpdate();
                workshopRefresher.Refreshing = false;
            };
        }

        private void InitLists()
        {
            workshopSetListAdapter = new WorkshopCategoryListAdapter(this.LayoutInflater);
            workshopSetListView = FindViewById<ListView>(Resource.Id.workshopSetList);
            workshopSetListView.Adapter = workshopSetListAdapter;

            workshopListAdapter = new BookingsListAdapter(this.LayoutInflater, Resources, false);
            workshopListView = FindViewById<ListView>(Resource.Id.workshopList);
            workshopListView.Adapter = workshopListAdapter;
        }

        private async void LoadSets(bool localOnly, bool force = false)
        {
            var list = await Services.Workshop.GetWorkshopSets(localOnly, force);
            workshopSetListAdapter.Clear();
            workshopSetListAdapter.AddAll(list);
            if (workshopSetListAdapter.Count > 0)
            {
                RunOnUiThread(delegate
                {
                    FindViewById<ProgressBar>(Resource.Id.workshopSetLoading).Visibility = ViewStates.Gone;
                    NotifyListUpdate();
                });
            }
        }

        private async void LoadWorkshops(int workshopSet, bool localOnly, bool force = false)
        {
            if (CurrentWorkshopSet > 0)
            {
                var list = await Services.Workshop.GetWorkshops(workshopSet, localOnly, force);
                workshopListAdapter.Clear();
                workshopListAdapter.AddAll(list);
                
                RunOnUiThread(delegate
                {
                    if (workshopListAdapter.Count > 0)
                    {
                        FindViewById<ProgressBar>(Resource.Id.workshopLoading).Visibility = ViewStates.Gone;
                        NotifyWorkshopListUpdate();
                    } else if (!localOnly)
                    {
                        FindViewById<ProgressBar>(Resource.Id.workshopLoading).Visibility = ViewStates.Gone;
                        FindViewById<TextView>(Resource.Id.noWorkshops).Visibility = ViewStates.Visible;
                    }
                });               
            }
        }

        private void NotifyListUpdate()
        {
            workshopSetListAdapter.NotifyDataSetChanged();
            categoryRefresher.Refreshing = false;
        }

        private void NotifyWorkshopListUpdate()
        {
            workshopListAdapter.NotifyDataSetChanged();
            workshopRefresher.Refreshing = false;
        }

        private async void InitWorkshopList(int id)
        {
            CurrentWorkshopSet = id;
            FlipView();

            //Load from local first
            await Task.Factory.StartNew(() => LoadWorkshops(id, true));
            //Do a background Sync now
            await Task.Factory.StartNew(() => LoadWorkshops(id, false, false));
        }

        public override void OnBackPressed()
        {
            if (Back())
                return;
            base.OnBackPressed();
        }
      
        public bool Back()
        {
            workshopListAdapter.Clear();
            NotifyListUpdate();
            CurrentWorkshopSet = 0;
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