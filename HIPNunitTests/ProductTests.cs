using Microsoft.AspNetCore.Mvc;
using HipAndClavicle.Repositories;
using HipAndClavicle.Models;
using HipAndClavicle;
using Microsoft.Extensions.DependencyInjection;
using HipAndClavicle.ViewModels;
using Microsoft.AspNetCore.Identity;
using Moq;
using AspNetCoreHero.ToastNotification.Abstractions;
using HIPNunitTests.Fakes;
using System;
using Microsoft.AspNetCore.Http;

namespace HIPNunitTests
{
    public class ProductTests
    {
        private IServiceProvider _serviceProvider;
        private ProductController _productController;
        private FakeProductRepo _fakeProductRepo;

        [SetUp]
        public void SetUp()
        {
            var serviceCollection = new ServiceCollection();

            // Create a mock user store
            var userStoreMock = new Mock<IUserStore<AppUser>>();

            // Create a new instance of UserManager with the mock user store
            var userManager = new UserManager<AppUser>(userStoreMock.Object, null, null, null, null, null, null, null, null);

            // Add your fake product repository to the service collection
            serviceCollection.AddScoped<IProductRepo, FakeProductRepo>();
            serviceCollection.AddScoped<IAdminRepo, FakeAdminRepo>();
            serviceCollection.AddScoped<INotyfService, FakeNotyfService>();

            // Add UserManager to the service collection
            serviceCollection.AddSingleton<UserManager<AppUser>>(userManager);

            // Build the service provider
            _serviceProvider = serviceCollection.BuildServiceProvider();

            // Create the controller, passing in the service provider
            _productController = new ProductController(_serviceProvider);

            // Get the fake product repository
            _fakeProductRepo = _serviceProvider.GetService<IProductRepo>() as FakeProductRepo;
        }

        [Test]
        public async Task AddProduct_CreatesProduct_WhenModelStateIsValid()
        {
            // Arrange
            var testProductVM = new ProductVM();
            testProductVM.Edit = new Product();

            // Act
            await _productController.AddProduct(testProductVM);

            // Assert
            var createdProduct = await _fakeProductRepo.GetProductByIdAsync(testProductVM.Edit.ProductId);
            Assert.IsNotNull(createdProduct);
            // You can also add more assertions here to check that the product's properties are as expected.
        }

        [Test]
        public async Task EditProduct_UpdatesProduct_WhenProductExists()
        {
            // Arrange
            var testProduct = new Product() { ProductId = 1, Name = "Original Name" };
            await _fakeProductRepo.CreateProductAsync(testProduct);

            var updatedProduct = new Product() { ProductId = 1, Name = "Updated Name" };

            // Act
            await _productController.EditProduct(updatedProduct);

            // Assert
            var retrievedProduct = await _fakeProductRepo.GetProductByIdAsync(1);
            Assert.AreEqual("Updated Name", retrievedProduct.Name);
        }

        [Test]
        public async Task DeleteProduct_DeletesProduct_WhenProductExists()
        {
            // Arrange
            var testProduct = new Product() { ProductId = 1 };
            await _fakeProductRepo.CreateProductAsync(testProduct);

            // Act
            await _productController.DeleteProduct(1);

            // Assert
            var deletedProduct = await _fakeProductRepo.GetProductByIdAsync(1);
            Assert.IsNull(deletedProduct);
        }

        [Test]
        public async Task AddProduct_ReturnsViewWithModel_WhenModelStateIsInvalid()
        {
            // Arrange
            var testProductVM = new ProductVM()
            {
                // Add other properties if needed...
                Edit = new Product { }
            };
            _productController.ModelState.AddModelError("Error", "Model state is invalid");

            // Act
            var result = await _productController.AddProduct(testProductVM);

            // Assert
            Assert.IsInstanceOf<ViewResult>(result);
            var viewResult = result as ViewResult;
            Assert.That(viewResult.Model, Is.EqualTo(testProductVM));
        }

        [Test]
        public async Task EditProduct_ReturnsRedirectToAction_WhenModelStateIsInvalid()
        {
            // Arrange
            var testProduct = new Product
            {
                // Initialize properties as needed...
                ProductId = 1,
                Name = "Test Product",
                // Add other properties if needed...
            };
            await _fakeProductRepo.CreateProductAsync(testProduct);
            _productController.ModelState.AddModelError("Error", "Model state is invalid");

            // Act
            var result = await _productController.EditProduct(testProduct);

            // Assert
            Assert.IsInstanceOf<RedirectToActionResult>(result);
            var redirectResult = result as RedirectToActionResult;
            Assert.That(redirectResult.ActionName, Is.EqualTo("Products"));
            Assert.That(redirectResult.ControllerName, Is.EqualTo("Admin"));
        }

        [Test]
        public async Task DeleteProduct_ReturnsRedirectToAdminProducts()
        {
            // Arrange
            int testProductId = 1; // Set it as per your test data

            // Act
            var result = await _productController.DeleteProduct(testProductId);

            // Assert
            Assert.IsInstanceOf<RedirectToActionResult>(result);
            var redirectResult = result as RedirectToActionResult;

            // Verify that it redirects to the "Products" action of the "Admin" controller.
            Assert.AreEqual("Products", redirectResult.ActionName);
            Assert.AreEqual("Admin", redirectResult.ControllerName);
        }

    }
}