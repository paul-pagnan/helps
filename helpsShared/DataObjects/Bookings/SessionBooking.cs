using SQLite;
using System;

namespace helps.Shared.DataObjects
{
    public class SessionBooking
    { 
        [PrimaryKey]
        public int BookingId { get; set; }
        public int SessionId { get; set; }
        public string LecturerFirstName { get; set; }
        public string LecturerLastName { get; set; }
        public string LecturerEmail { get; set; }
        public string SessionType { get; set; }
        public string AssignmentType { get; set; }
        public string AppointmentType { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Campus { get; set; }
        public bool Cancel { get; set; }
        public string Assistance { get; set; }
        public string Reason { get; set; }
        public int Attended { get; set; }
        public int? WaitingID { get; set; }
        public int IsGroup { get; set; }
        public string NumPeople { get; set; }
        public string LecturerComment { get; set; }
        public string AssignTypeOther { get; set; }
        public string Subject { get; set; }
        public string AssistanceText { get; set; }
        public string notes { get; set; }
        public DateTime LastUpdated { get; set; }

    }
}