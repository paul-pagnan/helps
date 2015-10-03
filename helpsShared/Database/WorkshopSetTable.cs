using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using helps.Shared.DataObjects;

namespace helps.Shared.Database
{
    public class WorkshopSetTable
    {
        // Update once a day - WorkshopSets change very rarely
        public int UpdateBuffer = 1440;
       
        public List<WorkshopSet> GetAll()
        {
            return helpsDatabase.Database.Table<WorkshopSet>().ToList<WorkshopSet>();
        }

        public static WorkshopSet Get(int Id)
        {
            return helpsDatabase.Database.Table<WorkshopSet>().Where(x => x.Id == Id).FirstOrDefault();
        }

        public WorkshopSet First()
        {
            return helpsDatabase.Database.Table<WorkshopSet>().FirstOrDefault();
        }

        public void SetAll(List<WorkshopSet> list)
        {
            var updatedList = list.Select(x => { x.LastUpdated = DateTime.Now; return x; }).ToList();

            helpsDatabase.Database.Table<WorkshopSet>().Delete(x => x.Id != null);
            helpsDatabase.Database.InsertAll(updatedList);
        }

        public bool NeedsUpdating()
        {
            var record = First();
            return (record == null) ? true : helpsDatabase.NeedsUpdating(record.LastUpdated, UpdateBuffer);
        }
    }
}
