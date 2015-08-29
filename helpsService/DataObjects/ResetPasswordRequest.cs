namespace helps.Service.DataObjects
{
    public class ResetPasswordRequest
    {
        public string Errors { get; set; }
        public string ResetToken { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
    }
}