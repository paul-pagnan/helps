using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using helps.Shared.DataObjects;

namespace helps.Shared.Database
{
    public class WorkshopSetTable : helpsDatabase
    {
        // Update once a day - WorkshopSets change very rarely
        public int UpdateBuffer = 1440;
        public WorkshopSetTable() : base() { }
       
        public List<WorkshopSet> GetAll()
        {
            return database.Table<WorkshopSet>().ToList<WorkshopSet>();
        }

        public WorkshopSet First()
        {
            return database.Table<WorkshopSet>().FirstOrDefault();
        }

        public void SetAll(List<WorkshopSet> list)
        {
            var updatedList = list.Select(x => { x.LastUpdated = DateTime.Now; return x; }).ToList();
            if(First() == null)
                database.InsertAll(updatedList);
            else
                database.UpdateAll(updatedList);
        }

        public bool NeedsUpdating()
        {
            var record = First();
            return (record == null) ? true : base.NeedsUpdating(record.LastUpdated, UpdateBuffer);
        }
    }
}
