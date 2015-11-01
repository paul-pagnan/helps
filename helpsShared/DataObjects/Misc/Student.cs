using System;

namespace helps.Shared.DataObjects
{
    public class Student
    {
        public string studentID { get; set; }
        public DateTime? dob { get; set; }
        public string gender { get; set; }
        public string degree { get; set; }
        public string status { get; set; }
        public string first_language { get; set; }
        public string country_origin { get; set; }
        public string background { get; set; }
        public bool? HSC { get; set; }
        public string HSC_mark { get; set; }
        public bool? IELTS { get; set; }
        public string IELTS_mark { get; set; }
        public bool? TOEFL { get; set; }
        public string TOEFL_mark { get; set; }
        public bool? TAFE { get; set; }
        public string TAFE_mark { get; set; }
        public bool? CULT { get; set; }
        public string CULT_mark { get; set; }
        public bool? InsearchDEEP { get; set; }
        public string InsearchDEEP_mark { get; set; }
        public bool? InsearchDiploma { get; set; }
        public string InsearchDiploma_mark { get; set; }
        public bool? foundationcourse { get; set; }
        public string foundationcourse_mark { get; set; }
        public DateTime? created { get; set; }
        public int? creatorID { get; set; }
        public string degree_details { get; set; }
        public string alternative_contact { get; set; }
        public string preferred_name { get; set; }
    }
}
