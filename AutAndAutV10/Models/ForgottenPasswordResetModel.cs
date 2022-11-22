namespace AutAndAutV10.Models
{
    public class ForgottenPasswordResetModel
    {
        public string Email { get; set; }
        public string Password { get; set; }

        public string ConfirmPassword { get; set; }

        public string ForgottenPasswordToken { get; set; }
    }
}
