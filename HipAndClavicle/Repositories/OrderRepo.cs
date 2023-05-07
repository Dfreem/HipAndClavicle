using NuGet.Packaging.Signing;
using NuGet.Versioning;

namespace HipAndClavicle.Repositories;

public class OrderRepo : IOrderRepo
{
    readonly ApplicationDbContext _context;
    public OrderRepo(ApplicationDbContext context)
    {
        _context = context;
    }

    //public async Task CreateOrder(ShoppingCart cart)
    //{
    //    Order order = new Order()
    //    {
    //        Purchaser = cart.Owner,
    //        Address = cart.Owner.Address!,
    //        DateOrdered = DateTime.Now,
    //        Items =
    //    }

    //}

    public async Task<ShoppingCart> GetShoppingCartById(int id)
    {
        return await _context.ShoppingCarts
            .Include(sc => sc.Owner)
            .ThenInclude(o => o.Address)
            .Include(sc => sc.ShoppingCartItems)
            .ThenInclude(i => i.ListingItem)
            .ThenInclude(l => l.Colors)
            .Include(sc => sc.ShoppingCartItems)
            .ThenInclude(i => i.ListingItem)
            .ThenInclude(l => l.Images)
            .FirstAsync(sc => sc.Id == id);
    }

    //public static OrderItem ToOrderItem(this ShoppingCartItem scItem, Order parentOrder)
    //{
    //    return new OrderItem()
    //    {
    //        AmountOrdered = scItem.Quantity,
    //        Item = scItem.ListingItem.ListingProduct,
    //        ItemColors = scItem.ListingItem.Colors,
    //        ItemType = scItem.ListingItem.ListingProduct.Category,
    //        ParentOrder = parentOrder,
    //        PricePerUnit = scItem.ListingItem.Price,
    //        SetSize = scItem.ItemSetSize,
            
    //    }
    //}
}
