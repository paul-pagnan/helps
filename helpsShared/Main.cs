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
        public static UserTable userTable;
        
        public static WorkshopSetTable workshopSetTable;
        public static WorkshopBookingTable workshopBookingTable;
        public static CampusTable campusTable;


        public Main()
        {
            userTable = new UserTable();
            workshopSetTable = new WorkshopSetTable();
            workshopBookingTable = new WorkshopBookingTable();
            campusTable = new CampusTable();
        }
    }
}