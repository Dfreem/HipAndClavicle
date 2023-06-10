namespace HipAndClavicle.Models
{
    /// <summary>
    /// An order Item respresents a product that has been added to an order.
    /// </summary>
    public class OrderItem : HipItem
    {
        public int OrderItemId { get; set; }
    }
}
