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
using Web.HtmlHelpers;
using Web.Models;

namespace UnitTests
{
    [TestFixture]
    public class UnitTest
    {
        private Mock<IProductRepository> mock;

        [SetUp]
        public void Setup()
        {
            mock = new Mock<IProductRepository>();

            mock.Setup(x => x.Products).Returns(new Product[]
            {
                new Product{ ProductId = 1, Name = "P1"}, 
                new Product{ ProductId = 2, Name = "P2"},
                new Product{ ProductId = 3, Name = "P3"},
                new Product{ ProductId = 4, Name = "P4"},
                new Product{ ProductId = 5, Name = "P5"}
            }.AsQueryable());
        }

        [Test]
        public void Can_Paginate()
        {
            // arrange
            var controller = new ProductController(mock.Object);
            controller.PageSize = 3;

            // Act
            var result = (ProductsListViewModel)controller.List(null, 2).Model;

            // Assert 
            Product[] prodArray = result.Products.ToArray(); 

            Assert.IsTrue(prodArray.Length == 2); 
            Assert.AreEqual(prodArray[0].Name, "P4"); 
            Assert.AreEqual(prodArray[1].Name, "P5");
        }

        [Test]
        public void Can_Generate_Page_Links()
        {
            // arrange
            HtmlHelper myHelper = null;

            PagingInfo pagingInfo = new PagingInfo
            {
                CurrentPage = 2,
                TotalItems = 28,
                ItemsPerPage = 10
            };

            // Act
            MvcHtmlString result = myHelper.PageLinks(pagingInfo, x => "Page" + x);

            // Assert 

            Assert.AreEqual(result.ToString(), @"<a href=""Page1"">1</a>" 
                + @"<a class=""selected"" href=""Page2"">2</a>" 
                + @"<a href=""Page3"">3</a>");
        }

        [Test]
        public void Can_Send_Pagination_View_Model()
        {
            // arrange

            var controller = new ProductController(mock.Object);
            controller.PageSize = 3;

            // Act
            var result = (ProductsListViewModel)controller.List(null, 2).Model;

            // Assert 
            var pagingInfo = result.PagingInfo;
            Assert.AreEqual(pagingInfo.CurrentPage, 2);
            Assert.AreEqual(pagingInfo.ItemsPerPage, 3);
            Assert.AreEqual(pagingInfo.TotalItems, 5);
            Assert.AreEqual(pagingInfo.TotalPages, 2);
        }
    }
}
