using System;
using BP.Domain.Abstract;
using BP.Domain.Entities;

namespace BP.Domain.Concrete
{
    public class UnitOfWork: IUnitOfWork
    {

        private readonly BPContext _context = new BPContext();
        private GenericRepository<Organization> _organizations;
        private AccountRepository _accountRepository;
        private IRepository<Milestone> _milestones;
        private IRepository<Step> _steps;
        private IRepository<Task> _tasks;
        private IRepository<BikePlanApplication> _bikePlanApplications;
        private IRepository<TaskOutcome> _taskOutcomes;
        private IRepository<vw_UserDetails> _vwUserDetails;
        
        private IRepository<UserProfile> _userProfile;

        public IRepository<Organization> Organizations
        {
            get {
                return _organizations ??
                       (_organizations = new GenericRepository<Organization>(_context));
            }
        }

        public IAccountRepository Accounts
        {
            get
            {
                return _accountRepository ??
                       (_accountRepository = new AccountRepository(_context));
            }
        }
        
        public void Commit()
        {
            _context.SaveChanges();
        }

        #region Disposable
        private bool _disposed;

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing) _context.Dispose();
            }
            _disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        } 
        #endregion

        
        public IRepository<Milestone> Milestones
        {
            get
            {
                return _milestones ??
                       (_milestones = new GenericRepository<Milestone>(_context));
            }
        }





        public IRepository<Step> Steps
        {
            get
            {
                return _steps ??
                    (_steps = new GenericRepository<Step>(_context));
            }
        }


        public IRepository<UserProfile> UserProfiles
        {
            get
            {
                return _userProfile ??
                   (_userProfile = new GenericRepository<UserProfile>(_context));
            }
        }


        public IRepository<Task> Tasks
        {
            get
            {
                return _tasks ??
                   (_tasks = new GenericRepository<Task>(_context));
            }
        }


        public IRepository<BikePlanApplication> BikePlanApplications
        {
            get
            {
                return _bikePlanApplications ??
                   (_bikePlanApplications = new GenericRepository<BikePlanApplication>(_context));
            }
        }


        public IRepository<TaskOutcome> TaskOutcomes
        {
            get
            {
                return _taskOutcomes ??
                   (_taskOutcomes = new GenericRepository<TaskOutcome>(_context));
            }
        }


        public IRepository<vw_UserDetails> VwUserDetails
        {
            get
            {
                return _vwUserDetails ??
                   (_vwUserDetails = new GenericRepository<vw_UserDetails>(_context));
            }
        }
    }
}
