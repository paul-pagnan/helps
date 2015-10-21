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
        public static UserTable userTable = new UserTable();
        
        public static readonly WorkshopSetTable workshopSetTable = new WorkshopSetTable();
        public static readonly WorkshopBookingTable workshopBookingTable = new WorkshopBookingTable();
        public static readonly SessionBookingTable sessionBookingTable = new SessionBookingTable();
        public static readonly WorkshopTable workshopTable = new WorkshopTable();
        public static readonly CampusTable campusTable = new CampusTable();
        public static readonly NotificationTable notificationTable = new NotificationTable();
    }
}