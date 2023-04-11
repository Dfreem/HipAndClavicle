namespace HipAndClavicle.ViewModels
{
    public class RegisterVM
    {
        [MaxLength(20)]
        [Required(ErrorMessage = "Please enter your name, should be under 20 characters")]
        public string FName { get; set; } = "";
        [MaxLength(20)]
        [Required(ErrorMessage = "Please enter your name, should be under 20 characters")]
        public string LName { get; set; } = "";
        [MaxLength(30)]
        [Required(ErrorMessage = "user name should be under 30 characters")]
        public string UserName { get; set; } = "";
        [Required(ErrorMessage = "Email is required for order communication")]
        public string Email { get; set; } = "";
        [Required]
        [PasswordPropertyText]
        public string Password { get; set; } = default!;
        [Required]
        [PasswordPropertyText]
        public string ConfirmPassword { get; set; } = default!;

        public string ReturnUrl { get; set; } = "";

        public RegisterVM()
        {
        }
    }
}

