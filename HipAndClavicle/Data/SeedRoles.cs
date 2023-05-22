﻿namespace HipAndClavicle.Repositories;

public static class SeedRoles
{
    public static async Task SeedAdminRole(IServiceProvider services)
    {
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = services.GetRequiredService<UserManager<AppUser>>();
        var devin = await userManager.FindByNameAsync("dfreem987");
        var steven = await userManager.FindByNameAsync("steven123");
        var michael = await userManager.FindByNameAsync("michael123");
        var nehemiah = await userManager.FindByNameAsync("nehemiah123");
        var testAdmin = await userManager.FindByNameAsync("TestAdmin123");
        string rolename = "Admin";

        if (await roleManager.FindByNameAsync(rolename) is null)
        {
            await roleManager.CreateAsync(new IdentityRole(rolename));
        }

        await userManager.AddToRoleAsync(devin!, rolename);
        await userManager.AddToRoleAsync(steven!, rolename);
        await userManager.AddToRoleAsync(michael!, rolename);
        await userManager.AddToRoleAsync(nehemiah!, rolename);
        await userManager.AddToRoleAsync(testAdmin!, rolename);
    }
    /// <summary>
    /// This will be removed once messages can functon without it.
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static async Task KeepMessagesWorking(IServiceProvider services)
    {
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = services.GetRequiredService<UserManager<AppUser>>();
        var anne = await userManager.FindByNameAsync("anne");
        var anneMarie = await userManager.FindByNameAsync("anneMarie");
        var annie = await userManager.FindByNameAsync("annie");
        var ane = await userManager.FindByNameAsync("ane");
        var an = await userManager.FindByNameAsync("an");
        string rolename = "Customer";
        if (await roleManager.FindByNameAsync(rolename) is null)
        {
            await roleManager.CreateAsync(new IdentityRole(rolename));
        }
        await userManager.AddToRoleAsync(anne!, rolename);
        await userManager.AddToRoleAsync(anneMarie!, rolename);
        await userManager.AddToRoleAsync(annie!, rolename);
        await userManager.AddToRoleAsync(ane!, rolename);
        await userManager.AddToRoleAsync(an!, rolename);
    }

}

