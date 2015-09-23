using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
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
            Init();
            base.OnCreate(bundle);

            //Create Exception Handlers
            AppDomain.CurrentDomain.UnhandledException += HandleExceptions;
            //AndroidEnvironment.UnhandledExceptionRaiser += HandleAndroidException;
            Thread.DefaultUncaughtExceptionHandler = new ThreadExceptionHandler();

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

        public void Logout()
        {
            Services.Auth.Logout();
            CurrentUser = null;
            var intent = new Intent(this, typeof(SignInActivity));
            StartActivity(intent);
            Finish();
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            if (item.ItemId == Resource.Id.menu_logout)
                Logout();
            return base.OnOptionsItemSelected(item);
        }


        //Exception Handlers
        void HandleExceptions(object sender, UnhandledExceptionEventArgs e)
        {
            var Ex = e.ExceptionObject;
            try {
                AggregateException ParentEx = (AggregateException)e.ExceptionObject;
                var exceptions = ParentEx.InnerExceptions;
                Ex = exceptions.FirstOrDefault();
            } catch(System.Exception ex) {  }
               
            if (Ex.GetType() == typeof(System.Net.WebException))
            {
                var context = CurrentActivity();
                HelpsService.CurrentlyUpdating = false;
                context.RunOnUiThread(delegate
                {
                    var NoConnection = context.FindViewById(Resource.Id.NoConnection);
                    if (NoConnection != null)
                    {
                        NoConnection.Visibility = ViewStates.Visible;

                        new Handler().PostDelayed(delegate
                        {
                            Android.Views.Animations.Animation fadeOut = new AlphaAnimation(1, 0);
                            fadeOut.Interpolator = new DecelerateInterpolator(); 
                            fadeOut.Duration = 1000;

                            AnimationSet animation = new AnimationSet(false);
                            animation.AddAnimation(fadeOut);
                            NoConnection.Animation = animation;
                            NoConnection.StartAnimation(animation);
                            new Handler().PostDelayed(delegate { NoConnection.Visibility = ViewStates.Gone; }, 1000);
                        }, 2000);
                    }
                });
            }
        }
        
        void HandleAndroidException(object sender, RaiseThrowableEventArgs e)
        {
            e.Handled = true;
        }


        public static Activity CurrentActivity()
        {
            Class activityThreadClass = Class.ForName("android.app.ActivityThread");

            Java.Lang.Object activityThread = activityThreadClass.GetMethod("currentActivityThread").Invoke(null);
            Field activitiesField = activityThreadClass.GetDeclaredField("mActivities");
            activitiesField.Accessible = true;
            Android.Util.ArrayMap activities = (Android.Util.ArrayMap) activitiesField.Get(activityThread);
            foreach (Java.Lang.Object activityRecord in activities.Values())
            {
                Class activityRecordClass = activityRecord.Class;
                Field pausedField = activityRecordClass.GetDeclaredField("paused");
                pausedField.Accessible = true;
                if (!pausedField.GetBoolean(activityRecord))
                {
                    Field activityField = activityRecordClass.GetDeclaredField("activity");
                    activityField.Accessible = true;
                    Activity activity = (Activity)activityField.Get(activityRecord);
                    return activity;
                }
            }

            return null;
        }


        public class ThreadExceptionHandler : Java.Lang.Object, Java.Lang.Thread.IUncaughtExceptionHandler
        {
            public void UncaughtException(Thread thread, Throwable ex)
            {
                Console.WriteLine("ERROR");
            }
        }
    }
}