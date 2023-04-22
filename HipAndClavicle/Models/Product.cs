using Color = HipAndClavicle.Models.Color;

namespace HipAndClavicle.Models;

public class Product
{
    public int ProductId { get; set; }
    public string Name { get; set; } = default!;
    public ProductCategory Category { get; set; } = default!;
    public List<Color> Colors { get; set; } = new();
    public bool InStock { get; set; } = default!;
    [Display(Name = "# On-Hand")]
    public int QuantityOnHand { get; set; } = default!;
    public List<Review> Reviews { get; set; } = new();
    [Display(Name = "Set Sizes")]
    public List<SetSize> SetSizes { get; set; } = new();
    public Image? ProductImage { get; set; }
    [StringLength(250)]
    public string? Description { get; set; } = default!;
}
