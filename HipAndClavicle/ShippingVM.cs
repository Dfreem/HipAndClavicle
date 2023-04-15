using System;
namespace HipAndClavicle;

public class ShippingVM
{
    public Order OrderToShip { get; set; } = default!;
    public AppUser Customer { get; set; } = default!;
    public AppUser Merchant { get; set; } = default!;
    public AdminSettings Settings { get; set; } = new();
}

