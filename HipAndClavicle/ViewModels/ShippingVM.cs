
namespace HipAndClavicle;

public class ShippingVM
{
    [Display(Name = "Order to Ship")]
    public Order OrderToShip { get; set; } = default!;
    public AppUser Customer { get; set; } = default!;
    public AppUser Merchant { get; set; } = default!;
    public ShipEngineSDK.CreateLabelFromShipmentDetails.Package NewPackage { get; set; } = new();
    public ShipEngineSDK.GetRatesWithShipmentDetails.Result? ShippingRates { get; set; }
    public ShipEngineSDK.ListCarriers.Service? SelectedService { get; set; } = new();
    public ShipEngineSDK.ListCarriers.Result? Carriers { get; set; }
    public ShipEngineSDK.Common.Enums.LabelFormat LabelFormat { get; set; } = LabelFormat.PDF;
    public ShipEngineSDK.Common.Enums.LabelLayout LabelLayout { get; set; } = LabelLayout.FourBySix;
    public UserSettings Settings { get; set; } = new();
    public DateTime ShipDate { get; set; }
}

