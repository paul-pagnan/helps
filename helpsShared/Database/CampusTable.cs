using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using helps.Shared.DataObjects;

namespace helps.Shared.Database
{
    public class CampusTable
    {
        // Update once a day - WorkshopSets change very rarely
        public int UpdateBuffer = 1440;
        public CampusTable() : base() { }
       
        public Campus GetCampus(int id)
        {
            return helpsDatabase.Database.Table<Campus>().Where(x => x.Id == id).FirstOrDefault();
        }

        public async Task<bool> SetAll(List<Campus> list)
        {
            if (First() == null)
                helpsDatabase.Database.InsertAll(list);
            else
                helpsDatabase.Database.UpdateAll(list);
            return true;
        }

        public Campus First()
        {
            return helpsDatabase.Database.Table<Campus>().FirstOrDefault();
        }
    }
}
