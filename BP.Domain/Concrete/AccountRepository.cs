using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Security;
using BP.Domain.Abstract;
using System.Web;
using BP.Domain.Entities;

namespace BP.Domain.Concrete
{
    public class AccountRepository: IAccountRepository, IDisposable
    {
        private readonly BPContext _context;

        public AccountRepository(BPContext context)
        {
            _context = context;
        }
        
        public bool IsValidLogin(string username, string password)
        {
            var result = Membership.ValidateUser(username, password);
            if (result)
            {
                FormsAuthentication.SetAuthCookie(username,false);
            }

            return result;
        }


        public MembershipCreateStatus CreateUser(UserModel user)
        {
            MembershipCreateStatus createStatus;
            var newUser = Membership.CreateUser(user.Email, user.Password, user.Email, passwordQuestion: null, passwordAnswer: null, isApproved: true, providerUserKey: null, status: out createStatus);

            if (createStatus == MembershipCreateStatus.Success)
            {
                Roles.AddUserToRole(user.Email, user.Role);

                _context.UserProfiles.Add(new UserProfile
                    {
                        UserProfileId = Guid.NewGuid(),
                        UserId = (Guid)newUser.ProviderUserKey,
                        GivenName = user.GivenName,
                        FamilyName = user.FamilyName,
                        Phone = user.Phone,
                        OrganizationId = user.OrganizationId

                    });

                if (_context.BikePlanApplications.FirstOrDefault(b => b.OrganizationId == user.OrganizationId) == null)
                {
                    _context.BikePlanApplications.Add(new BikePlanApplication {OrganizationId = user.OrganizationId});
                }

                _context.SaveChanges();
      
                FormsAuthentication.SetAuthCookie(user.Email, createPersistentCookie: false);
               
            }

            return createStatus;
        }

        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing) _context.Dispose();
            }
            this.disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }


        public UserProfile GetUserProfileByUserName(string username)
        {
            var user = _context.Users.FirstOrDefault(u => u.UserName == username);
            if (user != null)
            {
                var userId = user.UserId;
                return _context.UserProfiles.FirstOrDefault(u => u.UserId == userId);
            }
            return null;
        }


        public IEnumerable<UserModel> GetUsers()
        {

            var user = from u in _context.Vw_UserDetails
                       select new UserModel
                                  {
                                      UserId = u.UserId,
                                      Email = u.Email,
                                      Password = u.Password,
                                      FamilyName = u.FamilyName,
                                      GivenName = u.GivenName,
                                      Phone = u.Phone,
                                      Role = u.RoleName,
                                      OrganizationId = u.OrganizationId,
                                      OrganizationName = u.OrganizationName,
                                  };
            return user;

        }


        public bool DeleteUser(Guid id, out string error)
        {
            error = string.Empty;
            try
            {
                var entityToDelete = _context.Users.FirstOrDefault(u => u.UserId == id);
                if (entityToDelete != null)
                {
                    _context.Users.Remove(entityToDelete);
                    _context.SaveChanges();
                }
                return true;
            }
            catch (Exception ex)
            {
                error = ((ex.InnerException).InnerException).Message;
                return false;
            }
        }


        public bool UpdateUser(UserModel userModel, out string error)
        {
            error = string.Empty;
            try
            {
                var entityToUpdate = _context.Users.FirstOrDefault(u => u.UserId == userModel.UserId);
                var userProfile = _context.UserProfiles.FirstOrDefault(u => u.UserId == userModel.UserId);
                if (entityToUpdate != null)
                {
                    var user = Membership.GetUser(entityToUpdate.UserName);
                    if (user != null)
                    {
                        user.Email = userModel.Email;
                        Membership.UpdateUser(user);
                    }

                    entityToUpdate.UserName = userModel.Email;
                }
                if (userProfile != null)
                {
                    userProfile.Phone = userModel.Phone;
                    userProfile.FamilyName = userModel.FamilyName;
                    userProfile.GivenName = userModel.GivenName;
                    userProfile.OrganizationId = userModel.OrganizationId;
                }

                _context.SaveChanges();

                return true;
            }
            catch (Exception ex)
            {
                error = ((ex.InnerException).InnerException).Message;
                return false;
            }
        }
    }
}
