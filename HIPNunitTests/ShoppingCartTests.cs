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
            //await _fakeCustRepo.AddListingAsync(testListing);

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

            var httpResponseMock = new Mock<HttpResponse>();

            httpContextMock.Setup(h => h.Request).Returns(httpRequestMock.Object);
            httpContextMock.Setup(h => h.User).Returns(new ClaimsPrincipal());
            //httpContextMock.Setup(h => h.Response).Returns(httpResponseMock.Object);

            var httpContextAccessorMock = new Mock<IHttpContextAccessor>();
            httpContextAccessorMock.Setup(hca => hca.HttpContext).Returns(httpContextMock.Object);

            shoppingCartController = new ShoppingCartController(
                serviceProvider.GetRequiredService<IShoppingCartRepo>(),
                serviceProvider.GetRequiredService<ICustRepo>(),
                httpContextAccessorMock.Object);

            // Act
            var result = await shoppingCartController.AddToCart(testListingId, testQuantity);

            // Assert
            // Verify if the SetShoppingCartToCookie method is called with correct data
            httpContextMock.Verify(h => h.Response.Cookies.Append(
                It.IsAny<string>(),
                It.Is<string>(value => value.Contains(testListingId.ToString()) && value.Contains(testQuantity.ToString())),
                It.IsAny<CookieOptions>()
            ));

            // Verify if the method results in a redirection
            Assert.IsInstanceOf<RedirectToActionResult>(result);
            var redirectToActionResult = result as RedirectToActionResult;
            Assert.AreEqual("Index", redirectToActionResult.ActionName);
            Assert.AreEqual("ShoppingCart", redirectToActionResult.ControllerName);
        }

        // ... Other tests for EditCart, DeleteCart, and other methods in ShoppingCartController ...
    }

    
}

