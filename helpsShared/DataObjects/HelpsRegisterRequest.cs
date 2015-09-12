using System;

namespace helps.Shared.DataObjects
{
    public class HelpsRegisterRequest
    {
        public string StudentId { get; set; }
        public DateTime DateOfBirth { get; set; }
        public int Degree { get; set; }
        public int Status { get; set; }
        public string FirstLanguage { get; set; }
        public string CountryOrigin { get; set; }
    }
}
