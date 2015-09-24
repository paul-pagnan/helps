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
    public static class helpsDatabase
    {
        public static SQLiteConnection Database;
        public static void InitDatabase()
        {
            Database = DependencyService.Get<ISQLite>().GetConnection();

            //Auth
            Database.CreateTable<User>();

            //Workshops
            Database.CreateTable<Workshop>();
            Database.CreateTable<WorkshopSet>();
            Database.CreateTable<WorkshopBooking>();

            //Misc
            Database.CreateTable<Campus>();
        }

        public static bool NeedsUpdating(DateTime lastUpdated, int UpdateBuffer)
        {
            return DateTime.Now > lastUpdated.AddMinutes(UpdateBuffer);
        }
    }
}
