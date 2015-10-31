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
using helps.Shared;
using Java.Security;
using Java.Util;
using Java.Util.Concurrent.Atomic;

namespace helps.Droid.Helpers
{
    public class NotificationHelper
    {
        private static List<NotificationOption> items = new List<NotificationOption>();
        private static AtomicInteger counter = new AtomicInteger();

        public NotificationHelper(int id, bool isWorkshop)
        {
            items = Services.Notification.GetNotifications(id);
            if (items.Count == 0)
            {
                items = (isWorkshop) ? 
                    NotificationService.DefaultWorkshopNotifications : 
                    NotificationService.DefaultSessionNotifications;
            }
        }

        public NotificationHelper()
        {
        }

        public void ShowFontSize(Context cntext)
        {
            AlertDialog.Builder builder = new AlertDialog.Builder(cntext);
            builder.SetTitle("Change Font Size");
            builder.Create().Show();
        }

        public void ShowDialog(Context contxt, int id, DateTime sessionDate, bool isWorkshop)
        {
            AlertDialog.Builder builder = new AlertDialog.Builder(contxt);
            builder.SetTitle("Set Notifications");
            builder.SetMultiChoiceItems(items.Select(x => x.title).ToArray(), items.Select(x => x.selected).ToArray(), new MultiClickListener());
            var clickListener = new ActionClickListener(contxt, id, sessionDate, isWorkshop);
            builder.SetPositiveButton("OK", clickListener);
            builder.SetNegativeButton("Cancel", clickListener);
            builder.Create().Show();
        }

        private static void CreateAlarmManager(Context ctx, Notification notification, DateTime date, NotificationOption not, int sessionId)
        {
            DateTime dtBasis = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            AlarmManager alarmManager = (AlarmManager) ctx.GetSystemService(Context.AlarmService);
            alarmManager.Set(AlarmType.RtcWakeup, (long) date.ToUniversalTime().Subtract(dtBasis).TotalMilliseconds, GetPendingIntent(ctx, notification, not, sessionId));
        }

        private static PendingIntent GetPendingIntent(Context ctx, Notification notification, NotificationOption not, int sessionId)
        {
            // Creates an explicit intent for an Activity in your app
            Intent notificationIntent = new Intent(ctx, typeof(NotificationPublisher));
            int identifier = (not.isWorkshop) ? 1 : 0;
            int notificationId = Integer.ParseInt(identifier + sessionId.ToString() + not.mins);
            notificationIntent.PutExtra(NotificationPublisher.NOTIFICATION_ID, notificationId);
            notificationIntent.PutExtra(NotificationPublisher.NOTIFICATION, notification);
            return PendingIntent.GetBroadcast(ctx, notificationId, notificationIntent, PendingIntentFlags.UpdateCurrent);
        }

        private static Notification GetNotification(Context ctx, int sessionId, NotificationOption not)
        {
            WorkshopDetail session = null;
            if (not.isWorkshop)
                session = Services.Workshop.GetWorkshopFromBookingLocal(sessionId);
            else
                session = Services.Session.GetSession(sessionId);

            var prefix = (not.isWorkshop) ? "" : "Session with ";
            Notification.Builder mBuilder =
                new Notification.Builder(ctx)
                    .SetSmallIcon(Resource.Drawable.notificationIcon)
                    .SetContentTitle(prefix + session.Title)
                    .SetContentText(session.Time + " - " + session.DateHumanFriendly)
                    .SetAutoCancel(true)
                    .SetColor(ctx.Resources.GetColor(Resource.Color.primary))
                    .SetDefaults(NotificationDefaults.All)
                    .SetStyle(
                        new Notification.BigTextStyle().SetSummaryText(session.Title)
                            .BigText(session.Time + " - " + session.DateHumanFriendly + System.Environment.NewLine +
                                     session.Room));
            try
            {
                Looper.Prepare();
            }
            catch (System.Exception ex) { }

            Intent resultIntent = new Intent(ctx, new ViewSessionActivity().Class);
            if (not.isWorkshop)
                resultIntent = new Intent(ctx, new ViewWorkshopActivity().Class);
            resultIntent.PutExtra("Id", sessionId);
            resultIntent.PutExtra("IsBooking", true);

            TaskStackBuilder stackBuilder = TaskStackBuilder.Create(ctx);
            stackBuilder.AddParentStack(new ViewWorkshopActivity().Class);
            stackBuilder.AddNextIntent(resultIntent);
            int identifier = (not.isWorkshop) ? 1 : 0;
            int notificationId = Integer.ParseInt(identifier + sessionId.ToString() + not.mins);
            PendingIntent resultPendingIntent = stackBuilder.GetPendingIntent(notificationId, PendingIntentFlags.UpdateCurrent);
            mBuilder.SetContentIntent(resultPendingIntent);
            return mBuilder.Build();
        }

        public static void ScheduleNotifications(Context ctx, int sessionId, bool isWorkshop)
        {
            Schedule(ctx, Services.Notification.GetNotifications(sessionId, isWorkshop));
        }

        public static void ScheduleAllNotifications(Context ctx)
        {
            var notifications = Services.Notification.GetSelected();
            Console.Out.WriteLine("HELPS: Notifications Found - " + notifications.Count);
            Schedule(ctx, notifications);
        }

        public static void ScheduleAllNotifications(Context ctx, bool isWorkshop)
        {
            var notifications = Services.Notification.GetNotifications(isWorkshop);
            Schedule(ctx, notifications);
        }

        private static void Schedule(Context ctx, List<NotificationOption> notifications)
        {
            foreach (var notification in notifications.Where(x => x.selected))
            {
                CreateAlarmManager(ctx, GetNotification(ctx, notification.sessionId, notification),
                    notification.ScheduledDate, notification, notification.sessionId);
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
            private Context ctx;
            private int id;
            private bool isWorkshop;
            private DateTime sessionDate;

            public ActionClickListener(Context ctx, int id, DateTime sessionDate, bool isWorkshop) : base()
            {
                this.id = id;
                this.sessionDate = sessionDate;
                this.ctx = ctx;
                this.isWorkshop = isWorkshop;
            }

            public void OnClick(IDialogInterface dialog, int which)
            {
                if (which == -1)
                {
                    Cancel(ctx, id);
                    Services.Notification.StoreNotifications(id, sessionDate, items);
                    ScheduleNotifications(ctx, id, isWorkshop);
                    ViewHelper.CurrentActivity().RunOnUiThread(() =>
                    {
                        ViewSessionBase.UpdateNotifications(id, isWorkshop);
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