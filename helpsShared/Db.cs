using helps.Shared.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace helps.Shared
{
    public static class Db
    {
        public static UserTable User;
        public static WorkshopSetTable WorkshopSet;
        static Db()
        {
            User = new UserTable();
            WorkshopSet = new WorkshopSetTable();
        }
    }
}