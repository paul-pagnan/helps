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

        public WorkshopBooking GetByWorkshopId(int id, bool Current = false)
        {
            var a = GetAll(Current);
            var b = helpsDatabase.Database.Table<WorkshopBooking>().FirstOrDefault(x => x.workshopId == id && x.starting > DateTime.UtcNow);
            return b;
        }

        public void RemoveBookingByWorkshopId(int id)
        {
            helpsDatabase.Database.Table<WorkshopBooking>().Delete(x => x.workshopId == id);
        }

        public List<WorkshopBooking> GetAll(bool Current)
        {
            if (Current)
                return
                    helpsDatabase.Database.Table<WorkshopBooking>()
                        .Where(x => x.starting > DateTime.UtcNow)
                        .OrderBy(x => x.starting)
                        .ToList();
            return
                helpsDatabase.Database.Table<WorkshopBooking>()
                    .Where(x => x.starting < DateTime.UtcNow)
                    .OrderByDescending(x => x.starting)
                    .ToList();
    }

        public WorkshopBooking First(bool? Current = null)
        {
            if (Current.HasValue)
                return GetAll(Current.Value).FirstOrDefault();
            return helpsDatabase.Database.Table<WorkshopBooking>().FirstOrDefault();
        }

        public async Task<bool> SetAll(List<WorkshopBooking> list, bool? Current)
        {
            var updatedList = list
               .Select(x => { x.LastUpdated = DateTime.Now; return x; })
               .Select(x =>
               {
                   var set = WorkshopSetTable.Get(x.WorkShopSetID);
                   if(set != null)
                        x.WorkShopSetName = set.Name;
                   return x;
               })
               .ToList();

            if (Current.HasValue)
            {
                foreach (var booking in GetAll(Current.Value))
                    helpsDatabase.Database.Table<WorkshopBooking>().Delete(x => x.BookingId == booking.BookingId);
            }
            helpsDatabase.Database.RunInTransaction(() => { helpsDatabase.Database.InsertAll(updatedList); });
            return true;
        }

        public bool NeedsUpdating(bool Current)
        {
            var record = First(Current);
            return (record == null) ? true : helpsDatabase.NeedsUpdating(record.LastUpdated, UpdateBuffer);
        }

        public bool NeedsUpdating(int workshopId)
        {
            var record = GetByWorkshopId(workshopId);
            return (record == null) ? true : helpsDatabase.NeedsUpdating(record.LastUpdated, UpdateBuffer);
        }

    }
}
