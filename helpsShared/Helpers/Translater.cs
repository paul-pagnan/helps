using helps.Shared.DataObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace helps.Shared.Helpers
{
    public class Translater
    {

        public static async Task<List<WorkshopPreview>> TranslatePreview(List<WorkshopBooking> list)
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

        public static WorkshopDetail TranslateDetail(List<Workshop> workshops)
        {
            List<SessionPreview> sessions = new List<SessionPreview>();
            if (workshops.First().type == "multiple")
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
                Date = workshop.StartDate,
                TotalPlaces = workshop.maximum,
                ProgramId = workshop.ProgramId,
                Sessions = sessions,
                Type = workshop.type,
                WorkshopSetId = workshop.WorkShopSetId
            };
        }

        public static WorkshopDetail TranslateDetailLocal(WorkshopBooking booking)
        {
            return new WorkshopDetail()
            {
                Id = booking.workshopId,
                Title = booking.topic,
                Room = MiscServices.GetCampusLocal(booking.campusID),
                Time = HumanizeTimeSpan(booking.starting, booking.ending),
                DateHumanFriendly = HumanizeDate(booking.starting),
                TargetGroup = booking.targetingGroup ?? "N/A",
                Description = booking.description ?? "N/A",
                TotalPlaces = booking.maximum,
                Date = booking.starting,
                FilledPlaces = -1,
                Sessions = new List<SessionPreview>(),
                Type = booking.type,
                WorkshopSetId = booking.WorkShopSetID,
                Notes = booking.notes
            };
        }

        public static async Task<WorkshopDetail> TranslateDetail(WorkshopBooking booking)
        {
            var obj = TranslateDetailLocal(booking);
            obj.Room = await MiscServices.GetCampus(booking.campusID);
            return obj;
        }

        public static List<WorkshopPreview> TranslatePreview(List<Workshop> list)
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

        private static string HumanizeDate(DateTime starting, bool IncludeDay = false)
        {
            var humanized = starting.ToString("dd/MM/yyyy");
            bool Past = starting < DateTime.Now;

            if (starting < DateTime.UtcNow.AddDays(1) && !Past)
                humanized = "Today";
            else if (starting < DateTime.UtcNow.AddDays(2) && !Past)
                humanized = "Tomorrow";
            else if (IncludeDay)
                humanized = starting.DayOfWeek + " " + humanized;
            return humanized;
        }

        private static string HumanizeDate(DateTime starting, DateTime ending)
        {
            return starting.ToString("dd/MM/yyyy") + " - " + ending.ToString("dd/MM/yyyy");
        }

        private static string HumanizeTimeSpan(DateTime start, DateTime end)
        {
            return To12Hour(start.Hour).ToString() + " - " + To12Hour(end.Hour).ToString() + " " + Meridiem(end.Hour);
        }

        private static int To12Hour(int Hour)
        {
            return (Hour > 12) ? (Hour - 12) : Hour;
        }

        private static string Meridiem(int Hour)
        {
            return (Hour >= 12) ? "PM" : "AM";
        }
    }
}
