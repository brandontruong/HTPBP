﻿using System.Data.Entity;
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
    }
}