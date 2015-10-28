using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using helps.Shared.Database;
using helps.Shared.DataObjects;

namespace helps.Shared
{
    public class NotificationService : HelpsService
    {
        public NotificationService() : base()
        {
        }

        public void StoreNotifications(int id, DateTime notificationDate, List<NotificationOption> notifications)
        {
            notifications = notifications.Select(x =>
            {
                x.workshop = id;
                x.ScheduledDate = notificationDate.AddMinutes(x.mins * -1);
                return x;
            }).ToList();

            notificationTable.InsertAll(notifications);
        }

        public List<NotificationOption> GetNotifications(int workshopId)
        {
            return notificationTable.GetAll(workshopId);
        }

        public List<NotificationOption> GetAll()
        {
            return notificationTable.GetAll();
        }

        public void Clear(int workshopId)
        {
            notificationTable.Clear(workshopId);
        }
    }
}
