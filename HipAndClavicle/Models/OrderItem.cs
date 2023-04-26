namespace HipAndClavicle.Models
{
    public class OrderItem
    {
        public int OrderItemId { get; set; }
        public int OrderId { get; set; }
        public List<Color> ItemColor { get; set; } = new();
        public bool IsPulled { get; set; }
        public bool IsCut { get; set; }
        public bool IsFolded { get; set; }
        public bool IsStickered { get; set; }
        public Product Item { get; set; } = default!;
        public ProductCategory ItemType { get; set; } = default!;
        public int? SetSizeId { get; set; }
        public SetSize? SetSize { get; set; }
        public int AmountOrdered { get; set; }
        [Display(Name = "Item Price")]
        public double PricePerUnit { get; set; }
        /// <summary>
        /// an enum to determine if the price is per unit or per set
        /// </summary>
        [Display(Name = "Price Per")]
        public PriceUnit SetOrEach { get; set; } = PriceUnit.Unit;
        public enum PriceUnit { Unit, Set }
    }
}
