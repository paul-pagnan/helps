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
using System.Threading;
using System.Web.Script.Serialization;
using Connectivity.Plugin;
using helps.Shared.DataObjects.Workshops;

namespace helps.Shared
{
    /// <summary>
    /// The Workshop 
    /// </summary>
    public class WorkshopService : HelpsService
    {

        public WorkshopService() : base()
        {
        }

        public async Task<List<WorkshopSet>> GetWorkshopSets(CancellationToken ct, bool LocalOnly, bool ForceUpdate = false)
        {
            if (!LocalOnly && (workshopSetTable.NeedsUpdating() || ForceUpdate))
            {
                TestConnection();
                
                var response = await helpsClient.GetAsync("api/workshop/workshopSets/true");

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsAsync<GetResponse<WorkshopSet>>();
                    List<WorkshopSet> decodedResponse = result.Results; 
                    workshopSetTable.SetAll(decodedResponse);
                    
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

        public WorkshopDetail GetWorkshopFromBookingLocal(int workshopId)
        {
            var booking = workshopBookingTable.GetByWorkshopId(workshopId);
            return Translater.TranslateDetailLocal(booking);
        }

        public async Task<List<WorkshopPreview>> GetWorkshops(CancellationToken ct, int workshopSet, bool LocalOnly, bool ForceUpdate)
        {
            //TODO Introduce Pagination
            if (!LocalOnly && (workshopTable.NeedsUpdating(workshopSet) || ForceUpdate))
            {
                TestConnection();
                var queryString = "workshopSetId=" + workshopSet + "&active=true" + "&startingDtBegin=" + DateTime.Now.ToString(DateFormat) + "&startingDtEnd=" + DateTime.MaxValue.AddMonths(-1).ToString(DateFormat) + "&pageSize=9999";
                var response = await helpsClient.GetAsync("api/workshop/search?" + queryString);
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsAsync<GetResponse<Workshop>>();
                    List<Workshop> decodedResponse = result.Results;
                    if (decodedResponse != null)
                        workshopTable.SetAllByWorkshopSet(decodedResponse, workshopSet);
                    
                    return Translater.TranslatePreview(decodedResponse);
                }
            }
            
            return Translater.TranslatePreview(workshopTable.GetAll(workshopSet));
        }

        public async Task<List<WorkshopPreview>> GetBookings(CancellationToken ct, bool Current, bool LocalOnly, bool ForceUpdate = false)
        {
            //TODO Introduce Pagination
            if (!LocalOnly && (workshopBookingTable.NeedsUpdating(Current) || ForceUpdate))
            {
                TestConnection();
                await BookingsService.UpdateBookings("workshop", Current);
            }
            
            return await Translater.TranslatePreview(workshopBookingTable.GetAll(Current));
        }

        public async Task<WorkshopBooking> GetBooking(CancellationToken ct, int workshopId, bool LocalOnly, bool ForceUpdate = false)
        {
            if (!LocalOnly && (workshopBookingTable.NeedsUpdating(workshopId) || ForceUpdate))
            {
                TestConnection();
                await BookingsService.UpdateBookings("workshop", true);
            }
            return workshopBookingTable.GetByWorkshopId(workshopId);
        }

        public async Task<GenericResponse> Book(CancellationToken ct, int id)
        {
            var queryString = "workshopId=" + id;
            return await BookingBase(ct, "api/workshop/booking/create?", queryString);
        }

        public async Task<GenericResponse> CancelBooking(CancellationToken ct, int id)
        {
            var workshop = workshopTable.Get(id);
            if (workshop != null && workshop.ProgramId.HasValue) 
                return await CancelProgram(ct, workshop.ProgramId.Value);

            var queryString = "workshopId=" + id;
            var response = await BookingBase(ct, "api/workshop/booking/cancel?", queryString);
            if(response.Success)
                workshopBookingTable.RemoveBookingByWorkshopId(id);
            return response;
        }

        public async Task<GenericResponse> BookProgram(CancellationToken ct, int programId)
        {
            var queryString = "programId=" + programId;
            return await BookingBase(ct, "api/program/booking/create?", queryString);
        }

        public async Task<GenericResponse> CancelProgram(CancellationToken ct, int programId)
        {
            var queryString = "programId=" + programId;
           
            var response = await BookingBase(ct, "api/program/booking/cancel?", queryString);
            if (response.Success)
                foreach (Workshop workshop in workshopTable.GetProgramWorkshops(programId))
                    workshopBookingTable.RemoveBookingByWorkshopId(workshop.WorkshopId);
            return response;
        }

        private async Task<GenericResponse> BookingBase(CancellationToken ct, string endpoint, string queryString)
        {
            if (!IsConnected())
                return ResponseHelper.CreateErrorResponse("No Network Connection", "Please check your network connection and try again");

            var response = await helpsClient.PostAsync(endpoint + queryString + "&studentId=" + AuthService.GetCurrentUser().StudentId + "&userId=" + AuthService.GetCurrentUser().StudentId, null);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsAsync<GetResponse<GenericResponse>>();
                if (result.IsSuccess)
                {
                    await GetBookings(ct, true, false, true);
                    return ResponseHelper.Success();
                }
                return ResponseHelper.CreateErrorResponse("Error", result.DisplayMessage);
            }
            return ResponseHelper.CreateErrorResponse("Error", "An unknown error occured");
        }



        public async Task<GenericResponse> AddNotes(string notes, int workshopId)
        {
            if (!IsConnected())
                return ResponseHelper.CreateErrorResponse("No Network Connection", "Please check your network connection and try again");
            var request = new WorkshopBookingUpdate()
            {
                workshopId = workshopId,
                studentId = AuthService.GetCurrentUser().StudentId,
                userId = AuthService.GetCurrentUser().StudentId,
                notes = notes
            };
            var response = await helpsClient.PutAsJsonAsync("api/workshop/booking/update", request);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsAsync<GetResponse<GenericResponse>>();
                if (result.IsSuccess)
                {

                    workshopBookingTable.UpdateNotes(notes, workshopId);
                    return ResponseHelper.Success();
                }
                return ResponseHelper.CreateErrorResponse("Error", result.DisplayMessage);
            }
            return ResponseHelper.CreateErrorResponse("Error", "An unknown error occurred, please try again");
        }
    }
}
