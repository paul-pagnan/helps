using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using helps.Shared.DataObjects;

namespace helps.Shared.Database
{
    public class SettingTable
    {
        public static int Set(Setting setting)
        {
            helpsDatabase.Database.Table<Setting>().Delete(x => x.key == setting.key);
            return helpsDatabase.Database.InsertOrReplace(setting);
        }
        public static string Get(string key)
        {
            var result = helpsDatabase.Database.Table<Setting>().FirstOrDefault(x => x.key == key);
            if (result != null)
                return result.value;
            return null;
        }

        public static void seed()
        {
            if (helpsDatabase.Database.Table<Setting>().Count() == 0)
            {
                var defaultSettings = new List<Setting>();
                defaultSettings.Add(new Setting()
                {
                    key = "notification",
                    value = true.ToString()
                });
                defaultSettings.Add(new Setting()
                {
                    key = "font",
                    value = "medium"
                });
                defaultSettings.Add(new Setting()
                {
                    key = "color",
                    value = "default"
                });
                helpsDatabase.Database.InsertAll(defaultSettings);
            }
        }
    }
}
