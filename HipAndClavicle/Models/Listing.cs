
using HipAndClavicle.Models.JunctionTables;

namespace HipAndClavicle.Models
{
    public class Listing
    {
        public int ListingId { get; set; }
        public Image? SingleImage { get; set; }
        [NotMapped]
        public IFormFile? ImageFile { get; set; }
        public double Price { get; set; } = default!;
        public List<Color> Colors { get; set; } = new();
        public Product ListingProduct { get; set; } = default!;
        public string ListingTitle { get; set; } = default!;
        public string ListingDescription { get; set;} = default!;
        [Range(0, int.MaxValue)]
        public int QuantitySold { get; set; } = default!;
        public DateTime DateCreated { get; } = DateTime.Now;
        [Range(0, int.MaxValue)]
        public int OnHand { get; set; } = default!;
        public string? shape { get; set; }

        public List<ListingColorJT> ListingColorJTs { get; set; } = new List<ListingColorJT>();
        public int OrderItemId { get; set; }
        public List<OrderItem> OrderItems { get; set; } = new();
    }
}
