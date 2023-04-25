namespace HipAndClavicle.Models;

public class Shippment
{
    [Key]
    public string Shipmentld { get; set; } = default!;
    public int OrderID { get; set; }
    public DateTime ShippedOn { get; set; } = DateTime.Now;
    [Required]
    public Address FromAddress { get; set; } = default!;
    public Parcel Package { get; set; } = default!;
    public List<Rate> Rates { get; set; } = default!;
    public Address ToAddress { get; set; } = default!;
    public List<CarrierPayment>? CarrierPayments { get; set; }
    public string? ParcelTrackingNumber { get; set; }
    public List<Parameter>? ShipmentOptions { get; set; }
    public string? ShipmentType { get; set; }
    public Address? SoldToAddress { get; set; }
}
