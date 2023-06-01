using NuGet.Packaging.Signing;
using System;
namespace HipAndClavicle.Models;

public class Color
{
    public int ColorId { get; set; }
    public string? ColorName { get; set; }
    public string HexValue { get; set; } = "#00000000";
    public int ColorFamilyId { get; set; }
    public List<ColorFamily> ColorFamilies { get; set; } = new List<ColorFamily>();
    
    // Color Functionality
    [NotMapped]
    public (int, int, int) RGB
    {

        get => (Red, Green, Blue);
        set => (Red, Green, Blue) = value;
    }

    [Range(0, 255)]
    public int Red { get; set; } = 0;
    [Range(0, 255)]
    public int Blue { get; set; } = 0;
    [Range(0, 255)]
    public int Green { get; set; } = 0;
    public int? ProductId { get; set; }
    public List<Product> ApplicableProducts { get; set; } = new();
    public int? OrderItemId { get; set; }
    public List<OrderItem> OrderItems { get; set; } = new();

}

