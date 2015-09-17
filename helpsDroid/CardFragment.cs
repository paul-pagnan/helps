using Android.Support.V4.App;
using Android.OS;
using Android.Support.V4.View;


using Android.Widget;
using Android;

namespace helps.Droid
{
    public class CardFragment : Fragment
    {
        private int position;
        public static CardFragment NewInstance(int position)
        {

            var f = new CardFragment();
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
            var root = inflater.Inflate(Resource.Layout.Fragment_CurrentBookings, container, false);
            ViewCompat.SetElevation(root, 50);
            return root;
        }
    }
}