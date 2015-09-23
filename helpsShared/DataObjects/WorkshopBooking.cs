using SQLite;
using System;

namespace helps.Shared.DataObjects
{
    public class WorkshopBooking
    { 
        [PrimaryKey]
        public int BookingId { get; set; }
        public int workshopId { get; set; }
        public string studentId { get; set; }
        public string topic { get; set; }
        public string description { get; set; }
        public string targetingGroup { get; set; }
        public string campusID { get; set; }
        public string starting { get; set; }
        public string ending { get; set; }
        public int maximum { get; set; }
        public int cutoff { get; set; }
        public string attended { get; set; }
        public string type { get; set; }
        public DateTime LastUpdated { get; set; }
    }
}