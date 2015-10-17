﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Views.Animations;
using Android.Widget;
using helps.Shared;
using Java.Lang;
using Java.Lang.Reflect;
using Java.Nio.Channels;

namespace helps.Droid.Helpers
{
    public static class ExceptionHelper
    {

        //Exception Handlers
        public static void HandleExceptions(object sender, UnhandledExceptionEventArgs e)
        {
            if (e.ExceptionObject.GetType() == typeof (System.Net.WebException))
                NoConnection();
        }

        public static void NoConnection()
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
                    Activity activity = (Activity) activityField.Get(activityRecord);
                    return activity;
                }
            }
            return null;
        }
    }
}