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
        public readonly static List<NotificationOption> DefaultWorkshopNotifications = new List<NotificationOption>()
        {
            new NotificationOption() { title = "10 minutes before", mins = 10, isWorkshop = true},
            new NotificationOption() { title = "30 minutes before", mins = 30, selected = true, isWorkshop = true },
            new NotificationOption() { title = "1 hour before", mins = 60, isWorkshop = true },
            new NotificationOption() { title = "1 day before", mins = 1440, isWorkshop = true },
            new NotificationOption() { title = "1 week before", mins = 10080, isWorkshop = true }
        };

        public readonly static List<NotificationOption> DefaultSessionNotifications = new List<NotificationOption>()
        {
            new NotificationOption() { title = "10 minutes before", mins = 10 },
            new NotificationOption() { title = "30 minutes before", mins = 30, selected = true },
            new NotificationOption() { title = "1 hour before", mins = 60 },
            new NotificationOption() { title = "1 day before", mins = 1440 },
            new NotificationOption() { title = "1 week before", mins = 10080 }
        };

        public NotificationService() : base()
        {
        }

        public void StoreNotifications(int id, DateTime sessionDate, List<NotificationOption> notifications)
        {
            NotificationTable.InsertAll(notifications, id, sessionDate);
        }

        public List<NotificationOption> GetNotifications(int bookingId, bool isWorkshop = true)
        {
            return (isWorkshop) ? 
                NotificationTable.GetAllWorkshopNotifications(bookingId) : 
                NotificationTable.GetAllSessionNotifications(bookingId);
        }

        public List<NotificationOption> GetNotifications(bool isWorkshop)
        {
            return NotificationTable.GetAll(isWorkshop);
        }

        public List<NotificationOption> GetSelected()
        {
            return NotificationTable.GetSelected();
        }

        public void Clear(int workshopId)
        {
            NotificationTable.Clear(workshopId);
        }

    }
}
