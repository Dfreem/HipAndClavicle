namespace HipAndClavicle.Models
{
    /// <summary>
    /// An order Item respresents a product that has been added to an order.
    /// </summary>
    public class OrderItem
    {
        /// <summary>
        /// The number of units ordered of this product. May be number of items or number of sets depending on <see cref="OrderItem.SetOrEach()"></see>/>
        /// </summary>
        public int AmountOrdered { get; set; } = 1;

        public int? ColorId { get; set; }
        public Product Item { get; set; } = default!;
        /// <summary>
        /// The color or colors that this item has been ordered in/>
        /// </summary>
        public List<Color> ItemColors { get; set; } = new();

        /// <summary>
        /// An enum separating products in to the typ that they are such as Dragons, Dragonflies or Butterflies.
        /// </summary>
        public ProductCategory ItemType { get; set; } = default!;

        public int OrderId { get; set; }
        public int OrderItemId { get; set; }
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
