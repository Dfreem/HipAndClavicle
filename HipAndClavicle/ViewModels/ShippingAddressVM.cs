namespace HipAndClavicle.ViewModels
{

    public class ShippingAddressVM
    {
        public int ShippingAddressId { get; set; }


        [Display(Name = "Address Line 1")]
        [Required]
        public string AddressLine1 { get; set; } = default!;

        [Display(Name = "Address Line 2")]
        public string? AddressLine2 { get; set; } = default!;

        [Required]
        public string Country { get; set; } = "us";

        [DataType(DataType.PhoneNumber)]
        [Display(Name = "Phone Number")]
        [Required]
        public string? PhoneNumber { get; set; } = default!;

        [Display(Name = "City")]
        [Required]
        public string CityTown { get; set; } = default!;

        [Display(Name = "State")]
        [Required]
        public State StateAbr { get; set; } = default!;

        [DataType(DataType.PostalCode)]
        [Display(Name = "Zip-code")]
        [Required]
        public string PostalCode { get; set; } = default!;

        [Required]
        public bool Residential { get; set; }

        public string? AppUserId { get; set; }
        public AppUser? AppUser { get; set; }
    }
}