namespace HipAndClavicle;

public class ShippingVM
{
    [Display(Name = "Order to Ship")]
    public Order OrderToShip { get; set; } = default!;
    public AppUser Customer { get; set; } = default!;
    public AppUser Merchant { get; set; } = default!;
    public Package NewPackage { get; set; } = new();
    public AdminSettings Settings { get; set; } = new();
    [Display(Name = "Value of Goods")]
    public decimal ValueOfGoods { get; set; }
  // TODO Finish Shipping
}

