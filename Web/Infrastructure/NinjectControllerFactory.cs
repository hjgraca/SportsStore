using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Domain.Abstract;
using Domain.Concrete;
using Domain.Entities;
using Moq;
using Ninject;

namespace Web.Infrastructure
{
    public class NinjectControllerFactory : DefaultControllerFactory
    {
        private IKernel ninjectkernel;

        public NinjectControllerFactory()
        {
            ninjectkernel = new StandardKernel();
            this.AddBindings();
        }

        protected override IController GetControllerInstance(RequestContext requestContext, Type controllerType)
        {
            return controllerType == null ? null : (IController) ninjectkernel.Get(controllerType);
        }

        private void AddBindings()
        {
            // put bindings here

            ////Mock<IProductRepository> mock = new Mock<IProductRepository>();

            ////mock.Setup(x => x.Products).Returns(new List<Product>
            ////{
            ////    new Product{ Name = "Football", Price = 25},
            ////    new Product{ Name = "Surf Board", Price = 25},
            ////    new Product{ Name = "Running Shoes", Price = 25}
            ////}.AsQueryable());

            ////ninjectkernel.Bind<IProductRepository>().ToConstant(mock.Object);

            ninjectkernel.Bind<IProductRepository>().To<EFProductRepository>();
        }
    }
}