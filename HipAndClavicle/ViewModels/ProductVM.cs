
namespace HipAndClavicle.ViewModels;

public class ProductVM
{
    // UI Properties
    public List<SetSize> SetSizes { get; set; } = new();
    public Color NewColor { get; set; } = new();
    public SetSize NewSize { get; set; } = new();
    public List<Color> NamedColors { get; set; } = new();

    // Product Properties
    public ProductCategory Category { get; set; }
    public List<ColorFamily> Families { get; set; } = new();
    public List<Color> ProductColors { get; set; } = new();
    public Image? ProductImage { get; set; } = default!;
    public IFormFile? ImageFile { get; set; }
    public Product? Edit { get; set; }

    public static explicit operator Product(ProductVM v)
    {
        if (v.Edit is null)
        {
            throw new ArgumentException("Cannot convert null to Product: ProductVM.Edit is null");
        }
        return v.Edit;
    }

}

