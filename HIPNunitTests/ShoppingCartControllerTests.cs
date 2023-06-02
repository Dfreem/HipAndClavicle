using System.Security.Claims;
using HipAndClavicle.Controllers;
using HipAndClavicle.Models;
using HipAndClavicle.Repositories;
using HipAndClavicle.ViewModels;
using HIPNunitTests.Fakes;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Newtonsoft.Json;

namespace HIPNunitTests
{
    [TestFixture]
    public class ShoppingCartControllerTests
    {
        private IServiceProvider serviceProvider;
        private ShoppingCartController shoppingCartController;
        private FakeShoppingCartRepo fakeShoppingCartRepo;
        private FakeCustRepo fakeCustRepo;

        [SetUp]
        public void SetUp()
        {
            // Creates an instance of ServiceCollection to register services for Dependency Injection
            var serviceCollection = new ServiceCollection();

            // Creates a mock user store
            var userStoreMock = new Mock<IUserStore<AppUser>>();

            // Creates a new instance of UserManager with the mock user store
            var userManager = new UserManager<AppUser>(userStoreMock.Object, null, null, null, null, null, null, null, null);

            // Adds fake repositories to the service collection
            serviceCollection.AddScoped<IShoppingCartRepo, FakeShoppingCartRepo>();
            serviceCollection.AddScoped<ICustRepo, FakeCustRepo>();

            // Adds IHttpContextAccessor to the service collection
            serviceCollection.AddSingleton<IHttpContextAccessor>(new HttpContextAccessor());

            // Adds UserManager to the service collection
            serviceCollection.AddSingleton<UserManager<AppUser>>(userManager);

            // Builds the service provider
            serviceProvider = serviceCollection.BuildServiceProvider();

            // Gets the fake shopping cart repository
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
            // Creates a ClaimsPrincipal with the test user's Id. This will be used to mock the authenticated user in the HttpContext.
            var fakeClaimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId)
            }, "TestAuthenticationType"));

            // Mocks the HttpContext and set the User property to the fake ClaimsPrincipal
            var httpContextMock = new Mock<HttpContext>();
            httpContextMock.Setup(h => h.User).Returns(fakeClaimsPrincipal);

            // Mocks the HttpContextAccessor to return the mocked HttpContext
            var httpContextAccessorMock = new Mock<IHttpContextAccessor>();
            httpContextAccessorMock.Setup(hca => hca.HttpContext).Returns(httpContextMock.Object);

            // Instantiate the ShoppingCartController, passing in our mocked dependencies
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

            // Sets up a mock HttpContext that simulates the state of an unauthenticated user
            var httpContextMock = new Mock<HttpContext>();
            httpContextMock.Setup(h => h.User).Returns<ClaimsPrincipal>(null);

            // Mocks the HttpContext.Request property and cookies collection
            var httpRequestMock = new Mock<HttpRequest>();
            httpRequestMock.Setup(r => r.Cookies).Returns(Mock.Of<IRequestCookieCollection>());

            // Mocks the HttpResponse object and the Cookies collection
            var httpResponseMock = new Mock<HttpResponse>();
            var mockResponseCookies = new Mock<IResponseCookies>();
            httpResponseMock.Setup(r => r.Cookies).Returns(mockResponseCookies.Object);

            // Prepares to capture the value of the cookie that will be added in the response
            string cookieValue = null;
            mockResponseCookies.Setup(c => c.Append(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<CookieOptions>()
            )).Callback<string, string, CookieOptions>((name, value, options) => cookieValue = value);

            // Assigns the mocked Request and Response objects to the mocked HttpContext
            httpContextMock.Setup(h => h.Request).Returns(httpRequestMock.Object);
            httpContextMock.Setup(h => h.Response).Returns(httpResponseMock.Object);
            httpContextMock.Setup(h => h.User).Returns(new ClaimsPrincipal());

            // Mocks the HttpContextAccessor to return the mocked HttpContext
            var httpContextAccessorMock = new Mock<IHttpContextAccessor>();
            httpContextAccessorMock.Setup(hca => hca.HttpContext).Returns(httpContextMock.Object);

            // Initializes a ShoppingCartController with the necessary services and mocked HttpContextAccessor
            shoppingCartController = new ShoppingCartController(
                serviceProvider.GetRequiredService<IShoppingCartRepo>(),
                serviceProvider.GetRequiredService<ICustRepo>(),
                httpContextAccessorMock.Object);

            // Act
            var result = await shoppingCartController.AddToCart(testListingId, testQuantity);

            // Retrieves the shopping cart from the cookie.
            var shoppingCart = JsonConvert.DeserializeObject<SimpleShoppingCart>(cookieValue);

            // Asserts
            // Checks if the deserialized shopping cart contains the item that was added
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

            // Sets up a mock HttpContext that simulates the state of an unauthenticated user
            var httpContextMock = new Mock<HttpContext>();
            httpContextMock.Setup(h => h.User).Returns<ClaimsPrincipal>(null);

            // Defines a dictionary to act as cookie store
            Dictionary<string, string> mockCookies = new Dictionary<string, string>();

            // Prepares shopping cart with test item
            var initialShoppingCart = new SimpleShoppingCart
            {
                Items = new List<SimpleCartItem>
                {
                    new SimpleCartItem { Id = testListingId, Qty = testQuantity }
                }
            };
            mockCookies["Cart"] = JsonConvert.SerializeObject(initialShoppingCart);

            // Mocks the HttpContext.Request property and cookies collection
            var httpRequestMock = new Mock<HttpRequest>();
            httpRequestMock.Setup(r => r.Cookies[It.IsAny<string>()]).Returns((string key) => mockCookies.ContainsKey(key) ? mockCookies[key] : null);

            // Mocks Response and Cookies
            var httpResponseMock = new Mock<HttpResponse>();
            var mockResponseCookies = new Mock<IResponseCookies>();
            httpResponseMock.Setup(r => r.Cookies).Returns(mockResponseCookies.Object);

            // Sets up HttpResponse mock to interact with this cookie store
            mockResponseCookies.Setup(c => c.Append(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CookieOptions>()))
                .Callback<string, string, CookieOptions>((name, value, options) => mockCookies[name] = value);

            // Assigns the mocked Request and Response objects to the mocked HttpContext
            httpContextMock.Setup(h => h.Request).Returns(httpRequestMock.Object);
            httpContextMock.Setup(h => h.Response).Returns(httpResponseMock.Object);
            httpContextMock.Setup(h => h.User).Returns(new ClaimsPrincipal());

            // Sets up an unauthenticated user
            var anonymousIdentity = new ClaimsIdentity();
            var anonymousPrincipal = new ClaimsPrincipal(anonymousIdentity);
            httpContextMock.Setup(h => h.User).Returns(anonymousPrincipal);

            // Mocks the HttpContextAccessor to return the mocked HttpContext
            var httpContextAccessorMock = new Mock<IHttpContextAccessor>();
            httpContextAccessorMock.Setup(hca => hca.HttpContext).Returns(httpContextMock.Object);

            // Initializes a ShoppingCartController with the necessary services and mocked HttpContextAccessor
            shoppingCartController = new ShoppingCartController(
                serviceProvider.GetRequiredService<IShoppingCartRepo>(),
                serviceProvider.GetRequiredService<ICustRepo>(),
                httpContextAccessorMock.Object);

            // Sets the ControllerContext
            shoppingCartController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContextMock.Object
            };

            // Act
            await shoppingCartController.AddToCart(testListingId, testQuantity);
            var updateResult = await shoppingCartController.UpdateCart(testListingId, updatedQuantity);

            // Retrieves the shopping cart from the cookie.
            var shoppingCart = JsonConvert.DeserializeObject<SimpleShoppingCart>(mockCookies["Cart"]);

            // Asserts
            // Checks if the deserialized shopping cart contains the item that was updated
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

            string userId = "test user";
            // Mocks an HttpContext object
            var httpContextMock = new Mock<HttpContext>();

            // Set up an authenticated user
            var userClaims = new List<Claim>()
            {
                new Claim(ClaimTypes.NameIdentifier, userId),
            };

            // Creates a ClaimsIdentity object from the user claims
            var userIdentity = new ClaimsIdentity(userClaims, "TestAuthenticationType");
            // Creates a ClaimsPrincipal object from the ClaimsIdentity object
            var userPrincipal = new ClaimsPrincipal(userIdentity);
            // Assigns the ClaimsPrincipal to the User property of the HttpContext
            httpContextMock.Setup(h => h.User).Returns(userPrincipal);

            // Mocks the IHttpContextAccessor to return the mocked HttpContext
            var httpContextAccessorMock = new Mock<IHttpContextAccessor>();
            httpContextAccessorMock.Setup(hca => hca.HttpContext).Returns(httpContextMock.Object);

            // Initializes a ShoppingCartController with the necessary services and mocked HttpContextAccessor
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

        [Test]
        public async Task RemoveItemWhenUserIsLoggedIn()
        {
            // Arrange
            int itemId = 1;
            int testQuantity = 2;
            int itemId2 = 2;
            int testQuantity2 = 4;

            string userId = "test user";
            // Mocks an HttpContext object
            var httpContextMock = new Mock<HttpContext>();

            // Set up an authenticated user
            var userClaims = new List<Claim>()
            {
                new Claim(ClaimTypes.NameIdentifier, userId),
            };

            // Creates a ClaimsIdentity object from the user claims
            var userIdentity = new ClaimsIdentity(userClaims, "TestAuthenticationType");
            // Creates a ClaimsPrincipal object from the ClaimsIdentity object
            var userPrincipal = new ClaimsPrincipal(userIdentity);

            // Assigns the ClaimsPrincipal to the User property of the HttpContext
            httpContextMock.Setup(h => h.User).Returns(userPrincipal);

            // Mocks the IHttpContextAccessor to return the mocked HttpContext
            var httpContextAccessorMock = new Mock<IHttpContextAccessor>();
            httpContextAccessorMock.Setup(hca => hca.HttpContext).Returns(httpContextMock.Object);

            // Initializes a ShoppingCartController with the necessary services and mocked HttpContextAccessor
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
            await shoppingCartController.AddToCart(itemId, testQuantity);
            await shoppingCartController.AddToCart(itemId2, testQuantity2);
            shoppingCart = await fakeShoppingCartRepo.GetOrCreateShoppingCartAsync(userId, userId);
            Assert.IsTrue(shoppingCart.ShoppingCartItems.Any(item => item.ListingItem.ListingId == itemId));
            Assert.IsTrue(shoppingCart.ShoppingCartItems.Any(item => item.ListingItem.ListingId == itemId2));

            // Remove item from cart
            var initialItemCount = shoppingCart.ShoppingCartItems.Count;
            var removeResult = await shoppingCartController.RemoveFromCart(itemId);
            shoppingCart = await fakeShoppingCartRepo.GetOrCreateShoppingCartAsync(userId, userId);
            var finalItemCount = shoppingCart.ShoppingCartItems.Count;

            // Assert
            // Check that cart still exists
            Assert.IsNotNull(shoppingCart);
            // Check that item has been removed from cart
            var removedItem = shoppingCart.ShoppingCartItems.FirstOrDefault(item => item.ListingItem.ListingId == itemId);
            Assert.IsNull(removedItem);
            Assert.AreEqual(initialItemCount - 1, finalItemCount);
            Assert.IsInstanceOf<RedirectToActionResult>(removeResult);
        }

        [Test]
        public async Task RemoveItemWhenUserNotLoggedIn()
        {
            // Arrange
            int itemId = 1;
            int testQuantity = 2;
            int itemId2 = 2;
            int testQuantity2 = 4;

            // Mocks an HttpContext object
            var httpContextMock = new Mock<HttpContext>();
            httpContextMock.Setup(h => h.User).Returns<ClaimsPrincipal>(null);

            // Defines a dictionary to act as cookie store
            Dictionary<string, string> mockCookies = new Dictionary<string, string>();

            // Prepares an initial shopping cart with test items
            var initialShoppingCart = new SimpleShoppingCart
            {
                Items = new List<SimpleCartItem>
                {
                    new SimpleCartItem { Id = itemId, Qty = testQuantity },
                    new SimpleCartItem { Id = itemId2, Qty = testQuantity2 }
                }
            };
            // Serialize the shopping cart object to a string and add it to the mockCookies dictionary
            mockCookies["Cart"] = JsonConvert.SerializeObject(initialShoppingCart);

            // Mocks the HttpContext.Request property and its Cookies collection to return cookies from the mockCookies dictionary
            var httpRequestMock = new Mock<HttpRequest>();
            httpRequestMock.Setup(r => r.Cookies[It.IsAny<string>()]).Returns((string key) => mockCookies.ContainsKey(key) ? mockCookies[key] : null);

            // Mocks the HttpContext.Response property and its Cookies collection
            var httpResponseMock = new Mock<HttpResponse>();
            var mockResponseCookies = new Mock<IResponseCookies>();
            httpResponseMock.Setup(r => r.Cookies).Returns(mockResponseCookies.Object);

            // Sets up the HttpResponse mock to interact with the mockCookies dictionary
            mockResponseCookies.Setup(c => c.Append(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CookieOptions>()))
                .Callback<string, string, CookieOptions>((name, value, options) => mockCookies[name] = value);

            httpContextMock.Setup(h => h.Request).Returns(httpRequestMock.Object);
            httpContextMock.Setup(h => h.Response).Returns(httpResponseMock.Object);
            httpContextMock.Setup(h => h.User).Returns(new ClaimsPrincipal());

            // Sets up an unauthenticated user (anonymous user)
            var anonymousIdentity = new ClaimsIdentity();
            var anonymousPrincipal = new ClaimsPrincipal(anonymousIdentity);
            httpContextMock.Setup(h => h.User).Returns(anonymousPrincipal);

            // Mocks the IHttpContextAccessor to return the mocked HttpContext
            var httpContextAccessorMock = new Mock<IHttpContextAccessor>();
            httpContextAccessorMock.Setup(hca => hca.HttpContext).Returns(httpContextMock.Object);

            // Initializes a ShoppingCartController with the necessary services and mocked HttpContextAccessor
            shoppingCartController = new ShoppingCartController(
                serviceProvider.GetRequiredService<IShoppingCartRepo>(),
                serviceProvider.GetRequiredService<ICustRepo>(),
                httpContextAccessorMock.Object);

            // Assign the mocked HttpContext to the ControllerContext of the ShoppingCartController
            shoppingCartController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContextMock.Object
            };

            // Act

            // Deserialize the shopping cart string from the mockCookies dictionary back into a SimpleShoppingCart object
            var shoppingCart = JsonConvert.DeserializeObject<SimpleShoppingCart>(mockCookies["Cart"]);

            // Check if the deserialized shopping cart contains the items that are expected
            Assert.IsTrue(shoppingCart.Items.Any(item => item.Id == itemId && item.Qty == testQuantity));
            Assert.IsTrue(shoppingCart.Items.Any(item => item.Id == itemId2 && item.Qty == testQuantity2));

            // Remove an item from the shopping cart
            var removeResult = await shoppingCartController.RemoveFromCart(itemId);

            // Deserialize the updated shopping cart string from the mockCookies dictionary
            shoppingCart = JsonConvert.DeserializeObject<SimpleShoppingCart>(mockCookies["Cart"]);

            // Asserts

            // Assert that the removed item no longer exists in the shopping cart
            Assert.IsFalse(shoppingCart.Items.Any(item => item.Id == itemId));
            // Assert that the non-removed item still exists in the shopping cart
            Assert.True(shoppingCart.Items.Any(item => item.Id == itemId2));

            // Assert that the result of removing an item from the shopping cart is a redirection to "Index" action in "ShoppingCart" controller
            Assert.IsInstanceOf<RedirectToActionResult>(removeResult);
            var redirectToActionResult = removeResult as RedirectToActionResult;
            Assert.AreEqual("Index", redirectToActionResult.ActionName);
            Assert.AreEqual("ShoppingCart", redirectToActionResult.ControllerName);
        }

        [Test]
        public async Task ClearCartWhenUserIsLoggedIn()
        {
            // Arrange
            string userId = "test user";
            int itemId1 = 1;
            int itemId2 = 2;

            // Mocks an HttpContext object
            var httpContextMock = new Mock<HttpContext>();

            // Sets up an authenticated user
            var userClaims = new List<Claim>()
            {
                new Claim(ClaimTypes.NameIdentifier, userId),
            };
            var userIdentity = new ClaimsIdentity(userClaims, "TestAuthenticationType");
            var userPrincipal = new ClaimsPrincipal(userIdentity);

            // Setup mocked HttpContext to return the authenticated user
            httpContextMock.Setup(h => h.User).Returns(userPrincipal);

            // Mock IHttpContextAccessor to return the mocked HttpContext
            var httpContextAccessorMock = new Mock<IHttpContextAccessor>();
            httpContextAccessorMock.Setup(hca => hca.HttpContext).Returns(httpContextMock.Object);

            // Initialize ShoppingCartController with required services and the mocked HttpContextAccessor
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

            // Ensure the shopping cart exists and add items
            var shoppingCart = await fakeShoppingCartRepo.GetOrCreateShoppingCartAsync(userId, userId);
            Assert.IsNotNull(shoppingCart);
            await shoppingCartController.AddToCart(itemId1, 1);
            await shoppingCartController.AddToCart(itemId2, 2);
            shoppingCart = await fakeShoppingCartRepo.GetOrCreateShoppingCartAsync(userId, userId);

            // Check that items have been added to the cart
            Assert.IsTrue(shoppingCart.ShoppingCartItems.Any(item => item.ListingItem.ListingId == itemId1));
            Assert.IsTrue(shoppingCart.ShoppingCartItems.Any(item => item.ListingItem.ListingId == itemId2));

            // Act
            // Clear the shopping cart
            var clearResult = await shoppingCartController.ClearCart(userId);
            shoppingCart = await fakeShoppingCartRepo.GetOrCreateShoppingCartAsync(userId, userId);

            // Assert
            // Check that cart still exists but is empty
            Assert.IsNotNull(shoppingCart);
            Assert.IsEmpty(shoppingCart.ShoppingCartItems);
            Assert.IsInstanceOf<RedirectToActionResult>(clearResult);
        }

        [Test]
        public async Task ClearCartWhenUserNotLoggedIn()
        {
            // Arrange
            int itemId = 1;
            int testQuantity = 2;
            int itemId2 = 2;
            int testQuantity2 = 4;

            // Mocks an HttpContext object
            var httpContextMock = new Mock<HttpContext>();
            httpContextMock.Setup(h => h.User).Returns<ClaimsPrincipal>(null);

            // Define a dictionary to act as cookie store
            Dictionary<string, string> mockCookies = new Dictionary<string, string>();

            // Prepare shopping cart with test items
            var initialShoppingCart = new SimpleShoppingCart
            {
                Items = new List<SimpleCartItem>
                {
                    new SimpleCartItem { Id = itemId, Qty = testQuantity },
                    new SimpleCartItem { Id = itemId2, Qty = testQuantity2 }
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

            // Set up an unauthenticated user
            var anonymousIdentity = new ClaimsIdentity();
            var anonymousPrincipal = new ClaimsPrincipal(anonymousIdentity);
            httpContextMock.Setup(h => h.User).Returns(anonymousPrincipal);

            // Mocks the IHttpContextAccessor to return the mocked HttpContext
            var httpContextAccessorMock = new Mock<IHttpContextAccessor>();
            httpContextAccessorMock.Setup(hca => hca.HttpContext).Returns(httpContextMock.Object);

            // Initializes a ShoppingCartController with the necessary services and mocked HttpContextAccessor
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
            var clearResult = await shoppingCartController.ClearCart(null);

            // Retrieve the shopping cart from the cookie.
            var shoppingCart = JsonConvert.DeserializeObject<SimpleShoppingCart>(mockCookies["Cart"]);

            // Asserts
            // Check if the deserialized shopping cart is empty
            Assert.IsEmpty(shoppingCart.Items);

            // Verify if the method results in a redirection
            Assert.IsInstanceOf<RedirectToActionResult>(clearResult);
            var redirectToActionResult = clearResult as RedirectToActionResult;
            Assert.AreEqual("Index", redirectToActionResult.ActionName);
            Assert.AreEqual("ShoppingCart", redirectToActionResult.ControllerName);
        }
    }

    
}

