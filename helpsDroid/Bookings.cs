using Android;
using Android.OS;
using Android.Support.V7.App;
using Android.Support.V7.Widget;

namespace helps.Droid
{
    public abstract class Bookings : AppCompatActivity
    {
        public Toolbar Toolbar { get; set; }

        protected abstract int LayoutResource { get; }

        protected int ActionBarIcon
        {
            set { Toolbar.SetNavigationIcon(value); }
        }

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(LayoutResource);
            Toolbar = FindViewById<Toolbar>(Resource.Id.Ttoolbar);
            if (Toolbar != null)
            {
                SetSupportActionBar(Toolbar);

                SupportActionBar.SetDisplayHomeAsUpEnabled(true);
                SupportActionBar.SetHomeButtonEnabled(true);
            }
        }
    }
}