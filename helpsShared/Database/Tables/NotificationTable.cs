using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using helps.Shared.DataObjects;

namespace helps.Shared.Database
{
    public class NotificationTable
    {
        public static List<NotificationOption> GetAllWorkshopNotifications(int bookingId)
        {
            return helpsDatabase.Database.Table<NotificationOption>().Where(x => x.sessionId == bookingId && x.isWorkshop).ToList();
        }

        public static List<NotificationOption> GetAllSessionNotifications(int bookingId)
        {
            return helpsDatabase.Database.Table<NotificationOption>().Where(x => x.sessionId == bookingId && !x.isWorkshop).ToList();
        }

        public static List<NotificationOption> GetSelected()
        {
            return helpsDatabase.Database.Table<NotificationOption>().Where(x => x.selected && x.ScheduledDate > DateTime.Now).ToList();
        }
        public static List<NotificationOption> GetAll(bool isWorkshop)
        {
            return helpsDatabase.Database.Table<NotificationOption>().Where(x => x.selected && x.isWorkshop == isWorkshop && x.ScheduledDate > DateTime.Now).ToList();
        }

        public static void InsertAll(List<NotificationOption> notifications, int id, DateTime sessionDate)
        {
            Clear(notifications.First().sessionId);
            notifications = notifications.Select(x =>
            {
                x.sessionId = id;
                x.ScheduledDate = sessionDate.AddMinutes(x.mins * -1);
                return x;
            }).ToList();
            helpsDatabase.Database.InsertAll(notifications);
        }

        public static void Clear(int bookingId)
        {
            helpsDatabase.Database.Table<NotificationOption>().Delete(x => x.sessionId == bookingId);
        }
    }
}
