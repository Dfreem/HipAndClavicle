using NUnit.Framework;

namespace HipAndClavicle;

public class ShippingVM
{
    public Order OrderToShip { get; set; } = default!;
    public AppUser Customer { get; set; } = default!;
    public AppUser Merchant { get; set; } = default!;
    public AdminSettings Settings { get; set; } = new();
    public ShippingAddress Address { get; set; } = default!;
    public Ship NewShipment { get; set; } = new();
    [Display(Name = "Package Weight")]
    public ParcelWeight ParcelWeight { get; set; } = default!;
    [Display(Name = "Unit")]
    public UnitOfDimension? UnitOfMeasure { get; set; }
    public decimal ParcelLength { get; set; }
    public decimal ParcelWidth { get; set; }
    public decimal ParcelHeight { get; set; }

    public static explicit operator Shipment(ShippingVM svm)
    {
        return new Shipment()
        {
            ToAddress = (Address)svm,
            FromAddress = new()
            {
                CityTown = svm.Merchant.Address!.CityTown,
                StateProvince = svm.Merchant.Address.StateAbr.ToString(),
                PostalCode = $"{svm.Merchant.Address.PostalCode}",
                CountryCode = "US",
                AddressLines = { svm.Merchant.Address.AddressLine1, svm.Merchant.Address.AddressLine2 },
                Name = svm.Merchant.FName + " " + svm.Merchant.LName
            },

        };
    }

    public static explicit operator Order(ShippingVM svm)
    {
        svm.OrderToShip.Address = svm.Address;
        return svm.OrderToShip;
    }

    public static explicit operator Address(ShippingVM svm)
    {
        return new Address()
        {
            CityTown = svm.Address.CityTown,
            StateProvince = svm.Address.StateAbr.ToString(),
            PostalCode = $"{svm.Address.PostalCode}",
            CountryCode = "US",
            AddressLines = { svm.Address.AddressLine1, svm.Address.AddressLine2 },
            Name = svm.Customer.FName + " " + svm.Customer.LName
        };

    }
}

