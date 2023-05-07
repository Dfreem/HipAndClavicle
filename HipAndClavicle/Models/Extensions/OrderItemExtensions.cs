namespace HipAndClavicle.Models.Extensions;

public static class OrderItemExtensions
{
    public static OrderItem ToOrderItem(this ShoppingCartItem scItem, Order parentOrder)
    {
        return new OrderItem()
        {
            AmountOrdered = scItem.Quantity,
            Item = scItem.ListingItem.ListingProduct,
            ItemColors = scItem.ListingItem.Colors,
            ItemType = scItem.ListingItem.ListingProduct.Category,
            ParentOrder = parentOrder,
            PricePerUnit = scItem.ListingItem.Price,
            SetSize = scItem.ItemSetSize
        };
    }
}
