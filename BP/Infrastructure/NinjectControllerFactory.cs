using System;
using System.Web.Mvc;
using System.Web.Routing;
using BP.Domain.Abstract;
using BP.Domain.Concrete;
using Ninject;

namespace BP.Infrastructure
{
    public class NinjectControllerFactory: DefaultControllerFactory
    {
        private readonly IKernel _ninjectKernel;
        public NinjectControllerFactory()
        {
            _ninjectKernel = new StandardKernel();
            AddBindings();
        }

        protected override IController GetControllerInstance(RequestContext requestContext,Type controllerType)
        {
            return controllerType == null
                ? null
                : (IController)_ninjectKernel.Get(controllerType);
        }


        private void AddBindings()
        {
            // Mock implementation of the IProductRepository Interface
            //var mock = new Mock<IOrganizationRepository>();
            //mock.Setup(m => m.Organizations).Returns(new List<Organization>
            //                                        {
            //                                            new Organization
            //                                                {
            //                                                    Name = "Hurstville City Council",
            //                                                    OrganizationId =
            //                                                        new Guid("D6249537-E8F0-4629-A3C1-CB9A6B64BD54")
            //                                                },
            //                                        }.AsQueryable());
            //_ninjectKernel.Bind<IOrganizationRepository>().ToConstant(mock.Object);

            //_ninjectKernel.Bind<IOrganizationRepository>().To<OrganizationRepository>();
            //_ninjectKernel.Bind<IAccountRepository>().To<AccountRepository>();
            _ninjectKernel.Bind<IUnitOfWork>().To<UnitOfWork>();

        }
    }
}