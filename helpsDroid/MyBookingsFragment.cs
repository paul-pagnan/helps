using Android;
using Android.App;
using Android.OS;
using Android.Support.V4.App;
using Android.Views;
using Android.Widget;
using helps.Shared;
using helps.Shared.DataObjects;

namespace helps.Droid
{
    public abstract class MyBookingsFragment : FragmentActivity
    {
        public Toolbar Toolbar { get; set; }

        protected abstract int LayoutResource { get; }

        public MyBookingsFragment()
        {

        }


        public void setPadding(Toolbar toolbar)
        {
            // Set the padding to match the Status Bar height
            toolbar.SetPadding(0, getStatusBarHeight(), 0, 0);
        }

        public int getStatusBarHeight()
        {
            int result = 0;

            int resourceId = Resources.GetIdentifier("status_bar_height", "dimen", "android");
            if (resourceId > 0)
            {
                result = Resources.GetDimensionPixelSize(resourceId);
            }
            return result;
        }

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(LayoutResource);
            Toolbar = FindViewById<Toolbar>(Resource.Id.Ttoolbar);
            if (Toolbar != null)
            {
                setPadding(Toolbar);
                SetActionBar(Toolbar);
                Toolbar.NavigationOnClick += (sender, e) =>
                {
                    Finish();
                };
            }
        }

        public bool OnOptionsItemSelected(IMenuItem item)
        {
            return base.OnOptionsItemSelected(item);
        }
    }
}