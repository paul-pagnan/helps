using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using helps.Shared.DataObjects;

namespace helps.Shared.Database
{
    public class WorkshopBookingTable : helpsDatabase
    {
        // Update Hourly
        public int UpdateBuffer = 0;
        public WorkshopBookingTable() : base() { }

        public List<WorkshopBooking> GetAll()
        {
            return database.Table<WorkshopBooking>().ToList<WorkshopBooking>();
        }

        public WorkshopBooking Get(int id)
        {
            return database.Table<WorkshopBooking>().Where(x => x.BookingId == id).FirstOrDefault();
        }

        public List<WorkshopBooking> GetAll(bool Current)
        {
            if(Current)
                return database.Table<WorkshopBooking>().Where(x => x.attended == null).ToList();
            else
                return database.Table<WorkshopBooking>().Where(x => x.attended != null).ToList();
        }

        public WorkshopBooking First()
        {
            return database.Table<WorkshopBooking>().FirstOrDefault();
        }

        public void SetAll(List<WorkshopBooking> list)
        {
            var updatedList = list.Select(x => { x.LastUpdated = DateTime.Now; return x; }).ToList();
            if (First() == null)
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
