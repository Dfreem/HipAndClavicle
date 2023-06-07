namespace HipAndClavicle.Models
{
    /// <summary>
    /// An order Item respresents a product that has been added to an order.
    /// </summary>
    public class OrderItem
    {
        public int OrderItemId { get; set; }
        /// <summary> Single items have a set set of 1. 
        /// This holds the number of sets of this item that was ordered, 
        /// even if the set size is one.</summary>
        public int AmountOrdered { get; set; } = 1;
        public Product Item { get; set; } = default!;

        public int? ColorId { get; set; }
        /// <summary>
        /// The color or colors that this item has been ordered in/>
        /// </summary>
        public List<Color> ItemColors { get; set; } = new();
                                    
        /// <summary>
        /// ex: Butterfly, dragon, dragonfly
        /// </summary>
        public ProductCategory ItemType { get; set; } = default!;

        public int OrderId { get; set; }
        /// <summary>
        /// The Order the this item belongs to
        /// </summary>
        public Order ParentOrder { get; set; } = default!;
        [Display(Name = "Item Price")]
        public double PricePerUnit { get; set; }

        public int ProductId { get; set; }       
        /// <summary>
        /// The set size that was ordered
        /// </summary>
        public SetSize? SetSize { get; set; }

        public int? SetSizeId { get; set; }
        public OrderStatus Status { get; set; }
    }
}
