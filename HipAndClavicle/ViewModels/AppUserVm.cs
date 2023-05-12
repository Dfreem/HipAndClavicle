namespace HipAndClavicle.ViewModels
{
    public class AppUserVm
    {
        public string Id { get; set; }
        [Required]
        [MinLength(1, ErrorMessage = "first name must atleast have one character")]
        [MaxLength(20, ErrorMessage = "limit first name to 20 characters")]
        public string FName { get; set; } = default!;
        [Required]
        [MinLength(1, ErrorMessage = "first name must atleast have one character")]
        [MaxLength(20, ErrorMessage = "limit last name to 20 characters")]
        public string LName { get; set; } = default!;
        [Required]
        public string Email { get; set; }
        [Required]
        public string PhoneNumber { get; set; }


        public bool IsPersistent { get; set; } = true;
        public int? ShippingAddressId { get; set; }
        public ShippingAddress? Address { get; set; }
    }
}
