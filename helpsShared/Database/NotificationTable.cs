﻿using System;
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
            return helpsDatabase.Database.Table<NotificationOption>().Where(x => x.workshop == bookingId).ToList();
        }

        public List<NotificationOption> GetAll()
        {
            return helpsDatabase.Database.Table<NotificationOption>().Where(x => x.selected && x.ScheduledDate > DateTime.Now).ToList();
        }

        public void InsertAll(List<NotificationOption> notifications)
        {
            Clear(notifications.First().workshop);
            helpsDatabase.Database.InsertAll(notifications);
        }

        public void Clear(int bookingId)
        {
            helpsDatabase.Database.Table<NotificationOption>().Delete(x => x.workshop == bookingId);
        }
    }
}
