using System;
using Android.App;
using Android.Content;
using Android.OS;
using helps.Droid.Helpers;
using helps.Shared;
using helps.Shared.Database;

namespace helps.Droid.Recievers
{
    [BroadcastReceiver(Enabled = true)]
    [IntentFilter(new[] { Android.Content.Intent.ActionBootCompleted })]
    public class BootReceiver : BroadcastReceiver
    {
        public override void OnReceive(Context context, Intent intent)
        {
            Console.Out.WriteLine("HELPS: Boot captured, scheduling notifications");
            helpsDatabase.Database = new SQLite_Android().GetConnection();
            
            NotificationHelper.ScheduleAllNotifications(context);
            Console.Out.WriteLine("HELPS: Done scheduling notifications");
        }
    }
}