using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using helps.Shared.DataObjects;
using Xamarin.Forms;

namespace helps.Shared.Database
{
    public class SessionBookingTable 
    {
        // Update Hourly
        public int UpdateBuffer = 60;

        public SessionBooking GetBySessionId(int id)
        {
            return helpsDatabase.Database.Table<SessionBooking>().FirstOrDefault(x => x.SessionId == id);
        }

        public List<SessionBooking> GetAll(bool Current)
        {
            if (Current) { 
                return 
                    helpsDatabase.Database.Table<SessionBooking>()
                        .Where(x => x.StartDate > DateTime.Now)
                        .OrderBy(x => x.EndDate)
                        .ToList();
            }
            return
                helpsDatabase.Database.Table<SessionBooking>()
                    .Where(x => x.EndDate < DateTime.Now)
                    .OrderByDescending(x => x.EndDate)
                    .ToList();
        }

        public SessionBooking Get(int sessionId)
        {
            return helpsDatabase.Database.Table<SessionBooking>().FirstOrDefault(x => x.SessionId == sessionId);
        }

        public SessionBooking First(bool? Current = null)
        {
            if (Current.HasValue)
                return GetAll(Current.Value).FirstOrDefault();
            return helpsDatabase.Database.Table<SessionBooking>().FirstOrDefault();
        }

        public async Task<bool> SetAll(List<SessionBooking> list, bool? Current)
        {
            var updatedList = list
               .Select(x => { x.LastUpdated = DateTime.Now; return x; })
               .ToList();

            if (Current.HasValue)
            {
                foreach (var item in list)
                    helpsDatabase.Database.Table<SessionBooking>().Delete(x => x.SessionId == item.SessionId);

                if (Current.Value)
                    helpsDatabase.Database.Table<SessionBooking>().Delete(x => x.EndDate > DateTime.Now);
                else
                    helpsDatabase.Database.Table<SessionBooking>().Delete(x => x.EndDate < DateTime.Now);
            }
            helpsDatabase.Database.RunInTransaction(() => { helpsDatabase.Database.InsertAll(updatedList); });
            return true;
        }

        public bool NeedsUpdating(bool Current)
        {
            var record = First(Current);
            return (record == null) ? true : helpsDatabase.NeedsUpdating(record.LastUpdated, UpdateBuffer);
        }

        public bool NeedsUpdating(int sessionId)
        {
            var record = GetBySessionId(sessionId);
            return (record == null) ? true : helpsDatabase.NeedsUpdating(record.LastUpdated, UpdateBuffer);
        }

        public void UpdateNotes(string notes, int sessionId)
        {
            var record = helpsDatabase.Database.Table<SessionBooking>().FirstOrDefault(x => x.SessionId == sessionId);
            if (record != null)
            {
                record.notes = notes;
                helpsDatabase.Database.Update(record);
            }
        }
    }
}
