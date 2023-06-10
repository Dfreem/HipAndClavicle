using System;
namespace HipAndClavicle.Models;

public abstract class HipItem
{
    public int Qty { get; set; } = 1;
    public int? ProductId { get; set; }
    public Product Item { get; set; } = default!;
    public int? ColorId { get; set; }
    /// <summary>
    /// The color or colors that this item has been ordered in/>
    /// </summary>
    public List<Color> ItemColors { get; set; } = new();
    //public int OrderId { get; set; }
    ///// <summary>
    ///// The Order the this item belongs to
    ///// </summary>
    //public Order ParentOrder { get; set; } = default!;
    [Display(Name = "Item Price")]
    public double PricePerUnit { get; set; }
    public int? SetSizeId { get; set; }
    public SetSize? SetSize { get; set; }
    public OrderStatus Status { get; set; }
    public List<Image> Images { get; set; } = new();
    public ItemType IType { get; set; }

    public static ListingItem ToListingItem(HipItem item)
    {
        return (ListingItem)item;
    }
    public static OrderItem ToOrderItem(HipItem item)
    {
        return (OrderItem)item;
    }
    public static ShoppingCartItem ToShoppingCartItem(HipItem item)
    {
        return (ShoppingCartItem)item;
    }
}



public enum ItemType
{
    listing = 1,
    shoppingCart = 2,
    order = 4
}