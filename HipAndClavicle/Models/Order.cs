
namespace HipAndClavicle.Models;

public class Order
{
    public int OrderId { get; set; }
    public List<OrderItem> Items { get; set; } = new();
    [Required]
    public string PurchaserId { get; set; } = default!;
    public AppUser Purchaser { get; set; } = default!;
    public bool IsPaid { get; set; }
    public bool IsShipped { get; set; }
    public DateTime DateOrdered { get; set; } = DateTime.Now;
    public int? ShipmentId { get; set; }
    public double TotalPrice { get; set; }
    public ShippingAddress ShippingAddress { get; set; } = default!;

}
