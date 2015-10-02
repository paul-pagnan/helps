using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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

        public List<Workshop> GetAll(int WorkshopSetId)
        {
            return helpsDatabase.Database.Table<Workshop>().Where(x => x.WorkShopSetId == WorkshopSetId).ToList();
        }

        public List<Workshop> GetAll()
        {
            return helpsDatabase.Database.Table<Workshop>().ToList();
        }

        public Workshop First()
        {
            return helpsDatabase.Database.Table<Workshop>().FirstOrDefault();
        }

        public void SetAll(List<Workshop> list)
        {
            var updatedList = list
                .Select(x => { x.LastUpdated = DateTime.Now; return x; })
                .Select(x => { x.WorkShopSetName = WorkshopSetTable.Get(x.WorkShopSetId).Name; return x; })
                .ToList();
            if (list.Count > 0)
            {
                if (GetAll(list.FirstOrDefault().WorkShopSetId).FirstOrDefault() == null)
                    helpsDatabase.Database.InsertAll(updatedList);
                else
                    helpsDatabase.Database.UpdateAll(updatedList);
            }
        }

        public bool NeedsUpdating()
        {
            var record = First();
            return (record == null) ? true : helpsDatabase.NeedsUpdating(record.LastUpdated, UpdateBuffer);
        }

        public bool NeedsUpdating(int WorkshopSet)
        {
            var record = GetAll(WorkshopSet).FirstOrDefault();
            return (record == null) ? true : helpsDatabase.NeedsUpdating(record.LastUpdated, UpdateBuffer);
        }

        public List<Workshop> GetProgramWorkshops(int programId)
        {
            return
                helpsDatabase.Database.Table<Workshop>()
                    .Where(x => x.ProgramId == programId)
                    .ToList();
        }
    }
}
