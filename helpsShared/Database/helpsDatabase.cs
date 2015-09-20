using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using SQLite;
using helps.Shared.DataObjects;

namespace helps.Shared.Database
{
    public class helpsDatabase
    {
        public SQLiteConnection database;
        public helpsDatabase()
        {
            database = DependencyService.Get<ISQLite>().GetConnection();
            database.CreateTable<User>();
            database.CreateTable<WorkshopSet>();
        }

        public bool NeedsUpdating(DateTime lastUpdated, int UpdateBuffer)
        {
            return DateTime.Now > lastUpdated.AddMinutes(UpdateBuffer);
        }
     
    }
}
