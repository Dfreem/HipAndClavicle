using NUnit.Framework;

namespace HipAndClavicle;

public class ShippingVM
{
    public Order OrderToShip { get; set; } = default!;
    public AppUser Customer { get; set; } = default!;
    public AppUser Merchant { get; set; } = default!;
    public AdminSettings Settings { get; set; } = new();
    public ShippingAddress Address { get; set; } = default!;
    public Ship NewShipment { get; set; } = new();

    public static explicit operator Shipment(ShippingVM svm)
    {
        return new Shipment()
        {
            CarrierPayments = 
        }
    }

}

