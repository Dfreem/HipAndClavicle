var builder = WebApplication.CreateBuilder(args);
string? connectionString;
// Add services to the container.

#region On Mac

if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
{
    connectionString = builder.Configuration.GetConnectionString("MYSQL_CONNECTION");
    builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseMySql(connectionString, MySqlServerVersion.Parse("mysql-8.0")));
}

#endregion

#region On Windows

else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows) && builder.Environment.IsDevelopment())
{
    connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseSqlite(connectionString));
}

#endregion

builder.Services.AddTransient<IHipRepo, HipRepo>();

#region Identity
builder.Services.AddIdentity<AppUser, IdentityRole>(options => 
    options.SignIn.RequireConfirmedAccount = false)
        .AddEntityFrameworkStores<ApplicationDbContext>()
        .AddDefaultTokenProviders();

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

#endregion

#region Toast Notifications

builder.Services.AddNotyf(configure =>
{
    configure.DurationInSeconds = 4;
    configure.HasRippleEffect = true;
    configure.IsDismissable = true;
    configure.Position = NotyfPosition.TopRight;
});

#endregion

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();
app.UseNotyf();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

using (var scope = app.Services.CreateAsyncScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<ApplicationDbContext>();
    await SeedData.Init(services, context);
    await SeedData.SeedUsers();
    await SeedData.SeedColors();
    await SeedData.SeedProducts();
    await SeedData.SeedItems();
    await SeedData.SeedOrders();
    await SeedRoles.SeedCustomerRole(services);
    await SeedRoles.SeedAdminRole(services);
}

app.Run();

