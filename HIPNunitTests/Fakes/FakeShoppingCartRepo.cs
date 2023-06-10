using HipAndClavicle.Models;
using HipAndClavicle.Repositories;
using HipAndClavicle.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIPNunitTests.Fakes
{
    public class FakeShoppingCartRepo : IShoppingCartRepo
    {
        private List<ShoppingCartItem> shoppingCartItems;
        private List<ShoppingCart> shoppingCarts;

        public FakeShoppingCartRepo()
        {
            shoppingCartItems = new List<ShoppingCartItem>();
            shoppingCarts = new List<ShoppingCart>();
        }

        public async Task<ShoppingCart> GetOrCreateShoppingCartAsync(string cartId, string ownerId)
        {
            var shoppingCart = shoppingCarts.FirstOrDefault(sc => sc.CartId == cartId);

            if (shoppingCart == null)
            {
                // If no such shopping cart exists, create a new one
                shoppingCart = new ShoppingCart
                {
                    CartId = cartId,
                    Owner = new AppUser { Id = ownerId},
                    ShoppingCartItems = new List<ShoppingCartItem>()
                };

                // And add it to the list of shopping carts
                shoppingCarts.Add(shoppingCart);
            }

            return await Task.FromResult(shoppingCart);
        }

        public Task<List<ShoppingCartItemViewModel>> GetShoppingCartItemsAsync(IEnumerable<ShoppingCartItem> items)
        {
            var viewModels = items
                .Select(item => new ShoppingCartItemViewModel(item))
                .ToList();

            return Task.FromResult(viewModels);
        }

        public Task<ShoppingCartItem> GetCartItem(int id)
        {
            var item = shoppingCartItems.FirstOrDefault(i => i.ShoppingCartItemId == id);
            return Task.FromResult(item);
        }

        public async Task AddShoppingCartItemAsync(ShoppingCartItem item)
        {
            var shoppingCart = shoppingCarts.FirstOrDefault(sc => sc.Id == item.ShoppingCartId);
            if (shoppingCart != null)
            {
                item.ShoppingCartItemId = shoppingCartItems.Count + 1;  // Set ShoppingCartItemId

                shoppingCartItems.Add(item); // Adding the item to shoppingCartItems list
                shoppingCart.ShoppingCartItems.Add(item); // Adding the item to ShoppingCart's Items
            }

            await Task.CompletedTask;
        }

        public async Task UpdateItemAsync(ShoppingCartItem item)
        {
            // Find the item that needs to be updated
            var itemToUpdate = shoppingCartItems.FirstOrDefault(i => i.ShoppingCartItemId == item.ShoppingCartItemId);

            if (itemToUpdate != null)
            {
                // Update the item
                itemToUpdate.Quantity = item.Quantity;
                itemToUpdate.ListingItem = item.ListingItem;
            }
            await Task.CompletedTask;
        }

        public async Task RemoveItemAsync(ShoppingCartItem item)
        {
            // Find the shopping cart the item belongs to
            var shoppingCart = shoppingCarts.FirstOrDefault(sc => sc.ShoppingCartItems.Contains(item));
            if (shoppingCart != null)
            {
                // Remove the item from the ShoppingCart's Items
                shoppingCart.ShoppingCartItems.Remove(item);
            }

            // Remove the item from shoppingCartItems list
            shoppingCartItems.Remove(item);

            await Task.CompletedTask;
        }

        public async Task ClearShoppingCartAsync(string cartId, string ownerId)
        {
            // Find the shopping cart with the given cartId and ownerId
            var shoppingCart = shoppingCarts.FirstOrDefault(sc => sc.CartId == cartId && sc.Owner.Id == ownerId);

            if (shoppingCart != null)
            {
                // Get the items in the cart
                var itemsInCart = shoppingCart.ShoppingCartItems;

                // Remove the items from the ShoppingCart's Items
                shoppingCart.ShoppingCartItems.Clear();

                // Remove the items from shoppingCartItems list
                foreach (var item in itemsInCart)
                {
                    shoppingCartItems.Remove(item);
                }
            }

            await Task.CompletedTask;
        }

        public ShoppingCart GetCartByUser(string ownerId)
        {
            return shoppingCarts.FirstOrDefault(sc => sc.Owner.Id == ownerId);
        }
    }
}
