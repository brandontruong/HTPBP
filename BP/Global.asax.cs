using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using BP.Infrastructure;
using AutoMapper;
using BP.ViewModels;
using BP.Domain.Entities;
using BP.ViewModels.Admin;
using BP.ViewModels.BikePlan;

namespace BP
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
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

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);

            BundleTable.Bundles.RegisterTemplateBundles();

            //Database.SetInitializer(new DropCreateDatabaseAlways<BPContext>());

            ControllerBuilder.Current.SetControllerFactory(new NinjectControllerFactory());

            SetupMapping();
            
            //_kernel.Inject(Membership.Provider);
        }

        private void SetupMapping()
        {
            Mapper.CreateMap<RegisterViewModel, UserModel>();
            Mapper.CreateMap<Milestone, MilestoneViewModel>();
            Mapper.CreateMap<Step, StepViewModel>();
            Mapper.CreateMap<Task, TaskViewModel>();
            Mapper.CreateMap<TaskOutcome, TaskOutcomeViewModel>();
        }
    }
}