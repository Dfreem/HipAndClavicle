namespace HipAndClavicle.Models
{
    /// <summary>
    /// An order Item respresents a product that has been added to an order.
    /// </summary>
    public class OrderItem
    {
        public int OrderItemId { get; set; }
        public int OrderId { get; set; }
        /// <summary>
        /// The Order the this item belongs to
        /// </summary>
        public Order ParentOrder { get; set; } = default!;
        public int? ColorId { get; set; }
        /// <summary>
        /// The color or colors that this item has been ordered in/>
        /// </summary>
        public List<Color> ItemColors { get; set; } = new();
        public OrderStatus Status { get; set; }
        public int ProductId { get; set; }
        public Product Item { get; set; } = default!;
        /// <summary>
        /// An enum separating products in to the typ that they are such as Dragons, Dragonflies or Butterflies.
        /// </summary>
        public ProductCategory ItemType { get; set; } = default!;
        public int? SetSizeId { get; set; }
        /// <summary>
        /// The set size that was ordered
        /// </summary>
        public SetSize? SetSize { get; set; }
        /// <summary>
        /// The number of units ordered of this product. May be number of items or number of sets depending on <see cref="OrderItem.SetOrEach()"></see>/>
        /// </summary>
        public int AmountOrdered { get; set; } = 1;
        [Display(Name = "Item Price")]
        public double PricePerUnit { get; set; }
        ///// <summary>
        ///// an enum to determine if the price is per unit or per set
        ///// </summary>
        //[Display(Name = "Price Per")]
        //public PriceUnit SetOrEach { get; set; } = PriceUnit.Unit;
        /// <summary>
        /// This enum indicates if this item was ordered individually or as a set.
        /// </summary>
        public enum PriceUnit { Unit, Set }
    }
}
