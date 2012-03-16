using System.Data.Entity;
using BP.Domain.Entities;

namespace BP.Domain.Concrete
{
    public class BPContext: DbContext
    {
        public DbSet<Milestone> Milestones { get; set; }
        public DbSet<UserProfile> UserProfiles { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Organization> Organizations { get; set; }
        public DbSet<Step> Steps { get; set; }
        public DbSet<Task> Tasks { get; set; }
        public DbSet<BikePlanApplication> BikePlanApplications { get; set; }
        public DbSet<TaskOutcome> TaskOutcomes { get; set; }
        public DbSet<UserPermission> UserPermissions { get; set; }
        public DbSet<vw_UserDetails> Vw_UserDetails { get; set; }
    }
}