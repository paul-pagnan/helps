namespace helps.Service.DataObjects
{
    public class ForgotPasswordRequest
    {
        public string ResetToken { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
    }
}