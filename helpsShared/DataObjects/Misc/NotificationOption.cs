using System;
using SQLite;

namespace helps.Shared.DataObjects
{
    public class NotificationOption
    {
        [PrimaryKey] [AutoIncrement]
        public int notificationId { get; set; }
        public int sessionId { get; set; }
        public string title { get; set; }
        public int mins { get; set; }
        public DateTime ScheduledDate { get; set; }
        public bool selected { get; set; }
        public bool isWorkshop { get; set; }
    }
}