using System;
namespace HipAndClavicle.Models;
public class Address
{
    public int AddressId { get; set; }
    public string Line1 { get; set; } = default!;
    public string? Line2 { get; set; } = default!;
    public string City { get; set; } = default!;
    public string State { get; set; } = default!;
    public int ZipCode { get; set; }
}

