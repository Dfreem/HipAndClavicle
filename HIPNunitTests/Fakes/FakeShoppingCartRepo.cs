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
        private List<ShoppingCartItem> _shoppingCartItems;
        private List<ShoppingCart> _shoppingCarts;

        public FakeShoppingCartRepo()
        {
            _shoppingCartItems = new List<ShoppingCartItem>();
            _shoppingCarts = new List<ShoppingCart>();
        }

        public async Task<ShoppingCart> GetOrCreateShoppingCartAsync(string cartId, string ownerId)
        {
            var shoppingCart = _shoppingCarts.FirstOrDefault(sc => sc.CartId == cartId);

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
                _shoppingCarts.Add(shoppingCart);
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
            var item = _shoppingCartItems.FirstOrDefault(i => i.ShoppingCartItemId == id);
            return Task.FromResult(item);
        }

        public async Task AddShoppingCartItemAsync(ShoppingCartItem item)
        {
            _shoppingCarts.FirstOrDefault(sc => sc.Id == item.ShoppingCartId)
                .ShoppingCartItems.Add(item);
            await Task.CompletedTask;
        }

        public async Task UpdateItemAsync(ShoppingCartItem item)
        {
            // Implement the fake logic for UpdateItemAsync if needed
            await Task.CompletedTask;
        }

        public async Task RemoveItemAsync(ShoppingCartItem item)
        {
            _shoppingCartItems.Remove(item);
            await Task.CompletedTask;
        }

        public async Task ClearShoppingCartAsync(string cartId, string ownerId)
        {
            _shoppingCartItems.Clear();
            await Task.CompletedTask;
        }

        public ShoppingCart GetCartByUser(string ownerId)
        {
            return _shoppingCarts.FirstOrDefault(sc => sc.Owner.Id == ownerId);
        }
    }
}
