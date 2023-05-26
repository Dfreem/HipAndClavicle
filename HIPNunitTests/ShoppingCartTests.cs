using System.Security.Claims;
using System.Threading.Tasks;
using HipAndClavicle.Controllers;
using HipAndClavicle.Repositories;
using HipAndClavicle.ViewModels;
using HIPNunitTests.Fakes;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;
using NUnit.Framework;

namespace HIPNunitTests
{
    [TestFixture]
    public class ShoppingCartTests
    {
        private ShoppingCartController _controller;
        private FakeShoppingCartRepo _fakeShoppingCartRepo;
        private FakeCustRepo _fakeCustRepo;
        private Mock<IHttpContextAccessor> _mockHttpContextAccessor;

        [SetUp]
        public void Setup()
        {
            // Create an instance of the fake repository
            _fakeShoppingCartRepo = new FakeShoppingCartRepo();
            _fakeCustRepo = new FakeCustRepo();

            // Create a mock for IHttpContextAccessor and set up the HttpContext
            _mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
            var httpContext = new DefaultHttpContext();
            httpContext.User = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
            new Claim(ClaimTypes.NameIdentifier, "userId"),
            new Claim(ClaimTypes.Name, "username"),
            // Add other claims if needed
            }));
            _mockHttpContextAccessor.Setup(x => x.HttpContext).Returns(httpContext);

            // Create an instance of the controller and pass the dependencies
            _controller = new ShoppingCartController(_fakeShoppingCartRepo, _fakeCustRepo, _mockHttpContextAccessor.Object);
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };
        }

        [Test]
        public async Task Index_ReturnsViewWithViewModel()
        {
            // Act
            var result = await _controller.Index() as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<ShoppingCartViewModel>(result.Model);
        }

        [Test]
        public async Task AddToCart_WithValidListingId_AddsItemToCart()
        {
            // Arrange
            int listingId = 1;
            int quantity = 2;
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, "userId"),
                new Claim(ClaimTypes.Name, "username"),
                // Add other claims if needed
            }, "mock"))
                }
            };

            // Act
            var result = await _controller.AddToCart(listingId, quantity) as RedirectToActionResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Index", result.ActionName);
            Assert.AreEqual("ShoppingCart", result.ControllerName);
        }

        [Test]
        public async Task UpdateCart_WithValidItemId_UpdatesCartItem()
        {
            // Arrange
            int itemId = 1;
            int quantity = 3;

            // Act
            var result = await _controller.UpdateCart(itemId, quantity) as RedirectToActionResult;

            // Assert
            //Assert.IsNotNull(result);
            Assert.AreEqual("Index", result.ActionName);

        }

        // Write similar tests for other methods in the ShoppingCartController
    }
}
