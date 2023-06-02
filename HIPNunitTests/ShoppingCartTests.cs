using System.Security.Claims;
using System.Threading.Tasks;
using HipAndClavicle.Controllers;
using HipAndClavicle.Models;
using HipAndClavicle.Repositories;
using HipAndClavicle.ViewModels;
using HIPNunitTests.Fakes;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;

namespace HIPNunitTests
{
    [TestFixture]
    public class ShoppingCartTests
    {
        private IServiceProvider serviceProvider;
        private ShoppingCartController shoppingCartController;
        private FakeShoppingCartRepo fakeShoppingCartRepo;
        private FakeCustRepo fakeCustRepo;

        [SetUp]
        public void SetUp()
        {
            var serviceCollection = new ServiceCollection();

            // Create a mock user store
            var userStoreMock = new Mock<IUserStore<AppUser>>();

            // Create a new instance of UserManager with the mock user store
            var userManager = new UserManager<AppUser>(userStoreMock.Object, null, null, null, null, null, null, null, null);

            // Add your fake repositories to the service collection
            serviceCollection.AddScoped<IShoppingCartRepo, FakeShoppingCartRepo>();
            serviceCollection.AddScoped<ICustRepo, FakeCustRepo>();

            // Add IHttpContextAccessor to the service collection
            serviceCollection.AddSingleton<IHttpContextAccessor>(new HttpContextAccessor());

            // Add UserManager to the service collection
            serviceCollection.AddSingleton<UserManager<AppUser>>(userManager);

            // Build the service provider
            serviceProvider = serviceCollection.BuildServiceProvider();

            // Get the fake shopping cart repository
            fakeShoppingCartRepo = serviceProvider.GetService<IShoppingCartRepo>() as FakeShoppingCartRepo;
            fakeCustRepo = serviceProvider.GetService<ICustRepo>() as FakeCustRepo;
        }

        [Test]
        public async Task AddToCartWhenUserIsLoggedIn()
        {
            // Arrange
            int testListingId = 1;
            int testQuantity = 2;

            var testListing = await fakeCustRepo.GetListingByIdAsync(testListingId);

            string userId = "test user";
            var fakeClaimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId)
            }, "TestAuthenticationType"));

            var httpContextMock = new Mock<HttpContext>();
            httpContextMock.Setup(h => h.User).Returns(fakeClaimsPrincipal);

            var httpContextAccessorMock = new Mock<IHttpContextAccessor>();
            httpContextAccessorMock.Setup(hca => hca.HttpContext).Returns(httpContextMock.Object);

            shoppingCartController = new ShoppingCartController(
                serviceProvider.GetRequiredService<IShoppingCartRepo>(),
                serviceProvider.GetRequiredService<ICustRepo>(),
                httpContextAccessorMock.Object);

            // Ensure the shopping cart exists before adding an item
            var shoppingCart = await fakeShoppingCartRepo.GetOrCreateShoppingCartAsync(userId, userId);

            // Act
            var result = await shoppingCartController.AddToCart(testListingId, testQuantity);

            // Assert
            shoppingCart = await fakeShoppingCartRepo.GetOrCreateShoppingCartAsync(userId, userId);
            Assert.IsNotNull(shoppingCart);
            var addedItem = shoppingCart.ShoppingCartItems.FirstOrDefault(item => item.ListingItem.ListingId == testListingId);
            Assert.IsNotNull(addedItem);
            Assert.AreEqual(testQuantity, addedItem.Quantity);
            Assert.AreEqual(testQuantity, shoppingCart.ShoppingCartItems.Sum(item => item.Quantity));
            Assert.AreEqual(testQuantity * testListing.Price, shoppingCart.ShoppingCartItems.Sum(item => item.ListingItem.Price * item.Quantity));
            Assert.AreEqual(userId, shoppingCart.Owner.Id);
        }

        [Test]
        public async Task AddToCartWhenUserNotLoggedIn()
        {
            // Arrange
            int testListingId = 1;
            int testQuantity = 2;

            var testListing = await fakeCustRepo.GetListingByIdAsync(testListingId);

            var httpContextMock = new Mock<HttpContext>();
            httpContextMock.Setup(h => h.User).Returns<ClaimsPrincipal>(null);

            // Mock the HttpContext.Request property and cookies collection
            var httpRequestMock = new Mock<HttpRequest>();
            httpRequestMock.Setup(r => r.Cookies).Returns(Mock.Of<IRequestCookieCollection>());

            // Mock Response and Cookies
            var httpResponseMock = new Mock<HttpResponse>();
            var mockResponseCookies = new Mock<IResponseCookies>();
            httpResponseMock.Setup(r => r.Cookies).Returns(mockResponseCookies.Object);

            string cookieValue = null;
            mockResponseCookies.Setup(c => c.Append(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<CookieOptions>()
            )).Callback<string, string, CookieOptions>((name, value, options) => cookieValue = value);

            httpContextMock.Setup(h => h.Request).Returns(httpRequestMock.Object);
            httpContextMock.Setup(h => h.Response).Returns(httpResponseMock.Object);
            httpContextMock.Setup(h => h.User).Returns(new ClaimsPrincipal());

            var httpContextAccessorMock = new Mock<IHttpContextAccessor>();
            httpContextAccessorMock.Setup(hca => hca.HttpContext).Returns(httpContextMock.Object);

            shoppingCartController = new ShoppingCartController(
                serviceProvider.GetRequiredService<IShoppingCartRepo>(),
                serviceProvider.GetRequiredService<ICustRepo>(),
                httpContextAccessorMock.Object);

            // Act
            var result = await shoppingCartController.AddToCart(testListingId, testQuantity);

            // Retrieve the shopping cart from the cookie.
            var shoppingCart = JsonConvert.DeserializeObject<SimpleShoppingCart>(cookieValue);

            // Asserts
            // Check if the deserialized shopping cart contains the item that was added
            Assert.IsTrue(shoppingCart.Items.Any(item => item.ListingId == testListingId && item.Qty == testQuantity));

            // Verify if the method results in a redirection
            Assert.IsInstanceOf<RedirectToActionResult>(result);
            var redirectToActionResult = result as RedirectToActionResult;
            Assert.AreEqual("Index", redirectToActionResult.ActionName);
            Assert.AreEqual("ShoppingCart", redirectToActionResult.ControllerName);
        }

        [Test]
        public async Task UpdateCartWhenUserNotLoggedIn()
        {
            // Arrange
            int testListingId = 1;
            int testQuantity = 2;
            int updatedQuantity = 3;

            var testListing = await fakeCustRepo.GetListingByIdAsync(testListingId);

            var httpContextMock = new Mock<HttpContext>();
            httpContextMock.Setup(h => h.User).Returns<ClaimsPrincipal>(null);

            // Define a dictionary to act as cookie store
            Dictionary<string, string> mockCookies = new Dictionary<string, string>();

            // Prepare shopping cart with test item
            var initialShoppingCart = new SimpleShoppingCart
            {
                Items = new List<SimpleCartItem>
                {
                    new SimpleCartItem { Id = testListingId, Qty = testQuantity }
                }
            };
            mockCookies["Cart"] = JsonConvert.SerializeObject(initialShoppingCart);

            // Mock the HttpContext.Request property and cookies collection
            var httpRequestMock = new Mock<HttpRequest>();
            httpRequestMock.Setup(r => r.Cookies[It.IsAny<string>()]).Returns((string key) => mockCookies.ContainsKey(key) ? mockCookies[key] : null);

            // Mock Response and Cookies
            var httpResponseMock = new Mock<HttpResponse>();
            var mockResponseCookies = new Mock<IResponseCookies>();
            httpResponseMock.Setup(r => r.Cookies).Returns(mockResponseCookies.Object);

            // Set up HttpResponse mock to interact with this cookie store
            mockResponseCookies.Setup(c => c.Append(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CookieOptions>()))
                .Callback<string, string, CookieOptions>((name, value, options) => mockCookies[name] = value);

            httpContextMock.Setup(h => h.Request).Returns(httpRequestMock.Object);
            httpContextMock.Setup(h => h.Response).Returns(httpResponseMock.Object);
            httpContextMock.Setup(h => h.User).Returns(new ClaimsPrincipal());

            // Set up an unauthenticated user
            var anonymousIdentity = new ClaimsIdentity();
            var anonymousPrincipal = new ClaimsPrincipal(anonymousIdentity);
            httpContextMock.Setup(h => h.User).Returns(anonymousPrincipal);

            var httpContextAccessorMock = new Mock<IHttpContextAccessor>();
            httpContextAccessorMock.Setup(hca => hca.HttpContext).Returns(httpContextMock.Object);

            shoppingCartController = new ShoppingCartController(
                serviceProvider.GetRequiredService<IShoppingCartRepo>(),
                serviceProvider.GetRequiredService<ICustRepo>(),
                httpContextAccessorMock.Object);

            // Set the ControllerContext
            shoppingCartController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContextMock.Object
            };

            // Act
            await shoppingCartController.AddToCart(testListingId, testQuantity);
            var updateResult = await shoppingCartController.UpdateCart(testListingId, updatedQuantity);

            // Retrieve the shopping cart from the cookie.
            var shoppingCart = JsonConvert.DeserializeObject<SimpleShoppingCart>(mockCookies["Cart"]);

            // Asserts
            // Check if the deserialized shopping cart contains the item that was updated
            Assert.IsTrue(shoppingCart.Items.Any(item => item.Id == testListingId && item.Qty == updatedQuantity));

            // Verify if the method results in a redirection
            Assert.IsInstanceOf<RedirectToActionResult>(updateResult);
            var redirectToActionResult = updateResult as RedirectToActionResult;
            Assert.AreEqual("Index", redirectToActionResult.ActionName);
            Assert.AreEqual("ShoppingCart", redirectToActionResult.ControllerName);
        }

        [Test]
        public async Task UpdateCartWhenUserIsLoggedIn()
        {
            // Arrange
            int itemId = 1;
            int testQuantity = 2;
            int updatedQuantity = 3;

            var testListing = await fakeCustRepo.GetListingByIdAsync(itemId);

            string userId = "test user";
            var httpContextMock = new Mock<HttpContext>();

            // Set up an authenticated user
            var userClaims = new List<Claim>()
            {
                new Claim(ClaimTypes.NameIdentifier, userId),
                // add more claims as required
            };

            var userIdentity = new ClaimsIdentity(userClaims, "TestAuthenticationType");
            var userPrincipal = new ClaimsPrincipal(userIdentity);

            httpContextMock.Setup(h => h.User).Returns(userPrincipal);

            var httpContextAccessorMock = new Mock<IHttpContextAccessor>();
            httpContextAccessorMock.Setup(hca => hca.HttpContext).Returns(httpContextMock.Object);

            shoppingCartController = new ShoppingCartController(
            serviceProvider.GetRequiredService<IShoppingCartRepo>(),
            serviceProvider.GetRequiredService<ICustRepo>(),
            httpContextAccessorMock.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = httpContextMock.Object
                }
            };

            // Ensure the shopping cart exists before adding an item
            var shoppingCart = await fakeShoppingCartRepo.GetOrCreateShoppingCartAsync(userId, userId);

            // Act
            var addResult = await shoppingCartController.AddToCart(itemId, testQuantity);
            shoppingCart = await fakeShoppingCartRepo.GetOrCreateShoppingCartAsync(userId, userId);
            Assert.IsTrue(shoppingCart.ShoppingCartItems.Any(item => item.ListingItem.ListingId == itemId));
            var updatedResult = await shoppingCartController.UpdateCart(itemId, updatedQuantity);

            // Assert
            shoppingCart = await fakeShoppingCartRepo.GetOrCreateShoppingCartAsync(userId, userId);
            Assert.IsNotNull(shoppingCart);
            var updatedItem = shoppingCart.ShoppingCartItems.FirstOrDefault(item => item.ListingItem.ListingId == itemId);
            Assert.IsNotNull(updatedItem);
            Assert.AreEqual(updatedQuantity, updatedItem.Quantity);
        }

        // ... Other tests for EditCart, DeleteCart, and other methods in ShoppingCartController ...
    }

    
}

