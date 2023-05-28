using HipAndClavicle.Controllers;
using HipAndClavicle.Data;
using HipAndClavicle.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using System.Security.Claims;
using NUnit.Framework;
using Moq;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;
using System;


namespace HipAndClavicle.UnitTests
{

    [TestFixture]
    public class ContactTests
    {
        private ContactController controller;
        private AppUser currentUser;

        [SetUp]
        public void Setup()
        {
            currentUser = new AppUser
            {
                LName = "Test",
                FName = "Test",
                UserName = "sender",
                Email = "sender@example.com",
                PhoneNumber = "123456789"
            };

            var userManagerMock = new Mock<UserManager<AppUser>>(Mock.Of<IUserStore<AppUser>>(), null, null, null, null, null, null, null, null);
            var contextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDb")
                .Options;
            var context = new ApplicationDbContext(contextOptions);
            context.Users.Add(currentUser);
            context.SaveChanges();

            userManagerMock.Setup(u => u.GetUsersInRoleAsync("Admin"))
                .ReturnsAsync(new List<AppUser>());

            controller = new ContactController(context, userManagerMock.Object);
        }

        [Test]
        public async Task SaveMessageWithValidCustomerMessageReturnsUserMessage()
        {
            // Arrange
            var customerMessage = new CustomerMessage
            {
                Message = "Test message",
                SendTo = "receiver"
            };

            // Set up mock for User.Identity.Name
            var userClaims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, "sender") // Provide the desired value for User.Identity.Name
            };
            var identityMock = new Mock<ClaimsIdentity>();
            identityMock.SetupGet(i => i.Name).Returns("sender");
            identityMock.SetupGet(i => i.Claims).Returns(userClaims);
            var principalMock = new Mock<ClaimsPrincipal>();
            principalMock.SetupGet(p => p.Identity).Returns(identityMock.Object);
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = principalMock.Object }
            };

            // Act
            var result = await controller.SaveMessage(customerMessage);

            // Assert
            Assert.That(result.Email, Is.EqualTo(currentUser.Email));
            Assert.That(result.Number, Is.EqualTo(currentUser.PhoneNumber));
            Assert.That(result.Content, Is.EqualTo(customerMessage.Message));
            Assert.That(result.SenderUserName, Is.EqualTo(currentUser.UserName));
            Assert.That(result.ReceiverUserName, Is.EqualTo(customerMessage.SendTo));
            Assert.That(result.DateSent, Is.LessThan(DateTime.Now.AddSeconds(1)));

        }

        [Test]
        public async Task CreateGuestUserMessageValidUserMessageVMReturnsUserMessage()
        {
            // Arrange
            var userMessageVM = new UserMessageVM
            {
                Email = "test@example.com",
                Number = "1234567890",
                Response = "Test message"
            };

            // Act
            var result = await controller.CreateGuestUserMessage(userMessageVM);

            // Assert
            Assert.NotNull(result);
            Assert.That(result.Email, Is.EqualTo(userMessageVM.Email));
            Assert.That(result.Number, Is.EqualTo(userMessageVM.Number));
            Assert.That(result.Content, Is.EqualTo(userMessageVM.Response));
            Assert.That(DateTime.Now - result.DateSent, Is.LessThan(TimeSpan.FromSeconds(1)));
        }
    }

}