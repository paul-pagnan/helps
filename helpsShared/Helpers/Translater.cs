using helps.Shared.DataObjects;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
                    Date = booking.starting,
                    WorkshopSetId = booking.WorkShopSetID,
                    Type = (booking.type == "single") ? "Workshop" : "Program",
                    Time = HumanizeTimeSpan(booking.starting, booking.ending),
                    DateHumanFriendly = HumanizeDate(booking.starting),
                    Location = await MiscServices.GetCampus(booking.campusID),
                    FilledPlaces = -1,
                    TotalPlaces = booking.maximum
                });
            }
            return translated;
        }

        public static async Task<List<WorkshopPreview>> TranslatePreview(List<SessionBooking> list)
        {
            List<WorkshopPreview> translated = new List<WorkshopPreview>();
            foreach (SessionBooking booking in list)
            {
                translated.Add(new WorkshopPreview
                {
                    Id = booking.SessionId,
                    Date = booking.StartDate,
                    Name = booking.LecturerFirstName + " " + booking.LecturerLastName,
                    WorkshopSetId = booking.SessionType.Length,
                    Type = booking.SessionType,
                    Time = HumanizeTimeSpan(booking.StartDate, booking.EndDate),
                    DateHumanFriendly = HumanizeDate(booking.StartDate),
                    Location = booking.Campus,

                });
            }
            return translated;
        }

        internal static SessionDetail TranslateSession(SessionBooking session)
        {
            var numPeople = -1;
            try
            {
                numPeople = int.Parse(session.NumPeople);
            } catch(Exception ex) { }

            return new SessionDetail()
            {
                Id = session.SessionId,
                Title = session.LecturerFirstName + " " + session.LecturerLastName,
                Room = session.Campus,
                Time = HumanizeTimeSpan(session.StartDate, session.EndDate),
                DateHumanFriendly =  HumanizeDate(session.StartDate),
                FilledPlaces = numPeople,
                Date = session.StartDate,
                DateEnd = session.EndDate,
                Type = session.AppointmentType,
                LecturerEmail = session.LecturerEmail,
                AssignmentType = session.AssignmentType + ((session.AssignTypeOther != null) ? Environment.NewLine + session.AssignTypeOther : null),
                AppointmentType = session.AppointmentType,
                Cancel = session.Cancel,
                Assistance = session.Assistance,
                Reason = session.Reason,
                IsGroup = session.IsGroup,
                LecturerComment = session.LecturerComment,
                Subject = session.Subject,
                AssistanceText = session.AssistanceText
            };
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
                DateEnd = workshop.EndDate,
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
                DateEnd = booking.ending,
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
                        Date = workshop.StartDate,
                        WorkshopSetId = workshop.WorkShopSetId,
                        Type = (workshop.type == "single") ? "Workshop" : "Program",
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
            return To12Hour(start) + " - " + To12Hour(end) + " " + Meridiem(end.Hour);
        }

        private static string FormatMinutes(DateTime dt)
        {
            if (dt.Minute > 0)
            {
                var formattedMinute = (dt.Minute >= 10) ? dt.Minute.ToString() : "0" + dt.Minute;
                return ":" + formattedMinute;
            }
            return "";
        }

        private static string To12Hour(DateTime dt)
        {
            var hour = (dt.Hour > 12) ? (dt.Hour - 12) : dt.Hour;
            return hour + FormatMinutes(dt);
        }

        private static string Meridiem(int Hour)
        {
            return (Hour >= 12) ? "PM" : "AM";
        }

    }
}
