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
        public List<NotificationOption> GetAll(int bookingId)
        {
            return helpsDatabase.Database.Table<NotificationOption>().Where(x => x.workshopId == bookingId).ToList();
        }

        public List<NotificationOption> GetAll()
        {
            return helpsDatabase.Database.Table<NotificationOption>().ToList();
        }

        public void InsertAll(List<NotificationOption> notifications)
        {
            Clear(notifications.First().workshopId);
            helpsDatabase.Database.InsertAll(notifications);
        }

        private void Clear(int bookingId)
        {
            helpsDatabase.Database.Table<NotificationOption>().Delete(x => x.workshopId == bookingId);
        }
    }
}
