namespace HipAndClavicle.Repositories;

public static class SeedRoles
{
    public static async Task SeedAdminRole(IServiceProvider services)
    {
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = services.GetRequiredService<UserManager<AppUser>>();
        var devin = await userManager.FindByNameAsync("dfreem987");
        var steven = await userManager.FindByNameAsync("steven123");
        var michael = await userManager.FindByNameAsync("michael123");
        string rolename = "Admin";

        if (await roleManager.FindByNameAsync(rolename) is null)
        {
            await roleManager.CreateAsync(new IdentityRole(rolename));
        }

        await userManager.AddToRoleAsync(devin, rolename);
        await userManager.AddToRoleAsync(steven, rolename);
        await userManager.AddToRoleAsync(michael, rolename);
    }

}

