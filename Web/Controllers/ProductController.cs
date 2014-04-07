using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Domain.Abstract;
using Web.Models;

namespace Web.Controllers
{
    public class ProductController : Controller
    {
        private IProductRepository repository;
        public int PageSize = 4;

        public ProductController(IProductRepository productRepository)
        {
            this.repository = productRepository;
        }

        //
        // GET: /Product/
        public ViewResult List(int page = 1)
        {
            var result = new ProductsListViewModel
            {
                Products = repository.Products
                .OrderBy(x => x.ProductId)
                .Skip(PageSize * (page - 1))
                .Take(PageSize),
                PagingInfo = new PagingInfo
                {
                    CurrentPage = page,
                    ItemsPerPage = PageSize,
                    TotalItems = repository.Products.Count()
                }
            };
            return View(result);
        }
    }
}
