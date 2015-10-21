using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using helps.Shared.DataObjects;

namespace helps.Shared.Database
{
    public class UserTable
    {
        public User GetUser(string StudentId)
        {
            return helpsDatabase.Database.Table<User>().FirstOrDefault(x => x.StudentId == StudentId);
        }

        public int SetUser(User user)
        {
            return helpsDatabase.Database.InsertOrReplace(user);
        }

        public User CurrentUser()
        {
            return helpsDatabase.Database.Table<User>().FirstOrDefault(x => x.AuthToken != null);
        }
        public void ClearCurrentUser()
        {
            var user = CurrentUser();
            user.AuthToken = null;
            helpsDatabase.Database.Update(user);
        }
    }
}
