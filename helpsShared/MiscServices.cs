using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using helps.Shared.DataObjects;
using System.Net.Http;

namespace helps.Shared
{
    public class MiscServices : HelpsService
    {
        public MiscServices() : base()
        {
        }

        public static async Task<string> GetCampus(int id)
        {
            if (campusTable.GetCampus(id) == null)
            {
                TestConnection();
                CurrentlyUpdating = true;
                var response = await helpsClient.GetAsync("api/misc/campus/true");

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsAsync<GetResponse<Campus>>();
                    List<Campus> decodedResponse = result.Results;
                    campusTable.SetAll(decodedResponse);
                    CurrentlyUpdating = false;
                }
            }
            return campusTable.GetCampus(id).campus;
        }

        public static string GetCampusLocal(int campusID)
        {
            return  campusTable.GetCampus(campusID).campus;
        }
    }
}
