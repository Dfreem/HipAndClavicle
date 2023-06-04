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
    public class FakeCustRepo : ICustRepo
    {
        private readonly List<Listing> _listings;
        private readonly List<Product> _products;
        private readonly List<Color> _colors;
        private readonly List<Order> _orders;
        private readonly List<ShoppingCart> _shoppingCarts;

        public FakeCustRepo()
        {
            _listings = new List<Listing>();
            _products = new List<Product>();
            _colors = new List<Color>();
            _orders = new List<Order>();
            _shoppingCarts = new List<ShoppingCart>();
        }

        public async Task<List<Listing>> GetAllListingsAsync()
        {
            return _listings;
        }

        public async Task<List<Product>> GetAllProductsAsync()
        {
            return _products;
        }

        public async Task<List<Color>> GetAllColorsAsync()
        {
            return _colors;
        }

        public async Task<Listing> GetListingByIdAsync(int listingId)
        {
            var listing = new Listing {ListingId = listingId};
            if (listingId == 1)
            {
                // Create a fake listing object with the necessary properties
                listing.ListingTitle = "Sample Listing";
                listing.ListingDescription = "Sample Description";
                listing.Price = 10.0;
                
            }
            else
            {
                // Create a fake listing object with the necessary properties
                listing.ListingTitle = "Sample Listing";
                listing.ListingDescription = "Sample of chaos";
                listing.Price = 80.0;
            }

            return listing;
        }

        public async Task<List<Color>> GetColorsByColorFamilyNameAsync(string colorFamilyName)
        {
            return _colors.Where(c => c.ColorFamilies.Any(cf => cf.ColorFamilyName == colorFamilyName)).ToList();
        }

        public async Task<List<Listing>> GetListingsByColorFamilyAsync(string colorFamilyName)
        {
            var colors = await GetColorsByColorFamilyNameAsync(colorFamilyName);
            return _listings.Where(l => l.Colors.Any(c => colors.Contains(c))).ToList();
        }

        public async Task<Color> GetColorByIdAsync(int searchColorId)
        {
            return _colors.FirstOrDefault(c => c.ColorId == searchColorId);
        }

        public async Task<Product> GetProductByIdAsync(int productId)
        {
            return _products.FirstOrDefault(p => p.ProductId == productId);
        }

        public async Task<List<Order>> GetOrdersByCustomerId(string customerId)
        {
            return _orders.Where(o => o.PurchaserId == customerId).ToList();
        }

        public async Task<Order> GetOrderById(int orderId)
        {
            return _orders.FirstOrDefault(o => o.OrderId == orderId);
        }

        public async Task<ShoppingCart> GetCartById(int cartId)
        {
            return _shoppingCarts.FirstOrDefault(c => c.Id == cartId);
        }

        public async Task<ShoppingCart> GetOrCreateShoppingCartAsync(string cartId)
        {
            var shoppingCart = _shoppingCarts.FirstOrDefault(c => c.CartId == cartId);
            if (shoppingCart == null)
            {
                shoppingCart = new ShoppingCart { CartId = cartId };
                _shoppingCarts.Add(shoppingCart);
            }
            return shoppingCart;
        }

        public async Task ClearShoppingCartAsync(string cartId)
        {
            var shoppingCart = _shoppingCarts.FirstOrDefault(c => c.CartId == cartId);
            if (shoppingCart != null)
            {
                shoppingCart.ShoppingCartItems.Clear();
            }
        }

        public async Task AddListingAsync(Listing listing)
        {
            _listings.Add(listing);
            await Task.CompletedTask;
        }

        // Methods not implemented for tests currently
        Task ICustRepo.AddColorFamilyAsync(ColorFamily colorFamily)
        {
            throw new NotImplementedException();
        }

        Task ICustRepo.AddListingImageAsync(Image image)
        {
            throw new NotImplementedException();
        }

        Task ICustRepo.AddColorToListing(Listing listing, Color color)
        {
            throw new NotImplementedException();
        }

        Task ICustRepo.AddReviewAsync(CustReviewVM crVM)
        {
            throw new NotImplementedException();
        }

        Task ICustRepo.AddOrderByCheckoutVmAsync(CheckoutVM checkoutVm)
        {
            throw new NotImplementedException();
        }

        Task<bool> ICustRepo.UserPurchasedProduct(int productId, AppUser currentUser)
        {
            throw new NotImplementedException();
        }

        Task<ShoppingCart> ICustRepo.GetCartByCustId(string custId)
        {
            throw new NotImplementedException();
        }
    }
}
