using Color = HipAndClavicle.Models.Color;

namespace HipAndClavicle.Models;

public class Product
{
    public int ProductId { get; set; }
    public string Name { get; set; } = default!;
    public ProductCategory Category { get; set; } = default!;
    public List<Color> ColorOptions { get; set; } = new();
    public bool InStock { get; set; } = default!;
    public int QuantityOnHand { get; set; } = default!;
    public List<Review> Reviews { get; set; } = new();
    public int? QuantityOrdered { get; set; } = default!;
    public List<SetSize> SetSizes { get; set; } = new();

}
