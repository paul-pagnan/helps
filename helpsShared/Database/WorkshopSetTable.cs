﻿using System;
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

        public WorkshopSet First()
        {
            return helpsDatabase.Database.Table<WorkshopSet>().FirstOrDefault();
        }

        public void SetAll(List<WorkshopSet> list)
        {
            var updatedList = list.Select(x => { x.LastUpdated = DateTime.Now; return x; }).ToList();
            if(First() == null)
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
