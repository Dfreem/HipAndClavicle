
namespace HipAndClavicle.Models
{
    public class Listing
    {
        public int ListingId { get; set; }
        public List<Models.Image> Images { get; set; } = new();
        public double Price { get; set; } = default!;
        public List<Color> AvailableColors { get; set; } = new();
        public Product ListingProduct { get; set; } = default!;
        [Range(0, int.MaxValue)]
        public int QuantitySold { get; set; } = default!;
        public DateTime DateCreated { get; } = DateTime.Now;
        [Range(0, int.MaxValue)]
        public int OnHand { get; set; } = default!;
        

    }
}
