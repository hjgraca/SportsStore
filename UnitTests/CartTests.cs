using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Domain.Abstract;
using Domain.Entities;
using Moq;
using NUnit.Framework;
using Web.Controllers;
using Web.Models;

namespace UnitTests
{
    [TestFixture]
    public class CartTests
    {
        [Test]
        public void Can_Add_New_Lines()
        {
            // Arrange

            var p1 = new Product { ProductId = 1, Name = "P1" };
            var p2 = new Product { ProductId = 2, Name = "P2" };

            var target = new Cart();

            // Act

            target.AddItem(p1, 1);
            target.AddItem(p2, 1);

            var result = target.Lines.ToArray();

            // Assert

            Assert.AreEqual(result.Length, 2);
            Assert.AreEqual(result[0].Product, p1);
            Assert.AreEqual(result[1].Product, p2);
        }

        [Test]
        public void Can_Add_Quantity_For_Existing_Lines()
        {
            // Arrange
            var p1 = new Product { ProductId = 1, Name = "P1" };
            var p2 = new Product { ProductId = 2, Name = "P2" };
            var target = new Cart();

            // Act

            target.AddItem(p1, 1);
            target.AddItem(p2, 1);
            target.AddItem(p1, 10);

            var result = target.Lines.OrderBy(x => x.Product.ProductId).ToArray();

            Assert.AreEqual(result.Length, 2);
            Assert.AreEqual(result[0].Quantity, 11);
            Assert.AreEqual(result[1].Quantity, 1);
        }

        [Test]
        public void Can_Remove_Lines()
        {
            // Arrange
            var p1 = new Product { ProductId = 1, Name = "P1" };
            var p2 = new Product { ProductId = 2, Name = "P2" };
            var p3 = new Product { ProductId = 3, Name = "P3" };

            var target = new Cart();

            // Act

            target.AddItem(p1, 1);
            target.AddItem(p2, 3);
            target.AddItem(p3, 5);
            target.AddItem(p2, 1);

            target.RemoveLine(p2);

            Assert.AreEqual(target.Lines.Count(), 2);
            Assert.AreEqual(target.Lines.Count(p => p.Product == p2), 0);
        }

        [Test]
        public void Calculate_Cart_Total()
        {
            // Arrange
            var p1 = new Product { ProductId = 1, Name = "P1", Price = 100M };
            var p2 = new Product { ProductId = 2, Name = "P2", Price = 50M };

            var target = new Cart();

            // Act

            target.AddItem(p1, 1);
            target.AddItem(p2, 1);
            target.AddItem(p1, 3);

            var result = target.ComputeTotalValue();

            Assert.AreEqual(result, 450M);
        }

        [Test]
        public void Can_Clear_Contents()
        {
            // Arrange
            var p1 = new Product { ProductId = 1, Name = "P1", Price = 100M };
            var p2 = new Product { ProductId = 2, Name = "P2", Price = 50M };

            var target = new Cart();

            // Act

            target.AddItem(p1, 1);
            target.AddItem(p2, 1);

            target.Clear();

            Assert.AreEqual(target.Lines.Count(), 0);
        }

        private Mock<IProductRepository> mock;

        [SetUp]
        public void Setup()
        {
            mock = new Mock<IProductRepository>();

            mock.Setup(x => x.Products).Returns(new Product[]
            {
                new Product{ ProductId = 1, Name = "P1", Category = "Apples"}
            }.AsQueryable());
        }

        [Test]
        public void Can_Add_To_Cart()
        {
            // Arrange
            var cart = new Cart();
            var cartController = new CartController(mock.Object, null);

            // Act
            cartController.AddToCart(cart, 1, null);

            // Assert
            Assert.AreEqual(cart.Lines.Count(), 1);
            Assert.AreEqual(cart.Lines.ToArray()[0].Product.ProductId, 1);
        }

        [Test]
        public void Adding_Product_To_Cart_Goes_To_Cart_Screen()
        {
            // Arrange
            var cart = new Cart();
            var cartController = new CartController(mock.Object, null);

            // Act
            RedirectToRouteResult result = cartController.AddToCart(cart, 2, "myUrl");

            // Assert
            Assert.AreEqual(result.RouteValues["action"], "Index");
            Assert.AreEqual(result.RouteValues["returnUrl"], "myUrl");
        }

        [Test]
        public void Can_View_Cart_Contents()
        {
            // Arrange
            var cart = new Cart();
            var cartController = new CartController(null, null);

            // Act
            CartIndexViewModel result = (CartIndexViewModel) cartController.Index(cart, "myUrl").ViewData.Model;

            // Assert

            Assert.AreSame(result.Cart, cart);
            Assert.AreEqual(result.ReturnUrl, "myUrl");
        }

        [Test]
        public void Cannot_Checkout_Empty_Cart()
        {
            // Arrange - create a mock order processor 
            Mock<IOrderProcessor> mock = new Mock<IOrderProcessor>(); 
            
            // Arrange - create an empty cart 
            Cart cart = new Cart(); 
            
            // Arrange - create shipping details 
            ShippingDetails shippingDetails = new ShippingDetails(); 
            
            // Arrange - create an instance of the controller 
            CartController target = new CartController(null, mock.Object);

            // Act
            var result = target.Checkout(cart, shippingDetails);

            // Assert check that the order hasn't been passed on to the processor
            mock.Verify(m => m.ProcessOrder(It.IsAny<Cart>(), It.IsAny<ShippingDetails>()), Times.Never);

            // Assert - check that the method is returning the default view
            Assert.AreEqual("", result.ViewName);

            // Assert - check that we are passing an invalid model to the view
            Assert.AreEqual(false, result.ViewData.ModelState.IsValid);
        }

        [Test]
        public void Cannot_Checkout_Invalid_ShippingDetails()
        {
            // Arrange - create a mock order processor 
            Mock<IOrderProcessor> mock = new Mock<IOrderProcessor>();
            
            // Arrange - create cart with one item
            Cart cart = new Cart();
            cart.AddItem(new Product(), 1);

            // Arrange - create an instance of the controller 
            CartController target = new CartController(null, mock.Object);
            target.ModelState.AddModelError("error", "error");

            // Act
            var result = target.Checkout(cart, new ShippingDetails());

            // Assert check that the order hasn't been passed on to the processor
            mock.Verify(m => m.ProcessOrder(It.IsAny<Cart>(), It.IsAny<ShippingDetails>()), Times.Never);

            // Assert - check that the method is returning the default view
            Assert.AreEqual("", result.ViewName);

            // Assert - check that we are passing an invalid model to the view
            Assert.AreEqual(false, result.ViewData.ModelState.IsValid);
        }

        [Test]
        public void Can_Checkout_And_Submit_Order()
        {
            // Arrange - create a mock order processor 
            Mock<IOrderProcessor> mock = new Mock<IOrderProcessor>();

            // Arrange - create cart with one item
            Cart cart = new Cart();
            cart.AddItem(new Product(), 1);

            // Arrange - create an instance of the controller 
            CartController target = new CartController(null, mock.Object);

            // Act
            var result = target.Checkout(cart, new ShippingDetails());

            // Assert - check that the order has been passed on to the processor
            mock.Verify(m => m.ProcessOrder(It.IsAny<Cart>(), It.IsAny<ShippingDetails>()), Times.Once);

            // Assert - check that the method is returning the Completed view
            Assert.AreEqual("Completed", result.ViewName);

            // Assert - check that we are passing a valid model to the view
            Assert.AreEqual(true, result.ViewData.ModelState.IsValid);
        }
    }
}
