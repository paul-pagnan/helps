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
using Connectivity.Plugin;

namespace helps.Shared
{
    public class WorkshopService : HelpsService
    {

        public WorkshopService() : base()
        {
        }

        public async Task<List<WorkshopSet>> GetWorkshopSets(bool LocalOnly, bool ForceUpdate = false)
        {
            if (!LocalOnly && ((workshopSetTable.NeedsUpdating() || ForceUpdate) && !CurrentlyUpdating))
            {
                TestConnection();
                CurrentlyUpdating = true;
                var response = await helpsClient.GetAsync("api/workshop/workshopSets/true");

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsAsync<GetResponse<WorkshopSet>>();
                    List<WorkshopSet> decodedResponse = result.Results;
                    workshopSetTable.SetAll(decodedResponse);
                    CurrentlyUpdating = false;
                    return decodedResponse;
                }
            }
            return workshopSetTable.GetAll();
        }

        public async Task<List<WorkshopPreview>> GetBookings(bool Current, bool LocalOnly, bool ForceUpdate = false)
        {
            //TODO Introduce Pagination
            if (!LocalOnly && ((workshopBookingTable.NeedsUpdating() || ForceUpdate) && !CurrentlyUpdating))
            {
                TestConnection();

                CurrentlyUpdating = true;
                await UpdateBookings();
            }
            return TranslatePreview(workshopBookingTable.GetAll(Current));
        }

        private async Task<bool> UpdateBookings()
        {
            var currentUser = userTable.CurrentUser().StudentId;
            var queryString = "studentId=" + currentUser;
            var response = await helpsClient.GetAsync("api/workshop/booking/search?" + queryString);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsAsync<GetResponse<WorkshopBooking>>();
                List<WorkshopBooking> decodedResponse = result.Results;
                workshopBookingTable.SetAll(decodedResponse);
                CurrentlyUpdating = false;
                return true;
            }
            return false;
        }

        private List<WorkshopPreview> TranslatePreview(List<WorkshopBooking> list)
        {
            List<WorkshopPreview> translated = new List<WorkshopPreview>();

            foreach (WorkshopBooking booking in list)
            {
                translated.Add(new WorkshopPreview
                {
                    Id = booking.workshopId,
                    Name = booking.topic,
                    WorkshopSet = booking.workshopId,
                    Time = "asf",
                    DateHumanFriendly = "123",
                    Location = booking.campusID,
                    FilledPlaces = -1,
                    TotalPlaces = booking.maximum
                });
            }
            return translated;
        }
    }
}
