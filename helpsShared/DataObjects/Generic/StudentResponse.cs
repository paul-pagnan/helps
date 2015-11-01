using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace helps.Shared.DataObjects
{
    public class StudentResponse : GenericResponse
    {
        public Student Student { get; set; }
        public bool IsSuccess { get; set; }
        public string DisplayMessage { get; set; }
    }
}
