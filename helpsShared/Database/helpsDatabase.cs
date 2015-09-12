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
        SQLiteConnection database;
        public helpsDatabase()
        {
            database = DependencyService.Get<ISQLite>().GetConnection();
            database.CreateTable<User>();
        }
        
        public User GetUser(string StudentId)
        {
            return database.Table<User>().FirstOrDefault(x => x.StudentId == StudentId);
        }

        public int SetUser(User user)
        {
            return database.InsertOrReplace(user);
        }

        public User CurrentUser()
        {
            return database.Table<User>().FirstOrDefault(x => x.AuthToken != null);
        }
    }
}
