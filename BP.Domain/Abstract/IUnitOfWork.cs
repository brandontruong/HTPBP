using System;
using BP.Domain.Concrete;
using BP.Domain.Entities;

namespace BP.Domain.Abstract
{
    public interface IUnitOfWork: IDisposable
    {

        IRepository<Organization> Organizations { get; }
        IRepository<Milestone> Milestones { get; }
        IRepository<Step> Steps { get; }
        IRepository<Task> Tasks { get; }
        IRepository<UserProfile> UserProfiles { get; }
        IRepository<BikePlanApplication> BikePlanApplications { get; }
        IRepository<TaskOutcome> TaskOutcomes { get; }
        IRepository<vw_UserDetails> VwUserDetails { get; }
        IAccountRepository Accounts { get; }
        
        void Commit();

    }
}
