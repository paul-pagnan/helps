using Android.OS;

using Android.App;
using Android.Widget;
using Android;
using helps.Droid.Adapters;
using Android.Support.V4.View;

namespace helps.Droid
{
    public class TabFragment : Android.Support.V4.App.Fragment
    {
        private int position;
        private Activity activity;
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

            var bookingsAdapter = new BookingsListAdapter(inflater, Resources, true);
            var bookingsListView = root.FindViewById<ListView>(GetListView());
            bookingsListView.Adapter = bookingsAdapter;
            //Add data to the list


            ViewCompat.SetElevation(root, 50);
            return root;
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
    }

}