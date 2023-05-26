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

        public FakeShoppingCartRepo()
        {
            _shoppingCartItems = new List<ShoppingCartItem>();
        }

        public Task<ShoppingCart> GetOrCreateShoppingCartAsync(string cartId, string ownerId)
        {
            // Implement the fake logic for GetOrCreateShoppingCartAsync if needed
            throw new NotImplementedException();
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
            _shoppingCartItems.Add(item);
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
    }
}
