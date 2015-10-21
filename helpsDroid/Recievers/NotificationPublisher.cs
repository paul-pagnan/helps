using System;
using Android.App;
using Android.Content;
using helps.Shared;

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
            if (!SettingService.notificationsEnbabled())
                return;

            var notificationManager =
                (NotificationManager) context.GetSystemService(Context.NotificationService);

            var notification = (Notification) intent.GetParcelableExtra(NOTIFICATION);
            var id = intent.GetIntExtra(NOTIFICATION_ID, 0);
            notificationManager.Notify(id, notification);
        }
    }
}