namespace HipAndClavicle.Repositories
{
    public interface IAccountRepo
    {
        Task<ShippingAddress?> FindUserAddress(AppUser user);
        Task UpdateUserAddressAsync(AppUser user, ShippingAddress newAddress);
    }
}