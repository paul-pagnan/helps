using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using helps.Shared.DataObjects;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Formatting;
using helps.Shared.Helpers;
using System.Diagnostics;

namespace helps.Shared
{
    public class WorkshopService : HelpsService
    {
        public WorkshopService() : base()
        {
        }

        public async Task<List<WorkshopSet>> GetWorkshopSets(bool ForceUpdate)
        {
            if (workshopSetTable.NeedsUpdating() || ForceUpdate)
            {
                var response = helpsClient.GetAsync("api/workshop/workshopSets/true").Result;

                if (response.IsSuccessStatusCode)
                {
                    List<WorkshopSet> decodedResponse = response.Content.ReadAsAsync<GetResponse<WorkshopSet>>().Result.Results;
                    workshopSetTable.SetAll(decodedResponse);
                    return decodedResponse;
                }
            }
            return workshopSetTable.Get();
        }
    }
}
