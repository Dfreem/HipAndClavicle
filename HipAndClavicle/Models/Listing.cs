
namespace HipAndClavicle.Models
{
    public class Listing
    {
        public int ListingId { get; set; }
        public List<Image> Images { get; set; } = new();
        public double Price { get; set; } = default!;
<<<<<<< HEAD
        /// <summary>
        /// Use this list of Colors as a list of colors.
=======
        /// <summary>
        /// Use this list of Colors as a list of colors.
>>>>>>> 1f86abf4896a501a5e9520aa6c388dbe8d9ec0f5
        /// </summary>
        public List<Color> Colors { get; set; } = new();
        public Product ListingProduct { get; set; } = default!;
        [Range(0, int.MaxValue)]
        public int QuantitySold { get; set; } = default!;
        public DateTime DateCreated { get; } = DateTime.Now;
        [Range(0, int.MaxValue)]
        public int OnHand { get; set; } = default!;
        public string? shape { get; set; }

<<<<<<< HEAD
=======

>>>>>>> 1f86abf4896a501a5e9520aa6c388dbe8d9ec0f5
    }
}
