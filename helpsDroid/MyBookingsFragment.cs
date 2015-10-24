using System;
using Android;
using Android.App;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.App;
using Android.Views;
using Android.Widget;
using helps.Droid.Helpers;
using helps.Shared;
using helps.Shared.DataObjects;
using Java.Lang;

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
            this.SetTaskDescription(new ActivityManager.TaskDescription(
                Resources.GetString(Resource.String.app_name),
                BitmapFactory.DecodeResource(Resources, Resource.Drawable.ic_launcher),
                Resources.GetColor(Resource.Color.primary)));

            //Create Exception Handlers
            AppDomain.CurrentDomain.UnhandledException += ExceptionHelper.HandleExceptions;
            AndroidEnvironment.UnhandledExceptionRaiser += HandleAndroidException;

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

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.menu_list_actions, menu);
            return base.OnCreateOptionsMenu(menu);
        }

        void HandleAndroidException(object sender, RaiseThrowableEventArgs e)
        {
            if (e.Exception.GetType() == typeof (System.Net.WebException))
            {
                ExceptionHelper.NoConnection();
                e.Handled = true;
            }
        }
    }
}