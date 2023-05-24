using Microsoft.AspNetCore.Mvc;
using HipAndClavicle.Repositories;
using HipAndClavicle.Models;
using HipAndClavicle;
using Microsoft.Extensions.DependencyInjection;
using HipAndClavicle.ViewModels;
using Microsoft.AspNetCore.Identity;
using Moq;
using HipAndClavicle.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.InMemory;
using AspNetCoreHero.ToastNotification;
using AspNetCoreHero.ToastNotification.Abstractions;
using AspNetCoreHero;
using AspNetCoreHero.ToastNotification.Extensions;
using AspNetCoreHero.ToastNotification.Abstractions;
using AspNetCoreHero.ToastNotification;
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

        // Test method
        [Test]
        public async Task ExtractImageAsync_ReturnsImageWithCorrectProperties()
        {
            // Arrange
            var imageFileMock = new Mock<IFormFile>();
            var memoryStream = new MemoryStream();

            // Set up the image file mock to return appropriate values
            var imageData = new byte[] { 1, 2, 3 }; // Replace with your test image data
            imageFileMock.Setup(f => f.CopyToAsync(memoryStream, default))
                .Callback(() =>
                {
                    // Simulate copying the image data to the memory stream
                    memoryStream.Write(imageData, 0, imageData.Length);
                    memoryStream.Seek(0, SeekOrigin.Begin);
                })
                .Returns(Task.CompletedTask);

            // Act
            var result = await _productController.ExtractImageAsync(imageFileMock.Object, 200);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(200, result.Width);
            Assert.AreEqual(imageData.Length, result.ImageData.Length);
            CollectionAssert.AreEqual(imageData, result.ImageData);
        }

        /*[Test]
        public async Task AddProduct_CallsCreateProductAsync_WhenModelStateIsValid()
        {
            // Arrange
            var testProductVM = new ProductVM();
            testProductVM.Edit = new Product();
            _mockProductRepo.Setup(repo => repo.CreateProductAsync(It.IsAny<Product>())).Returns(Task.CompletedTask);

            // Act
            await _productController.AddProduct(testProductVM);

            // Assert
            _mockProductRepo.Verify(repo => repo.CreateProductAsync(It.IsAny<Product>()), Times.Once);
        }

        [Test]
        public async Task AddProduct_ReturnsViewWithModel_WhenModelStateIsInvalid()
        {
            // Arrange
            var testProductVM = new ProductVM();
            _productController.ModelState.AddModelError("Error", "Model state is invalid");

            // Act
            var result = await _productController.AddProduct(testProductVM);

            // Assert
            Assert.IsInstanceOf<ViewResult>(result);
            var viewResult = result as ViewResult;
            Assert.AreEqual(testProductVM, viewResult.Model);
        }

        [Test]
        public async Task EditProduct_CallsUpdateProductAsync_WhenProductExists()
        {
            // Arrange
            var testProduct = new Product() { ProductId = 1 };
            _mockProductRepo.Setup(repo => repo.GetProductByIdAsync(1)).ReturnsAsync(testProduct);

            // Act
            await _productController.EditProduct(testProduct);

            // Assert
            _mockProductRepo.Verify(repo => repo.UpdateProductAsync(It.IsAny<Product>()), Times.Once);
        }

        [Test]
        public async Task EditProduct_RedirectsToProducts_WhenProductIsEdited()
        {
            // Arrange
            var testProduct = new Product() { ProductId = 1 };
            _mockProductRepo.Setup(repo => repo.GetProductByIdAsync(1)).ReturnsAsync(testProduct);

            // Act
            var result = await _productController.EditProduct(testProduct);

            // Assert
            Assert.IsInstanceOf<RedirectToActionResult>(result);
            var redirectResult = result as RedirectToActionResult;
            Assert.AreEqual("Products", redirectResult.ActionName);
            Assert.AreEqual("Admin", redirectResult.ControllerName);
        }

        [Test]
        public async Task DeleteProduct_CallsDeleteProductAsync_WhenProductExists()
        {
            // Arrange
            var testProduct = new Product() { ProductId = 1 };
            _mockProductRepo.Setup(repo => repo.GetProductByIdAsync(1)).ReturnsAsync(testProduct);

            // Act
            await _productController.DeleteProduct(1);

            // Assert
            _mockProductRepo.Verify(repo => repo.DeleteProductAsync(It.IsAny<Product>()), Times.Once);
        }

        [Test]
        public async Task DeleteProduct_RedirectsToProducts_WhenProductIsDeleted()
        {
            // Arrange
            var testProduct = new Product() { ProductId = 1 };
            _mockProductRepo.Setup(repo => repo.GetProductByIdAsync(1)).ReturnsAsync(testProduct);

            // Act
            var result = await _productController.DeleteProduct(1);

            // Assert
            Assert.IsInstanceOf<RedirectToActionResult>(result);
            var redirectResult = result as RedirectToActionResult;
            Assert.AreEqual("Products", redirectResult.ActionName);
            Assert.AreEqual("Admin", redirectResult.ControllerName);
        }*/
    }
}