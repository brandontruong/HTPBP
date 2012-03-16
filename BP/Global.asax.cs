using System.Reflection;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Security;
using BP.Domain.Abstract;
using BP.Domain.Concrete;
using BP.Infrastructure;
using AutoMapper;
using BP.ViewModels;
using BP.Domain.Entities;
using BP.ViewModels.Admin;
using BP.ViewModels.BikePlan;
using Ninject;
using Ninject.Modules;
using Ninject.Web.Mvc;

namespace BP
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : NinjectHttpApplication
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            routes.MapRoute(
                name: "BikePlan",
                url: "bikeplan/milestone{milestoneOrder}/step{stepOrder}",
                defaults: new { controller = "bikeplan", action = "milestone", milestoneOrder = "1", stepOrder ="1"}
            );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }

        //private IKernel _kernel = new StandardKernel(new MyNinjectModules());

        //protected void Application_Start()
        //{
        //    AreaRegistration.RegisterAllAreas();

        //    RegisterGlobalFilters(GlobalFilters.Filters);
        //    RegisterRoutes(RouteTable.Routes);

        //    BundleTable.Bundles.RegisterTemplateBundles();

        //    //Database.SetInitializer(new DropCreateDatabaseAlways<BPContext>());

        //    ControllerBuilder.Current.SetControllerFactory(new NinjectControllerFactory());

        //    SetupMapping();
            
        //    //_kernel.Inject(Membership.Provider);
        //}


        protected override void OnApplicationStarted()
        {
            AreaRegistration.RegisterAllAreas();
            RegisterGlobalFilters(GlobalFilters.Filters);
            BundleTable.Bundles.RegisterTemplateBundles();
            RegisterRoutes(RouteTable.Routes);
            //ViewEngines.Engines.Clear();
            SetupMapping();
            //ViewEngines.Engines.Add(new MobileCapableWebFormViewEngine());
            //RegisterAllControllersIn(Assembly.GetExecutingAssembly());
        }

        private void SetupMapping()
        {
            Mapper.CreateMap<RegisterViewModel, UserModel>();
            Mapper.CreateMap<UserModel, RegisterViewModel>();
            Mapper.CreateMap<Milestone, MilestoneViewModel>();
            Mapper.CreateMap<Step, StepViewModel>();
            Mapper.CreateMap<Task, TaskViewModel>();
            Mapper.CreateMap<TaskOutcome, TaskOutcomeViewModel>();
        }

        protected override Ninject.IKernel CreateKernel()
        {

            var modules = new INinjectModule[]
            {
                new ServiceModule()
            };

                    return new StandardKernel(modules);
        }
    }

    internal class ServiceModule : NinjectModule
    {
        public override void Load()
        {
            Bind<IEmailService>().To<EmailService>();
            Bind<IUnitOfWork>().To<UnitOfWork>();
            //Bind<IFormsAuthentication>().To<FormsAuthenticationService>();
            //Bind<IMembershipService>().To<AccountMembershipService>();
            //Bind<MembershipProvider>().ToConstant(Membership.Provider);
            //Bind<IDinnerRepository>().To<DinnerRepository>();
        }
    }
}