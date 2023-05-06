namespace HipAndClavicle.ViewModels;

public class LoginVM
{
    [Required]
    public AppUser GetUser { get; set; } = default!;
    [Required(AllowEmptyStrings = true)]
    public string Password { get; set; } = "";
    [Required(AllowEmptyStrings = true)]
    public string UserName { get; set; } = "";
    [Required]
    [EmailAddress]
    public string Email { get; set; } = default!;
    public bool RememberMe { get; set; } = true;
    [Url]
    public string ReturnUrl { get; set; } = "";

    public LoginVM()
    {
        GetUser = new();
        UserName = GetUser.UserName;
        Email = GetUser.Email;
    }

    public LoginVM(AppUser currentUser)
    {
        GetUser = currentUser;
        UserName = currentUser.UserName;
        Email = currentUser.Email;
        RememberMe = currentUser.IsPersistent;
    }
}
