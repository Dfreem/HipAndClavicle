namespace HipAndClavicle.Models;

public class ProductImage
{
    public int ProductImageId { get; set; }
    public int ProductId { get; set; }
    public Product GetProduct { get; set; } = default!;
    public int ImageId { get; set; }
    public Image GetImage { get; set; } = default!;
}
