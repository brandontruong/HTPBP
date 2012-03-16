using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Security;
using BP.Domain.Abstract;
using BP.Domain.Entities;
using BP.Domain.Helpers;

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


        public MembershipCreateStatus CreateUser(UserModel userModel, out string error)
        {
            // Check if there is already someone from that organization registered with the system
            error = string.Empty;
            var registerUser = GetUsers().FirstOrDefault(u => u.OrganizationName == userModel.OrganizationName && u.Role == RoleTypes.TeamLeader);
            
            if (registerUser != null && userModel.Role == RoleTypes.TeamLeader)
            {
                var fullName = string.Format("{0} {1}", registerUser.GivenName, registerUser.FamilyName);
                error =
                    string.Format(
                        "{0} has already registered for {1}. Please contact {0} to create an account for you", fullName,
                        registerUser.OrganizationName);
                return MembershipCreateStatus.UserRejected;
            }

            // Check if we have that organization registered
            var organization = _context.Organizations.FirstOrDefault(o => o.Name == userModel.OrganizationName);
            
            if (organization == null)
            {
                _context.Organizations.Add(new Organization { Name = userModel.OrganizationName, OrganizationId = Guid.NewGuid() });
                _context.SaveChanges();

                organization = _context.Organizations.FirstOrDefault(o => o.Name == userModel.OrganizationName);
            }
                
            MembershipCreateStatus createStatus;
            var newUser = Membership.CreateUser(userModel.Email, userModel.Password, userModel.Email, passwordQuestion: null, passwordAnswer: null, isApproved: true, providerUserKey: null, status: out createStatus);

            if (createStatus == MembershipCreateStatus.Success)
            {
                Roles.AddUserToRole(userModel.Email, userModel.Role);

                _context.UserProfiles.Add(new UserProfile
                    {
                        UserProfileId = Guid.NewGuid(),
                        UserId = (Guid)newUser.ProviderUserKey,
                        GivenName = userModel.GivenName,
                        FamilyName = userModel.FamilyName,
                        Phone = userModel.Phone,
                        OrganizationId = organization.OrganizationId

                    });

                if (_context.BikePlanApplications.FirstOrDefault(b => b.OrganizationId == organization.OrganizationId) == null)
                {
                    _context.BikePlanApplications.Add(new BikePlanApplication { BikePlanApplicationId = Guid.NewGuid(), OrganizationId = organization.OrganizationId });
                }

                _context.SaveChanges();
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
            var user = _context.Vw_UserDetails.FirstOrDefault(u => u.UserName == username);
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
                                      Company = u.Company,
                                      IsApproved = u.IsApproved
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
                        //user.ResetPassword();
                        Membership.UpdateUser(user);
                    }

                    entityToUpdate.UserName = userModel.Email;
                }
                if (userProfile != null)
                {
                    var userRoles = Roles.GetRolesForUser(userModel.Email);
                    if (!userRoles.Contains(userModel.Role))
                    {
                        Roles.RemoveUserFromRoles(userModel.Email, userRoles);
                        Roles.AddUserToRole(userModel.Email, userModel.Role);
                    }
                    userProfile.Phone = userModel.Phone;
                    userProfile.FamilyName = userModel.FamilyName;
                    userProfile.GivenName = userModel.GivenName;
                    userProfile.OrganizationId = userModel.OrganizationId;
                    userProfile.Company = userModel.Company;
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


        public bool ActivateUser(Guid userId, bool toBeActive, out string error)
        {
            try
            {
                error = string.Empty;
                var user = _context.Users.FirstOrDefault(u => u.UserId == userId);
                if (user != null)
                {
                    var member = Membership.GetUser(user.UserName);
                    if (member != null)
                    {
                        member.IsApproved = toBeActive;
                        Membership.UpdateUser(member);
                    }
                }
            }
            catch (Exception ex)
            {
                error = ex.Message;
                return false;
            }
           
            return true;
        }
    }
}
