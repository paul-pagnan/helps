using Microsoft.WindowsAzure.Mobile.Service;
using System;

namespace helps.Service.DataObjects
{
    public class User : EntityData
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string StudentId { get; set; }
        public string Email { get; set; }
        public byte[] Salt { get; set; }
        public byte[] SaltedAndHashedPassword { get; set; }
        public string ConfirmToken { get; set; }
        public bool Confirmed { get; set; }
        public string ForgotPasswordToken { get; set; }
        public DateTime ResetTokenSentAt { get; set; }
       
    }
}