using BP.Domain.Abstract;
using BP.Domain.Concrete;
using Ninject.Modules;


namespace BP.Controllers
{

    internal class MyNinjectModules : NinjectModule
    {
        public override void Load()
        {
            Bind<IAccountRepository>()
                .To<AccountRepository>();
        }
    }
}