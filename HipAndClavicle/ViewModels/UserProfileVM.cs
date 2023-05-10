using System;
namespace HipAndClavicle.ViewModels
{
    public class UserProfileVM
    {
        public AppUser CurrentUser { get; set; } = default!;
        [PasswordPropertyText]
        public string? NewPassword { get; set; }
        [PasswordPropertyText]
        public string? ConfirmPassword { get; set; }
        [PasswordPropertyText]
        public string? CurrentPassword { get; set; }
    }
}

