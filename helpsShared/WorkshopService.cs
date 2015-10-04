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

        public WorkshopDetail GetWorkshop(int workshopId)
        {
            var workshop = workshopTable.Get(workshopId);
            if (workshop.type == "multiple")
                return Translater.TranslateDetail(workshopTable.GetProgramWorkshops(workshop.ProgramId.GetValueOrDefault()));
            return Translater.TranslateDetail(new List<Workshop>() { workshop });
        }

        public async Task<WorkshopDetail> GetWorkshopFromBooking(int workshopId)
        {
            var booking = workshopBookingTable.GetByWorkshopId(workshopId);
            return await Translater.TranslateDetail(booking);
        }

        public async Task<List<WorkshopPreview>> GetWorkshops(int workshopSet, bool LocalOnly, bool ForceUpdate)
        {
            //TODO Introduce Pagination
            if (!LocalOnly && ((workshopTable.NeedsUpdating(workshopSet) || ForceUpdate) && !CurrentlyUpdating))
            {
                TestConnection();
                CurrentlyUpdating = true;

                var queryString = "workshopSetId=" + workshopSet + "&active=true" + "&startingDtBegin=" + DateTime.Now.ToString(DateFormat) + "&startingDtEnd=" + DateTime.MaxValue.AddMonths(-1).ToString(DateFormat) + "&pageSize=9999";
                var response = await helpsClient.GetAsync("api/workshop/search?" + queryString);
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsAsync<GetResponse<Workshop>>();
                    List<Workshop> decodedResponse = result.Results;
                    if (decodedResponse != null)
                        workshopTable.SetAllByWorkshopSet(decodedResponse, workshopSet);
                    CurrentlyUpdating = false;
                    return Translater.TranslatePreview(decodedResponse);
                }
            }
            return Translater.TranslatePreview(workshopTable.GetAll(workshopSet));
        }

      

        public async Task<List<WorkshopPreview>> GetBookings(bool Current, bool LocalOnly, bool ForceUpdate = false)
        {
            //TODO Introduce Pagination
            if (!LocalOnly && ((workshopBookingTable.NeedsUpdating(Current) || ForceUpdate) && !CurrentlyUpdating))
            {
                TestConnection();
                CurrentlyUpdating = true;
                await UpdateBookings(Current);
            }
            return await Translater.TranslatePreview(workshopBookingTable.GetAll(Current));
        }

        public async Task<WorkshopBooking> GetBooking(int workshopId, bool LocalOnly, bool ForceUpdate = false, bool Current = false)
        {
            if (!LocalOnly && ((workshopBookingTable.NeedsUpdating(workshopId) || ForceUpdate) && !CurrentlyUpdating))
            {
                TestConnection();
                CurrentlyUpdating = true;
                await UpdateBookings(true);
            }
            return workshopBookingTable.GetByWorkshopId(workshopId, Current);
        }

        public async Task<GenericResponse> Book(int id)
        {
            var queryString = "workshopId=" + id;
            return await BookingBase("api/workshop/booking/create?", queryString);
        }

        public async Task<GenericResponse> CancelBooking(int id)
        {
            var workshop = workshopTable.Get(id);
            if (workshop != null && workshop.ProgramId.HasValue) 
                return await CancelProgram(workshop.ProgramId.Value);

            var queryString = "workshopId=" + id;
            var response = await BookingBase("api/workshop/booking/cancel?", queryString);
            if(response.Success)
                workshopBookingTable.RemoveBookingByWorkshopId(id);
            return response;
        }

        public async Task<GenericResponse> BookProgram(int programId)
        {
            var queryString = "programId=" + programId;
            return await BookingBase("api/program/booking/create?", queryString);
        }

        public async Task<GenericResponse> CancelProgram(int programId)
        {
            var queryString = "programId=" + programId;
           
            var response = await BookingBase("api/program/booking/cancel?", queryString);
            if (response.Success)
                foreach (Workshop workshop in workshopTable.GetProgramWorkshops(programId))
                    workshopBookingTable.RemoveBookingByWorkshopId(workshop.WorkshopId);
            return response;
        }

        private async Task<GenericResponse> BookingBase(string endpoint, string queryString)
        {
            var response = await helpsClient.PostAsync(endpoint + queryString + "&studentId=" + AuthService.GetCurrentUser().StudentId + "&userId=" + AuthService.GetCurrentUser().StudentId, null);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsAsync<GetResponse<GenericResponse>>();
                if (result.IsSuccess)
                {
                    GetBookings(true, false, true);
                    return ResponseHelper.Success();
                }
                else
                    return ResponseHelper.CreateErrorResponse("Error", result.DisplayMessage);
            }
            return ResponseHelper.CreateErrorResponse("Error", "An unknown error occured");
        }



        public async Task<bool> UpdateBookings(bool? Current = null)
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

            var response = await helpsClient.GetAsync("api/workshop/booking/search?" + queryString);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsAsync<GetResponse<WorkshopBooking>>(Formatters());
                List<WorkshopBooking> decodedResponse = result.Results;
                if(decodedResponse != null)
                    await workshopBookingTable.SetAll(decodedResponse, Current);
                CurrentlyUpdating = false;
                return true;
            }
            return false;
        }
    }
}
