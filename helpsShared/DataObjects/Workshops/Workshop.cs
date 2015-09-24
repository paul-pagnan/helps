using SQLite;
using System;

namespace helps.Shared.DataObjects
{
    public class Workshop
    {
        [PrimaryKey]
        public int WorkshopId { get; set; }
        public string topic { get; set; }
        public string description { get; set; }
        public string targetingGroup { get; set; }
        public string campus { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int maximum { get; set; }
        public int WorkShopSetId { get; set; }
        public int? cutoff { get; set; }
        public string type { get; set; }
        public string DaysOfWeek { get; set; }
        public int BookingCount { get; set; }

        public DateTime LastUpdated { get; set; }
    }
}
