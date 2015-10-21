using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using helps.Shared.Database;
using helps.Shared.DataObjects;

namespace helps.Shared
{
    public class SettingService 
    { 
        public static void toggleNotifications(bool enabled)
        {
            SettingTable.Set(new Setting(){ key = "notification", value = enabled.ToString()});
        }

        public static bool notificationsEnbabled()
        {
            var result = SettingTable.Get("notification");
            return result == "True";
        }
    }
}
