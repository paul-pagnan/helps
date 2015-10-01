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
                return TranslateDetail(workshopTable.GetProgramWorkshops(workshop.ProgramId.GetValueOrDefault()));
            return TranslateDetail(new List<Workshop>() { workshopTable.Get(workshopId) });
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
                        workshopTable.SetAll(decodedResponse);
                    CurrentlyUpdating = false;
                    return TranslatePreview(decodedResponse);
                }
            }
            return TranslatePreview(workshopTable.GetAll(workshopSet));
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
            return await TranslatePreview(workshopBookingTable.GetAll(Current));
        }

        private async Task<bool> UpdateBookings()
        {
            var currentUser = userTable.CurrentUser().StudentId;
            var queryString = "studentId=" + currentUser + "&pageSize=9999";
            var response = await helpsClient.GetAsync("api/workshop/booking/search?" + queryString);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsAsync<GetResponse<WorkshopBooking>>(Formatters());
                List<WorkshopBooking> decodedResponse = result.Results;
                workshopBookingTable.SetAll(decodedResponse);
                CurrentlyUpdating = false;
                return true;
            }
            return false;
        }

        private async Task<List<WorkshopPreview>> TranslatePreview(List<WorkshopBooking> list)
        {
            List<WorkshopPreview> translated = new List<WorkshopPreview>();
            foreach (WorkshopBooking booking in list)
            {
                translated.Add(new WorkshopPreview
                {
                    Id = booking.workshopId,
                    Name = booking.topic,
                    WorkshopSet = booking.WorkShopSetID,
                    NumSessions = booking.WorkShopSetName,
                    Time = HumanizeTimeSpan(booking.starting, booking.ending),
                    DateHumanFriendly = HumanizeDate(booking.starting),
                    Location = await MiscServices.GetCampus(booking.campusID),
                    FilledPlaces = -1,
                    TotalPlaces = booking.maximum
                });
            }
            return translated;
        }

        private WorkshopDetail TranslateDetail(List<Workshop> workshops)
        {
            List<SessionPreview> sessions = new List<SessionPreview>();
            if (workshops.Count > 1)
            {
                foreach (Workshop session in workshops)
                {
                    sessions.Add(new SessionPreview()
                    {
                        Id = session.WorkshopId,
                        Title = HumanizeDate(session.StartDate, true),
                        Time = HumanizeTimeSpan(session.StartDate, session.EndDate),
                        Location = session.campus
                    });
                }
            }

            var workshop = workshops.FirstOrDefault();
            return new WorkshopDetail()
            {
                Id = workshop.WorkshopId,
                Title = workshop.topic,
                Room = workshop.campus,
                Time = (workshop.type != "multiple") ? HumanizeTimeSpan(workshop.StartDate, workshop.EndDate) : null,
                DateHumanFriendly = (workshop.type == "multiple") ? HumanizeDate(workshop.ProgramStartDate.GetValueOrDefault(), workshop.ProgramEndDate.GetValueOrDefault()) : workshop.StartDate.ToString("dd/MM/yyyy"),
                TargetGroup = workshop.targetingGroup ?? "N/A",
                Description = workshop.description ?? "N/A",
                FilledPlaces = workshop.BookingCount,
                TotalPlaces = workshop.maximum,
                Sessions = sessions
            };
        }


        private List<WorkshopPreview> TranslatePreview(List<Workshop> list)
        {
            List<WorkshopPreview> translated = new List<WorkshopPreview>();
            List<int> programs = new List<int>();
            foreach (Workshop workshop in list)
            {
                if (!programs.Contains(workshop.ProgramId.GetValueOrDefault()))
                {
                    programs.Add(workshop.ProgramId.GetValueOrDefault());
                    translated.Add(new WorkshopPreview
                    {
                        Id = workshop.WorkshopId,
                        Name = workshop.topic,
                        WorkshopSet = workshop.WorkShopSetId,
                        NumSessions = (workshop.type == "multiple") ? "Num of Sessions: " + workshop.NumOfWeeks : "",
                        Time = HumanizeTimeSpan(workshop.StartDate, workshop.EndDate),
                        DateHumanFriendly = (workshop.type == "multiple") ? HumanizeDate(workshop.ProgramStartDate.GetValueOrDefault(), workshop.ProgramEndDate.GetValueOrDefault()) : HumanizeDate(workshop.StartDate),
                        Location = workshop.campus,
                        FilledPlaces = workshop.BookingCount,
                        TotalPlaces = workshop.maximum
                    });
                }
            }
            return translated;
        }

        private string HumanizeDate(DateTime starting, bool IncludeDay = false)
        {
            var humanized = starting.ToString("dd/MM/yyyy");
            bool Past = starting < DateTime.Now;

            if (starting < DateTime.Now.AddDays(1) && !Past)
                humanized = "Today";
            else if (starting < DateTime.Now.AddDays(2) && !Past)
                humanized = "Tomorrow";
            else if (IncludeDay)
                humanized = starting.DayOfWeek + " " + humanized;
            return humanized;
        }

        private string HumanizeDate(DateTime starting, DateTime ending) 
        {
            return starting.ToString("dd/MM/yyyy") + " - " + ending.ToString("dd/MM/yyyy");
        }

        private string HumanizeTimeSpan(DateTime start, DateTime end)
        {
            return To12Hour(start.Hour).ToString() + " - " + To12Hour(end.Hour).ToString() + " " + Meridiem(end.Hour);
        }

        private int To12Hour(int Hour)
        {
            return (Hour > 12) ? (Hour - 12) : Hour;
        }

       
        private string Meridiem(int Hour)
        {
            return (Hour >= 12) ? "PM" : "AM";
        }
    }
}
