using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.OS;
using TaskStackBuilder = Android.App.TaskStackBuilder;
using helps.Shared.DataObjects;
using Java.Lang;
using AlertDialog = Android.App.AlertDialog;
using Android.Support.V7.App;
using Java.Util;

namespace helps.Droid.Helpers
{
    public static class NotificationHelper
    {
        private static List<NotificationOption> items = new List<NotificationOption>();
        private static WorkshopDetail booking;
        private static Context context;

        public static NotificationOption DefaultNotification = new NotificationOption()
        {
            title = "10 minutes before",
            mins = 10,
            selected = true
        };

        private static void Init()
        {
            if (items.Count == 0)
            {
                items.Add(DefaultNotification);
                items.Add(new NotificationOption() {title = "30 minutes before", mins = 30});
                items.Add(new NotificationOption() {title = "1 hour before", mins = 60});
                items.Add(new NotificationOption() {title = "1 day before", mins = 1440});
                items.Add(new NotificationOption() {title = "1 week before", mins = 10080});
            }
        }

        public static void ShowDialog(Context contxt, WorkshopDetail workshopBooking)
        {
            booking = workshopBooking;
            context = contxt;
            Init();
            AlertDialog.Builder builder = new AlertDialog.Builder(context);
            builder.SetTitle("Set Notifications");
            builder.SetMultiChoiceItems(items.Select(x => x.title).ToArray(), null, new MultiClickListener());
            builder.SetPositiveButton("OK", new ActionClickListener());
            builder.SetNegativeButton("Cancel", new ActionClickListener());
            builder.Create().Show();
        }

        private static void ScheduleNotification(Notification notification, DateTime date)
        {
            DateTime dtBasis = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            AlarmManager alarmManager = (AlarmManager) context.GetSystemService(Context.AlarmService);
            alarmManager.Set(AlarmType.RtcWakeup, (long) date.ToUniversalTime().Subtract(dtBasis).TotalMilliseconds, GetPendingIntent(notification));
        }

        private static PendingIntent GetPendingIntent(Notification notification)
        {
            // Creates an explicit intent for an Activity in your app
            Intent notificationIntent = new Intent(context, typeof(NotificationPublisher));
            notificationIntent.PutExtra(NotificationPublisher.NOTIFICATION_ID, 1);
            notificationIntent.PutExtra(NotificationPublisher.NOTIFICATION, notification);
            return PendingIntent.GetBroadcast(context, 0, notificationIntent, PendingIntentFlags.UpdateCurrent);
        }

        private static Notification GetNotification(int workshopId)
        {
            Notification.Builder mBuilder =
                new Notification.Builder(context)
                .SetSmallIcon(Resource.Drawable.notificationIcon)
                .SetContentTitle(booking.Title)
                .SetContentText(booking.Time + " - " + booking.DateHumanFriendly)
                .SetAutoCancel(true)
                .SetColor(context.Resources.GetColor(Resource.Color.primary))
                .SetDefaults(NotificationDefaults.All)
                .SetStyle(new Notification.BigTextStyle().SetSummaryText(booking.Title).BigText(booking.Time + " - " + booking.DateHumanFriendly + System.Environment.NewLine + booking.Room));

            Intent resultIntent = new Intent(context, typeof(ViewWorkshopActivity));
            resultIntent.PutExtra("WorkshopId", workshopId);
            resultIntent.PutExtra("IsBooking", true);

            TaskStackBuilder stackBuilder = TaskStackBuilder.Create(context);
            stackBuilder.AddParentStack(context.Class);
            stackBuilder.AddNextIntent(resultIntent);
            PendingIntent resultPendingIntent = stackBuilder.GetPendingIntent(0, PendingIntentFlags.UpdateCurrent);
            mBuilder.SetContentIntent(resultPendingIntent);
            return mBuilder.Build();
        }

        public static void ScheduleNotifications(int workshopId, List<NotificationOption> notifications)
        {
            Services.Notification.StoreNotifications(workshopId, notifications);
            Schedule(workshopId, notifications);
        }

        public static void ScheduleNotification(int workshopId, NotificationOption notification)
        {
            var notifications = new List<NotificationOption>();
            notifications.Add(notification);
            Services.Notification.StoreNotifications(workshopId, notifications);
            Schedule(workshopId, notifications);
        }

        public static void ScheduleNotifications(int workshopId)
        {
            Schedule(workshopId, Services.Notification.GetNotifications(workshopId));
        }

        public static void ScheduleAllNotifications()
        {
            var notifications = Services.Notification.GetAll();
            Schedule(notifications.First().workshopId, notifications);
        }

        private static void Schedule(int workshopId, List<NotificationOption> notifications)
        {
            foreach (var notification in notifications)
            {
                var schedule = booking.Date.AddMinutes(notification.mins * -1);
                if(notification.selected)
                    ScheduleNotification(GetNotification(workshopId), schedule);
            }
        }

        private class MultiClickListener : Java.Lang.Object, IDialogInterfaceOnMultiChoiceClickListener
        {
            public void OnClick(IDialogInterface dialog, int which, bool isChecked)
            {
                items[which].selected = isChecked;
            }
        }

        private class ActionClickListener : Java.Lang.Object, IDialogInterfaceOnClickListener
        {
            public void OnClick(IDialogInterface dialog, int which)
            {
                if (which == -1)
                {
                    Services.Notification.StoreNotifications(booking.Id, items);
                    Cancel(booking.Id);
                    foreach (var notification in items)
                    {
                        if (notification.selected)
                        {
                            var schedule = booking.Date.AddMinutes(notification.mins*-1);
                            ScheduleNotification(GetNotification(booking.Id), schedule);
                        }
                    }
                    ViewHelper.CurrentActivity().RunOnUiThread(() =>
                    {
                        ViewWorkshopActivity.UpdateNotifications();
                    });
                }
            }
        }

        public static void Cancel(int workshopId)
        {
            AlarmManager alarmManager = (AlarmManager)context.GetSystemService(Context.AlarmService);
            alarmManager.Cancel(GetPendingIntent(GetNotification(workshopId)));
        }
    }
}