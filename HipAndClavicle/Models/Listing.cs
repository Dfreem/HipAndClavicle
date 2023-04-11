
namespace HipAndClavicle.Models
{
    public class Listing
    {
        public int ListingId { get; set; }
        public List<Image> Images { get; set; } = new();
        public double Price { get; set; } = default!;
        /// <summary>
        /// Use this list of Colors as a list of colors.
        /// </summary>
        public List<Color> Colors { get; set; } = new();
        public Product ListingProduct { get; set; } = default!;
        [Range(0, int.MaxValue)]
        public int QuantitySold { get; set; } = default!;
        public DateTime DateCreated { get; } = DateTime.Now;
        [Range(0, int.MaxValue)]
        public int OnHand { get; set; } = default!;
        public string? shape { get; set; }


    }
}
