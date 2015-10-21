using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using helps.Shared.DataObjects;

namespace helps.Shared
{
    public class BookingsService : HelpsService
    {
        public static async Task<bool> UpdateBookings(string type, bool? Current = null)
        {
            var currentUser = userTable.CurrentUser().StudentId;
            var queryString = "studentId=" + currentUser + "&pageSize=9999&active=true";
            if (Current.HasValue)
            {
                if (Current.Value)
                    queryString += "&startingDtBegin=" + DateTime.Now.ToString(DateFormat) + "&startingDtEnd=" +
                                   DateTime.MaxValue.AddMonths(-1).ToString(DateFormat);
                else
                    queryString += "&startingDtBegin=" + DateTime.MinValue.AddYears(2000).ToString(DateFormat) +
                                   "&startingDtEnd=" + DateTime.Now.ToString(DateFormat);
            }

            var response = await helpsClient.GetAsync("api/" + type + "/booking/search?" + queryString);
            if (response.IsSuccessStatusCode)
            {
                if (Current.HasValue)
                {
                    if (type == "workshop")
                        await DecodeAndUpdateWorkshop(response, Current.Value);
                    else
                        await DecodeAndUpdateSession(response, Current.Value);
                    CurrentlyUpdating = false;
                    return true;
                }
            }
            CurrentlyUpdating = false;
            return false;
        }

        private static async Task DecodeAndUpdateWorkshop(HttpResponseMessage response, bool Current)
        {
            var result = await response.Content.ReadAsAsync<GetResponse<WorkshopBooking>>(Formatters());
            List<WorkshopBooking> decodedResponse = result.Results;
            if (decodedResponse != null)
                await workshopBookingTable.SetAll(decodedResponse, Current);
        }

        private static async Task DecodeAndUpdateSession(HttpResponseMessage response, bool Current)
        {
            var result = await response.Content.ReadAsAsync<GetResponse<SessionBooking>>(Formatters());
            List<SessionBooking> decodedResponse = result.Results;
            if (decodedResponse != null)

                await sessionBookingTable.SetAll(decodedResponse, Current);
        }
    }
}
