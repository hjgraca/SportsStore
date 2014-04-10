using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;
using NUnit.Framework;

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
    }
}
