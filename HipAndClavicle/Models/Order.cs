
namespace HipAndClavicle.Models;

public class Order
{
    public int OrderId { get; set; }
    public List<OrderItem> Items { get; set; } = new();
    [Required]
    public string PurchaserId { get; set; } = default!;
    public AppUser Purchaser { get; set; } = default!;
    public OrderStatus Status { get; set; } = OrderStatus.Paid;
    public DateTime DateOrdered { get; set; } = DateTime.Now;
    public int? ShipmentId { get; set; }
    //public double TotalPrice { get; set; }
    public ShippingAddress Address{ get; set; } = default!;
    [NotMapped]
    public Dictionary<OrderStatus, string> StatusName
    {
        get
        {
            return new()
            {
                { OrderStatus.Received, "Order Received"
                },
                { OrderStatus.Paid, "Paid"
                },
                { OrderStatus.ReadyToShip, "Ready To Ship"
                },
                { OrderStatus.Shipped, "Shipped"
                },
                { OrderStatus.Late, "Late"
                }
            };
        }
    }

}
