using Android.OS;

using Android.App;
using Android.Widget;
using Android;
using helps.Droid.Adapters;
using Android.Support.V4.View;
using System.Threading.Tasks;
using System;
using Android.Support.V4.Widget;
using Android.Views;

namespace helps.Droid
{
    public class TabFragment : Android.Support.V4.App.Fragment
    {
        private int position;
        private Activity activity;

        private SwipeRefreshLayout refresher;

        private ListView list;
        private BookingsListAdapter listAdapter;

        public static TabFragment NewInstance(int position)
        {
            var f = new TabFragment();
            var b = new Bundle();
            b.PutInt("position", position);
            f.Arguments = b;
            return f;
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            position = Arguments.GetInt("position");
        }

        public override Android.Views.View OnCreateView(Android.Views.LayoutInflater inflater, Android.Views.ViewGroup container, Bundle savedInstanceState)
        {
            var root = inflater.Inflate(GetLayout(), container, false);

            InitElements(root, inflater);
            InitList(root, inflater);

            ViewCompat.SetElevation(root, 50);
            return root;
        }

        private void InitElements(Android.Views.View context, Android.Views.LayoutInflater inflater)
        {
            listAdapter = new BookingsListAdapter(inflater, Resources, true);
            list = context.FindViewById<ListView>(GetListView());
            list.Adapter = listAdapter;

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
            if (position == 0)
                listAdapter.AddAll(await Services.Workshop.GetBookings(PastOrCurrent(), localOnly, force));
            else if (!force)
            {
                Looper.Prepare();
                new Handler().PostDelayed(async () =>
                {
                    listAdapter.AddAll(await Services.Workshop.GetBookings(PastOrCurrent(), localOnly, force));
                    NotifyListUpdate();
                }, 1000);
            } else
            {
                listAdapter.AddAll(await Services.Workshop.GetBookings(PastOrCurrent(), localOnly, force));
            }
        }

        private void NotifyListUpdate()
        {
            listAdapter.NotifyDataSetChanged();
            refresher.Refreshing = false;

            list.ItemClick += (sender, e) =>
            {
                //Show Booking
            };
        }

        private async void InitList(Android.Views.View context, Android.Views.LayoutInflater inflater)
        {
            await Task.Factory.StartNew(() => LoadData(true));
            context.FindViewById<ProgressBar>(Resource.Id.loading).Visibility = ViewStates.Gone;
            NotifyListUpdate();
            //await Task.Factory.StartNew(() => BackgroundRefresh());
            //NotifyListUpdate();
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

        private async void BackgroundRefresh()
        {
            var tempList = await Services.Workshop.GetBookings(PastOrCurrent(), false, false);
            listAdapter.Clear();
            listAdapter.AddAll(tempList);
        }
    }
}