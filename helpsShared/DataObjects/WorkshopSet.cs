using SQLite;
using System;

namespace helps.Shared.DataObjects
{
    public class WorkshopSet
    {
        [PrimaryKey]
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime LastUpdated { get; set; }
    }
}
