
namespace HipAndClavicle;

public class ShippingVM
{
    [Display(Name = "Order to Ship")]
    public Order OrderToShip { get; set; } = default!;
    public AppUser Customer { get; set; } = default!;
    public AppUser Merchant { get; set; } = default!;
    public ShipEngineSDK.CreateLabelFromRate.Package NewPackage { get; set; } = new();
    public ShipEngineSDK.GetRatesWithShipmentDetails.Result? ShippingRates { get; set; }
    public ShipEngineSDK.ListCarriers.Result? Carriers { get; set; }
    public int SelectedCarrier { get; set; }
    public AdminSettings Settings { get; set; } = new();
}

