using Android.App;
using Android.Content.Res;
using Android.Views;
using Java.Lang;
using Java.Lang.Reflect;

namespace helps.Droid.Helpers
{
    public static class ViewHelper
    {
        public static Activity CurrentActivity()
        {
            Class activityThreadClass = Class.ForName("android.app.ActivityThread");

            Java.Lang.Object activityThread = activityThreadClass.GetMethod("currentActivityThread").Invoke(null);
            Field activitiesField = activityThreadClass.GetDeclaredField("mActivities");
            activitiesField.Accessible = true;
            Android.Util.ArrayMap activities = (Android.Util.ArrayMap)activitiesField.Get(activityThread);
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

        public static void ToggleMenu(IMenu menu, bool showMenu)
        {
            if (menu == null)
                return;
            menu.SetGroupVisible(Resource.Id.list_actions_group, showMenu);
        }
    }
}