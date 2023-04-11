namespace HipAndClavicle.Data;

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

    public static async Task SeedCustomerRole(IServiceProvider services)
    {
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = services.GetRequiredService<UserManager<AppUser>>();
        if (await roleManager.FindByNameAsync("Customer") is null)
        {
            await roleManager.CreateAsync(new IdentityRole("Customer"));
            var devin = await userManager.FindByNameAsync("dfreem987");
            var steven = await userManager.FindByNameAsync("steven123");
            var michael = await userManager.FindByNameAsync("michael123");
            var miah = await userManager.FindByNameAsync("nehemiah123");

            await userManager.AddToRoleAsync(devin, "Customer");
            await userManager.AddToRoleAsync(michael, "Customer");
            await userManager.AddToRoleAsync(steven, "Customer");
            await userManager.AddToRoleAsync(miah, "Customer");
        }
    }
}

