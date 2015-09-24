using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using helps.Shared.DataObjects;

namespace helps.Shared.Database
{
    public class WorkshopTable
    {
        // Update Hourly
        public int UpdateBuffer = 60;

        public Workshop Get(int id)
        {
            return helpsDatabase.Database.Table<Workshop>().Where(x => x.WorkshopId == id).FirstOrDefault();
        }

        public Workshop First()
        {
            return helpsDatabase.Database.Table<Workshop>().FirstOrDefault();
        }

        public void SetAll(List<Workshop> list)
        {
            var updatedList = list.Select(x => { x.LastUpdated = DateTime.Now; return x; }).ToList();
            if (First() == null)
                helpsDatabase.Database.InsertAll(updatedList);
            else
                helpsDatabase.Database.UpdateAll(updatedList);
        }

        public bool NeedsUpdating()
        {
            var record = First();
            return (record == null) ? true : helpsDatabase.NeedsUpdating(record.LastUpdated, UpdateBuffer);
        }

    }
}
