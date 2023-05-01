
namespace HipAndClavicle.ViewModels;

public class AddProductVM
{
    // UI Properties
    public List<SetSize> SetSizes { get; set; } = new();
    public Color NewColor { get; set; } = new();
    public SetSize NewSize { get; set; } = new();
    public List<Color> NamedColors { get; set; } = new();

    // Product Properties
    public string Name { get; set; } = default!;
    public ProductCategory Category { get; set; }
    public List<Color> ProductColors { get; set; } = new();
    public IFormFile ImageFile { get; set; } = default!;
    public Image? ProductImage { get; set; } = default!;
    public int QuantityOnHand { get; set; }

    public static explicit operator Product(AddProductVM v)
    {
        return new Product()
        {
            Name = v.Name,
            SetSizes = v.SetSizes,
            Category = v.Category,
            AvailableColors = v.ProductColors,
            ProductImage = v.ProductImage,
            QuantityOnHand = v.QuantityOnHand
        };
    }

}

