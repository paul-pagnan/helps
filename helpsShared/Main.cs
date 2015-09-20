using helps.Shared.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace helps.Shared
{
    public class Main
    {
        public UserTable userTable;
        public WorkshopSetTable workshopSetTable;
        public Main()
        {
            userTable = new UserTable();
            workshopSetTable = new WorkshopSetTable();
        }
    }
}
