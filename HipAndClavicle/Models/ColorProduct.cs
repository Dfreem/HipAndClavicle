namespace HipAndClavicle.Models;

public class ColorProduct
{
    public int ColorId { get; set; }
    public Color GetColor { get; set; } = default!;
    public int ProductId { get; set; }
    public Product GetProduct { get; set; } = default!;
}
