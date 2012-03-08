﻿using System;
using BP.Domain.Concrete;
using BP.Domain.Entities;

namespace BP.Domain.Abstract
{
    public interface IUnitOfWork: IDisposable
    {

        IRepository<Organization> Organizations { get; }
        IRepository<Milestone> Milestones { get; }
        IRepository<Step> Steps { get; }
        IRepository<UserProfile> UserProfiles { get; }
        IAccountRepository Accounts { get; }
     
        void Commit();

    }
}
