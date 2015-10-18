using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.OS;
using Android.Preferences;
using Android.Provider;
using TaskStackBuilder = Android.App.TaskStackBuilder;
using helps.Shared.DataObjects;
using Java.Lang;
using AlertDialog = Android.App.AlertDialog;
using Android.Support.V7.App;
using helps.Droid.Recievers;
using Java.Security;
using Java.Util;
using Java.Util.Concurrent.Atomic;

namespace helps.Droid.Helpers
{
    public class NotificationHelper
    {
        private static List<NotificationOption> items = new List<NotificationOption>();
        private static AtomicInteger counter = new AtomicInteger();

        public static NotificationOption DefaultNotification = new NotificationOption()
        {
            title = "10 minutes before",
            mins = 10,
            selected = true
        };

        public NotificationHelper(WorkshopDetail workshop)
        {
            items = Services.Notification.GetNotifications(workshop.Id);
            if (items.Count == 0)
            {
                items.Clear();
                items.Add(new NotificationOption() { title = "10 minutes before", mins = 10 });
                items.Add(new NotificationOption() { title = "30 minutes before", mins = 30 });
                items.Add(new NotificationOption() { title = "1 hour before", mins = 60 });
                items.Add(new NotificationOption() { title = "1 day before", mins = 1440 });
                items.Add(new NotificationOption() { title = "1 week before", mins = 10080 });
                items.Add(new NotificationOption() { title = "Test Notification", mins = (int)(workshop.Date.AddMinutes(-1) - DateTime.Now).TotalMinutes });
            }
        }

        public void ShowDialog(Context contxt, WorkshopDetail workshopBooking)
        { 
            AlertDialog.Builder builder = new AlertDialog.Builder(contxt);
            builder.SetTitle("Set Notifications");
            builder.SetMultiChoiceItems(items.Select(x => x.title).ToArray(), items.Select(x => x.selected).ToArray(), new MultiClickListener());
            var clickListener = new ActionClickListener(contxt, workshopBooking);
            builder.SetPositiveButton("OK", clickListener);
            builder.SetNegativeButton("Cancel", clickListener);
            builder.Create().Show();
        }

        private static void CreateAlarmManager(Context ctx, Notification notification, DateTime date, NotificationOption not, int workhopId)
        {
            DateTime dtBasis = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            AlarmManager alarmManager = (AlarmManager) ctx.GetSystemService(Context.AlarmService);
            alarmManager.Set(AlarmType.RtcWakeup, (long) date.ToUniversalTime().Subtract(dtBasis).TotalMilliseconds, GetPendingIntent(ctx, notification, not, workhopId));
        }

        private static PendingIntent GetPendingIntent(Context ctx, Notification notification, NotificationOption not, int workshopId)
        {
            // Creates an explicit intent for an Activity in your app
            Intent notificationIntent = new Intent(ctx, typeof(NotificationPublisher));
            int notificationId = Integer.ParseInt(workshopId.ToString() + not.mins);
            notificationIntent.PutExtra(NotificationPublisher.NOTIFICATION_ID, notificationId);
            notificationIntent.PutExtra(NotificationPublisher.NOTIFICATION, notification);
            return PendingIntent.GetBroadcast(ctx, notificationId, notificationIntent, PendingIntentFlags.UpdateCurrent);
        }

        private static Notification GetNotification(Context ctx, int workshopId, NotificationOption not)
        {
            var workshop = Services.Workshop.GetWorkshopFromBookingLocal(workshopId);
            Notification.Builder mBuilder =
                new Notification.Builder(ctx)
                .SetSmallIcon(Resource.Drawable.notificationIcon)
                .SetContentTitle(workshop.Title)
                .SetContentText(workshop.Time + " - " + workshop.DateHumanFriendly)
                .SetAutoCancel(true)
                .SetColor(ctx.Resources.GetColor(Resource.Color.primary))
                .SetDefaults(NotificationDefaults.All)
                .SetStyle(new Notification.BigTextStyle().SetSummaryText(workshop.Title).BigText(workshop.Time + " - " + workshop.DateHumanFriendly + System.Environment.NewLine + workshop.Room));

            Intent resultIntent = new Intent(ctx, new ViewWorkshopActivity().Class);
            resultIntent.PutExtra("WorkshopId", workshopId);
            resultIntent.PutExtra("IsBooking", true);

            TaskStackBuilder stackBuilder = TaskStackBuilder.Create(ctx);
            stackBuilder.AddParentStack(new ViewWorkshopActivity().Class);
            stackBuilder.AddNextIntent(resultIntent);
            int notificationId = Integer.ParseInt(workshopId.ToString() + not.mins);
            PendingIntent resultPendingIntent = stackBuilder.GetPendingIntent(notificationId, PendingIntentFlags.UpdateCurrent);
            mBuilder.SetContentIntent(resultPendingIntent);
            return mBuilder.Build();
        }

        public static void ScheduleNotification(Context ctx, int workshopId, NotificationOption notification)
        {
            var notifications = new List<NotificationOption>();
            notifications.Add(notification);
            Services.Notification.StoreNotifications(Services.Workshop.GetWorkshop(workshopId), notifications);
            Schedule(ctx, workshopId, notifications);
        }

        public static void ScheduleNotifications(Context ctx, int workshopId)
        {
            Schedule(ctx, workshopId, Services.Notification.GetNotifications(workshopId));
        }

        public static void ScheduleAllNotifications(Context ctx)
        {
            var notifications = Services.Notification.GetAll();
            Console.Out.WriteLine("HELPS: Notifications Found - " + notifications.Count);
            Schedule(ctx, notifications.First().workshopId, notifications);
        }

        private static void Schedule(Context ctx, int workshopId, List<NotificationOption> notifications)
        {
            foreach (var notification in notifications.Where(x => x.selected))
                CreateAlarmManager(ctx, GetNotification(ctx, workshopId, notification), notification.ScheduledDate, notification, workshopId);
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
            private WorkshopDetail workshop;
            private Context ctx;

            public ActionClickListener(Context ctx, WorkshopDetail workshop) : base()
            {
                this.workshop = workshop;
                this.ctx = ctx;
            }

            public void OnClick(IDialogInterface dialog, int which)
            {
                if (which == -1)
                {
                    Cancel(ctx, workshop.Id);
                    Services.Notification.StoreNotifications(workshop, items);
                    ScheduleNotifications(ctx, workshop.Id);
                    ViewHelper.CurrentActivity().RunOnUiThread(() =>
                    {
                        ViewWorkshopActivity.UpdateNotifications();
                    });
                }
            }
        }

        public static void Cancel(Context ctx, int workshopId)
        {
            foreach (var notification in Services.Notification.GetNotifications(workshopId))
            {
                AlarmManager alarmManager = (AlarmManager)ctx.GetSystemService(Context.AlarmService);
                alarmManager.Cancel(GetPendingIntent(ctx, GetNotification(ctx, workshopId, notification), notification, workshopId));
            }
            Services.Notification.Clear(workshopId);
        }
    }
}