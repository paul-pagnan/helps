﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using helps.Shared.DataObjects;

namespace helps.Shared.Database
{
    public class WorkshopSetTable : helpsDatabase
    {
        public int UpdateBuffer = 60;
        public WorkshopSetTable() : base() { }
       
        public List<WorkshopSet> Get()
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