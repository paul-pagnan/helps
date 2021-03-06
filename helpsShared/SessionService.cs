﻿using System;
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
    public class SessionService : HelpsService
    {
        public SessionService() : base()
        {
        }

        public async Task<List<WorkshopPreview>> GetBookings(CancellationToken ct, bool Current, bool LocalOnly, bool ForceUpdate = false)
        {
            //TODO Introduce Pagination
            if (!LocalOnly && ((sessionBookingTable.NeedsUpdating(Current) || ForceUpdate)))
            {
                TestConnection();
                await BookingsService.UpdateBookings("session", Current);
            }
            
            return await Translater.TranslatePreview(sessionBookingTable.GetAll(Current));
        }

        public SessionDetail GetSession(int sessionId)
        {
            var session = sessionBookingTable.Get(sessionId);
            return Translater.TranslateSession(session);
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
            var response = await helpsClient.PutAsJsonAsync("api/sessionId/booking/update", request);
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
