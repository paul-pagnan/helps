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
using Android.Content;

namespace helps.Droid
{
    public class TabFragment : Fragment
    {
        private int position;

        private SwipeRefreshLayout refresher;

        private ListView list;
        private BookingsListAdapter listAdapter;

        private View root;
        private LayoutInflater inflater;

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

        public override bool UserVisibleHint
        {
            set
            {
                if (position > 0 && listAdapter.IsEmpty)
                    InitList(root, inflater);
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

            list.ItemClick += (sender, e) =>
            {
                var intent = new Intent(container.Context, typeof(ViewWorkshopActivity));
                intent.PutExtra("WorkshopId", (int)e.Id);
                intent.PutExtra("IsBooking", true);
                StartActivity(intent);
            };

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
            var list = await Services.Workshop.GetBookings(PastOrCurrent(), localOnly, force);
            listAdapter.Clear();
            listAdapter.AddAll(list);
        }

        private void NotifyListUpdate()
        {
            listAdapter.NotifyDataSetChanged();
            refresher.Refreshing = false;
        }

        private async void InitList(Android.Views.View context, Android.Views.LayoutInflater inflater)
        {
            await Task.Factory.StartNew(() => LoadData(true));
            NotifyListUpdate();
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

    }
}