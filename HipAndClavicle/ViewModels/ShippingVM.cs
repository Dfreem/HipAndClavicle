using NUnit.Framework;

namespace HipAndClavicle;

public class ShippingVM
{
    [Display(Name = "Order to Ship")]
    public Order OrderToShip { get; set; } = default!;
    public AppUser Customer { get; set; } = default!;
    public AppUser Merchant { get; set; } = default!;
    public AdminSettings Settings { get; set; } = new();
    [Display(Name = "Value of Goods")]
    public decimal ValueOfGoods { get; set; }
    [Display(Name = "Shipping Rates")]
    public List<Rate> ShippingRates { get; set; } = new();
    public Rate? SelectedRate { get; set; }
    [Display(Name = "Package Weight")]
    public ParcelWeight ParcelWeight { get; set; } = new();
    [Display(Name = "Unit")]
    public UnitOfDimension? UnitOfMeasure { get; set; }
    [Display(Name = "Package Size")]
    public ParcelDimension PackageDimension { get; set; } = new();
    public List<Document> Documents { get; set; } = new();

}

