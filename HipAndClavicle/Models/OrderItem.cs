namespace HipAndClavicle.Models
{
    public class OrderItem
    {
        public int OrderItemId { get; set; }
        public List<Color> ItemColor { get; set; } = new();
        public bool IsPulled { get; set; }
        public bool IsCut { get; set; }
        public bool IsFolded { get; set; }
        public bool IsStickered { get; set; }
        public Product Item { get; set; } = default!;
<<<<<<< HEAD
        public ProductCategory ItemType { get; set; } = default!;
        public SetSize? SetSize { get; set; }
=======
        public ProductCategory ItemType { get; set; } = default!;
        public SetSize? SetSize { get; set; }

>>>>>>> 1f86abf4896a501a5e9520aa6c388dbe8d9ec0f5
    }
}
