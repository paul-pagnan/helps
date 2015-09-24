using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace helps.Shared.DataObjects
{
    public class User
    {
        [PrimaryKey]
        public string StudentId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public bool HasLoggedIn { get; set; }
        public string AuthToken { get; set; }        
    }
}
