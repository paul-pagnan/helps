using SQLite;
using System;

namespace helps.Shared.DataObjects
{
    public class SessionDetail : WorkshopDetail
    {
        public string LecturerEmail { get; set; }
        public string AssignmentType { get; set; }
        public string AppointmentType { get; set; }
        public bool Cancel { get; set; }
        public string Assistance { get; set; }
        public string Reason { get; set; }
        public int IsGroup { get; set; }
        public string LecturerComment { get; set; }
        public string Subject { get; set; }
        public string AssistanceText { get; set; }
    }
}