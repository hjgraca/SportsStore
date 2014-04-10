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
        public ViewResult List(string category, int page = 1)
        {
            var result = new ProductsListViewModel
            {
                Products = repository.Products
                .Where(p => category == null || p.Category == category)
                .OrderBy(x => x.ProductId)
                .Skip(PageSize * (page - 1))
                .Take(PageSize),
                PagingInfo = new PagingInfo
                {
                    CurrentPage = page,
                    ItemsPerPage = PageSize,
                    TotalItems = category == null ?  repository.Products.Count() : repository.Products.Count(p => p.Category == category)
                },
                CurrentCategory = category
            };
            return View(result);
        }
    }
}
