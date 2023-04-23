namespace HipAndClavicle.Models
{
    public class ShippingAddress
    {
        public int ShippingAddressId { get; set; }
        public string AddressLine1 { get; set; } = default!;
        public string? AddressLine2 { get; set; } = default!;
        public string Country { get; set; } = "USA";
        [DataType(DataType.PhoneNumber)]
        public string? PhoneNumber { get; set; } = default!;
        public string CityTown { get; set; } = default!;
        public State StateAbr { get; set; } = default!;
        [DataType(DataType.PostalCode)]
        public int PostalCode { get; set; } = default!;
    }
}
