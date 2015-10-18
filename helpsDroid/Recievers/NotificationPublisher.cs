using System;
using Android.App;
using Android.Content;

namespace helps.Droid.Recievers
{
    [BroadcastReceiver(Enabled = true)]
    public class NotificationPublisher : BroadcastReceiver
    {

        public static string NOTIFICATION_ID = "notification-id";
        public static string NOTIFICATION = "notification";

        public override void OnReceive(Context context, Intent intent)
        {
            Console.Out.WriteLine("HELPS: Spawning Notification");
            NotificationManager notificationManager =
                (NotificationManager) context.GetSystemService(Context.NotificationService);

            Notification notification = (Notification) intent.GetParcelableExtra(NOTIFICATION);
            int id = intent.GetIntExtra(NOTIFICATION_ID, 0);
            notificationManager.Notify(id, notification);
        }
    }
}