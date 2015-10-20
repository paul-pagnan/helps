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
            if(Database == null)
                Database = DependencyService.Get<ISQLite>().GetConnection();

            //Workshops
            Database.CreateTable<Workshop>();
            Database.CreateTable<WorkshopSet>();
            Database.CreateTable<WorkshopBooking>();

            //Notification
            Database.CreateTable<NotificationOption>();
            
            //Auth
            Database.CreateTable<User>();

            //Misc
            Database.CreateTable<Campus>();
        }

        public static bool NeedsUpdating(DateTime lastUpdated, int UpdateBuffer)
        {
            return DateTime.Now > lastUpdated.AddMinutes(UpdateBuffer);
        }

        public static void ClearDatabase()
        {
            Database.DropTable<NotificationOption>();
            Database.DropTable<Workshop>();
            Database.DropTable<WorkshopSet>();
            Database.DropTable<WorkshopBooking>();
            Database.DropTable<Campus>();
            InitDatabase();
        }
    }
}
