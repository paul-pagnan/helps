using Android.OS;

using Android.App;
using Android.Widget;
using Android;
using helps.Droid.Adapters;
using Android.Support.V4.View;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using Android.Support.V4.Widget;
using Android.Views;
using Android.Content;
using helps.Droid.Helpers;
using helps.Shared.DataObjects;

namespace helps.Droid
{
    public class BookingsTabFragment : Fragment
    {
        private int position;
        private bool isWorkshop;
        private SwipeRefreshLayout refresher;
        private ListView listView;
        private BookingsListAdapter listAdapter;
        private View root;
        private LayoutInflater inflater;
        private readonly Activity activity = ViewHelper.CurrentActivity();
        private List<WorkshopPreview> list;
        private ListActionHelper listActioner;

        public static BookingsTabFragment NewInstance(int position, bool workshop)
        {
            var f = new BookingsTabFragment();
            var b = new Bundle();
            b.PutInt("position", position);
            b.PutBoolean("isWorkshop", workshop);
            f.Arguments = b;
            return f;
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            position = Arguments.GetInt("position");
            isWorkshop = Arguments.GetBoolean("isWorkshop");
            SetHasOptionsMenu(true);
        }

        public override bool UserVisibleHint
        {
            set
            {
                if (position > 0 && listAdapter.IsEmpty)
                {
                    var activity = ViewHelper.CurrentActivity();
                    activity.RunOnUiThread(() =>
                    {
                        activity.FindViewById<ProgressBar>(loadingView()).Visibility = ViewStates.Visible;
                        activity.FindViewById<TextView>(noBookingsView()).Visibility = ViewStates.Gone;
                        activity.FindViewById<RelativeLayout>(centerWrapView()).Visibility = ViewStates.Visible;
                    });
                    InitList(root, inflater);
                }
            }
        }

        public override Android.Views.View OnCreateView(Android.Views.LayoutInflater inflater, Android.Views.ViewGroup container, Bundle savedInstanceState)
        {
            var root = inflater.Inflate(GetLayout(), container, false);
            this.root = root;
            this.inflater = inflater;

            InitElements(root, inflater);
            if(position < 1 && listAdapter.IsEmpty)
                InitList(root, inflater);

            ViewCompat.SetElevation(root, 50);

            listView.ItemClick += (sender, e) =>
            {
                var intent = new Intent(container.Context, typeof(ViewSessionActivity));
                if (isWorkshop)
                    intent = new Intent(container.Context, typeof(ViewWorkshopActivity));

                intent.PutExtra("Id", (int)e.Id);
                intent.PutExtra("IsBooking", true);
                StartActivity(intent);
            };
            return root;
        }


        private void InitElements(Android.Views.View context, Android.Views.LayoutInflater inflater)
        {
            listAdapter = new BookingsListAdapter(inflater, Resources, true);
            listView = context.FindViewById<ListView>(GetListView());
            listView.Adapter = listAdapter;

            refresher = context.FindViewById<SwipeRefreshLayout>(Resource.Id.swipeRefresh);
            refresher.Refresh += async (sender, e) =>
            {
                await Task.Factory.StartNew(() => LoadData(false, true));
                NotifyListUpdate();
                refresher.Refreshing = false;
            };
        }

        private async void LoadData(bool localOnly, bool force = false)
        {
            try
            {
                list = await GetData(localOnly, force);
                listAdapter.Clear();
                listAdapter.AddAll(list);
                NotifyListUpdate();
                PostListUpdateView(localOnly, list.Count == 0);
            }
            catch (Exception ex)
            {
                PostListUpdateView(localOnly, true);
                throw ex;
            }
        }

        private async Task<List<WorkshopPreview>> GetData(bool localOnly, bool force)
        {
            if(isWorkshop)
                return await Services.Workshop.GetBookings(PastOrCurrent(), localOnly, force);
            return await Services.Session.GetBookings(PastOrCurrent(), localOnly, force);
        }

        private void PostListUpdateView(bool localOnly, bool listEmpty)
        {
            activity.RunOnUiThread(() =>
            {
                var notLoading = (!listEmpty || !localOnly);
                var noBookings = (!localOnly && listEmpty);
                activity.FindViewById<ProgressBar>(loadingView()).Visibility = notLoading
                    ? ViewStates.Gone
                    : ViewStates.Visible;
                activity.FindViewById<TextView>(noBookingsView()).Visibility = noBookings
                    ? ViewStates.Visible
                    : ViewStates.Gone;
                activity.FindViewById<RelativeLayout>(centerWrapView()).Visibility = (notLoading && noBookings)
                   ? ViewStates.Visible
                   : ViewStates.Gone;
            });  
        }

        private int loadingView()
        {
            return PastOrCurrent() ? Resource.Id.loading : Resource.Id.loadingPast;
        }
        private int centerWrapView()
        {
            return PastOrCurrent() ? Resource.Id.centerWrap : Resource.Id.centerWrapPast;
        }

        private int noBookingsView()
        {
            return PastOrCurrent() ? Resource.Id.noBookings : Resource.Id.noBookingsPast;
        }


        private void NotifyListUpdate()
        {
            activity.RunOnUiThread(() =>
            {
                listAdapter.NotifyDataSetChanged();
                refresher.Refreshing = false;
            });
        }

        private async void InitList(Android.Views.View context, Android.Views.LayoutInflater inflater)
        {
            //Load from local first
            await Task.Factory.StartNew(() => LoadData(true));
            listActioner = new ListActionHelper(listAdapter, listView, activity, isWorkshop);
            //Do background refresh
            await Task.Factory.StartNew(() => LoadData(false));
        }

        private int GetLayout()
        {
            switch (position) {
                case 0:
                    return Resource.Layout.Fragment_CurrentBookings;
                case 1: return Resource.Layout.Fragment_PastBookings;
                default: return -1;
            }
        }

        private int GetListView()
        {
            switch (position)
            {
                case 0: return Resource.Id.currentBookingsList;
                case 1: return Resource.Id.pastBookingsList;
                default: return -1;
            }
        }

        private bool PastOrCurrent()
        {
            return (position == 0) ? true : false;
        }


        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            if (listActioner != null)
            {
                if (item.ItemId == Resource.Id.menu_filter)
                    listActioner.Filter(list, activity.LayoutInflater, activity);
                else if (item.ItemId == Resource.Id.menu_sort)
                    listActioner.Sort(list);
            }
            else
                Toast.MakeText(activity, "Please wait for the list to load first", ToastLength.Short);
            return base.OnOptionsItemSelected(item);
        }
    }
}