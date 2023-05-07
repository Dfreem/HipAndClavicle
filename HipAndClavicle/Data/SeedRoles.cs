namespace HipAndClavicle.Repositories;

public static class SeedRoles
{
    public static async Task SeedAdminRole(IServiceProvider services)
    {
        ApplicationDbContext context = services.GetRequiredService<ApplicationDbContext>();
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

        // only seed roles if not already done.
        if (await roleManager.RoleExistsAsync("Admin")) { return; }
        var userManager = services.GetRequiredService<UserManager<AppUser>>();

        // retreived seeded users to be admins
        var devin = await userManager.FindByNameAsync("dfreem987");
        var steven = await userManager.FindByNameAsync("steven123");
        var michael = await userManager.FindByNameAsync("michael123");
        string rolename = "Admin";

        await roleManager.CreateAsync(new IdentityRole(rolename));

        await userManager.AddToRoleAsync(devin!, rolename);
        await userManager.AddToRoleAsync(steven!, rolename);
        await userManager.AddToRoleAsync(michael!, rolename);

        AdminSettings settings = new()
        {
            Admin = devin!,
            AutoReply = true,
            AutoReplyMessage = "This is a test but I am not here",
            ContactBy = PreferredContact.Email,
            PurchaseRequiredForView = true,
            ShareContactInfo = false
        };

        context.Settings.Add(settings);
        context.SaveChanges();
    }

}

