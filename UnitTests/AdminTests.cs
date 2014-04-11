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

namespace UnitTests
{
    [TestFixture]
    public class AdminTests
    {
        [Test]
        public void Can_Save_Valid_Changes()
        {
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            AdminController target = new AdminController(mock.Object);

            var product = new Product {Name = "Test"};

            ActionResult result = target.Edit(product);

            // Assert - check that the repository was called
            mock.Verify(m => m.SaveProduct(product));

            // Assert - check the method result type 
            Assert.IsNotInstanceOfType(typeof(ViewResult), result);
        }
    }
}
