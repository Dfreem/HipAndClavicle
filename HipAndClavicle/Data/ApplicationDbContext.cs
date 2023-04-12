namespace HipAndClavicle.Data;

public class ApplicationDbContext : IdentityDbContext<AppUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {

    }
    public ApplicationDbContext()
    {

    }
    public DbSet<Image> Images { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<Review> Reviews { get; set; }
    public DbSet<ShoppingCart> ShoppingCarts { get; set; }
    public DbSet<Color> NamedColors { get; set; }
    public DbSet<AdminSettings> Settings { get; set; }
    public DbSet<Listing> Listings { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }
    public DbSet<UserMessage> Messages { get; set; }
}

