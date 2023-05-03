var builder = WebApplication.CreateBuilder(args);
string? connectionString;
// Add services to the container.

connectionString = builder.Configuration.GetConnectionString("MYSQL_CONNECTION");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(connectionString, MySqlServerVersion.Parse("mysql-8.0")));

builder.Services.AddTransient<IAdminRepo, AdminRepo>();
builder.Services.AddTransient<ICustRepo, CustRepo>();
builder.Services.AddTransient <IShippingRepo, ShippingRepo>();
builder.Services.AddTransient<IShoppingCartRepo, ShoppingCartRepo>();
builder.Services.AddHttpContextAccessor();

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
    await SeedUsers.Seed(services);
    await SeedData.Seed(services, context);
    await SeedListings.Seed(services, context);
    await SeedRoles.SeedCustomerRole(services);
    await SeedRoles.SeedAdminRole(services);
    await SeedCustomers.Seed(services, context);
    await SeedShoppingCart.Seed(context, services);
}

app.Run();

