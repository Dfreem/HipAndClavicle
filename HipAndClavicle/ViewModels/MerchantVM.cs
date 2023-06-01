
namespace HipAndClavicle.ViewModels;

public class MerchantVM
{
    public AppUser Admin { get; set; } = default!;
    public List<Order> CurrentOrders { get; set; } = new();
    public List<Order> ShippedOrders { get; set; } = new();
    public Address FromAddress { get; set; } = default!;
    public List<Product> Products { get; set; } = new();
    public Product? Edit { get; set; }
    public IFormFile? ImageFile { get; set; }
    public ProductCategory Category { get; set; }
    public List<ColorFamily> Families { get; set; } = new();
    public List<Color> ProductColors { get; set; } = new();
    public Image? ProductImage { get; set; } = default!;
}