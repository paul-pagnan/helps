using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using helps.Shared.DataObjects;

namespace helps.Shared.Database
{
    public class UserTable : helpsDatabase
    {
        public UserTable() : base() { }
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
        public void ClearCurrentUser()
        {
            var user = CurrentUser();
            user.AuthToken = null;
            database.Update(user);
        }
    }
}
