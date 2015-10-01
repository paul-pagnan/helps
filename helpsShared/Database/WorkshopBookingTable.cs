using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using helps.Shared.DataObjects;

namespace helps.Shared.Database
{
    public class WorkshopBookingTable 
    {
        // Update Hourly
        public int UpdateBuffer = 60;

        public List<WorkshopBooking> GetAll()
        {
            return helpsDatabase.Database.Table<WorkshopBooking>().ToList<WorkshopBooking>();
        }

        public WorkshopBooking Get(int id)
        {
            return helpsDatabase.Database.Table<WorkshopBooking>().Where(x => x.BookingId == id).FirstOrDefault();
        }

        public List<WorkshopBooking> GetAll(bool Current)
        {
            if(Current)
                return helpsDatabase.Database.Table<WorkshopBooking>().Where(x => x.attended == DateTime.MinValue).ToList();
            else
                return helpsDatabase.Database.Table<WorkshopBooking>().Where(x => x.attended != DateTime.MinValue).ToList();
        }

        public WorkshopBooking First()
        {
            return helpsDatabase.Database.Table<WorkshopBooking>().FirstOrDefault();
        }

        public void SetAll(List<WorkshopBooking> list)
        {
            var updatedList = list
               .Select(x => { x.LastUpdated = DateTime.Now; return x; })
               .Select(x => { x.WorkShopSetName = WorkshopSetTable.Get(x.WorkShopSetID).Name; return x; })
               .ToList();
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
