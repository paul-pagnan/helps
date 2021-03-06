using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Xamarin.Forms;
using Android.Widget;
using helps.Shared;
using helps.Shared.DataObjects;
using Java.Lang;
using helps.Droid.Helpers;
using Java.Lang.Reflect;
using Java.Util;
using Android.Views.Animations;

namespace helps.Droid
{
    public abstract class Main : Activity
    {
        public User CurrentUser;

        public Toolbar Toolbar { get; set; }
        protected static CancellationTokenSource cts = new CancellationTokenSource();
        protected abstract int LayoutResource { get; }


        public Main()
        {
            Init();
        }

        public void Init()
        {
            if(Forms.IsInitialized)
                CurrentUser = Services.Auth.CurrentUser();
        }

        protected override void OnCreate(Bundle bundle)
        {
            if (!Forms.IsInitialized)
                Xamarin.Forms.Forms.Init(this, bundle);
            base.OnCreate(bundle);
            Init();

            this.SetTaskDescription(new ActivityManager.TaskDescription(
                Resources.GetString(Resource.String.app_name), 
                BitmapFactory.DecodeResource(Resources, Resource.Drawable.ic_launcher),
                Resources.GetColor(Resource.Color.primary)));

            //Create Exception Handlers
            AppDomain.CurrentDomain.UnhandledException += ExceptionHelper.HandleExceptions;

            SetContentView(LayoutResource);
            Toolbar = FindViewById<Toolbar>(Resource.Id.Ttoolbar);
            if (Toolbar == null)
                Toolbar = FindViewById<Toolbar>(Resource.Id.TtoolbarTransparent);
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

        public void setPadding(Toolbar toolbar)
        {
            toolbar.SetPadding(0, getStatusBarHeight(), 0, 0);
        }

        public int getStatusBarHeight()
        {
            int resourceId = Resources.GetIdentifier("status_bar_height", "dimen", "android");
            if (resourceId > 0)
                return Resources.GetDimensionPixelSize(resourceId);
            return 0;
        }

    }
}