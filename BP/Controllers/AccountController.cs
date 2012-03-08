using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.Profile;
using System.Web.Security;
using BP.Helpers;
using BP.Infrastructure;
using BP.Models;
using BP.ViewModels;
using BP.Domain.Abstract;
using BP.Domain.Entities;
using AutoMapper;

namespace BP.Controllers
{

    [Authorize]
    public class AccountController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public AccountController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        //
        // GET: /Account/Login

        [AllowAnonymous]
        public ActionResult Login()
        {
            return ContextDependentView();
        }

        //
        // POST: /Account/JsonLogin

        [AllowAnonymous]
        [HttpPost]
        public JsonResult JsonLogin(LoginModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                
                if (_unitOfWork.Accounts.IsValidLogin(model.UserName, model.Password))
                {
                    FormsAuthentication.SetAuthCookie(model.UserName, model.RememberMe);
                    
                    var role = Roles.GetRolesForUser(model.UserName);

                    var userProfile = _unitOfWork.Accounts.GetUserProfileByUserName(model.UserName);
                    if (userProfile != null)
                    {
                        var profile = CustomProfile.GetProfile(model.UserName);

                        profile.FamilyName = userProfile.FamilyName;
                        profile.GivenName = userProfile.GivenName;
                        profile.Role = role[0];
                        profile.Organization = _unitOfWork.Organizations.GetById(userProfile.OrganizationId).Name;
                    }
           


                    if (role.Contains(RoleTypes.Admin))
                    {
                        var urlReferrer = (HttpContext.Request).UrlReferrer;
                        if (urlReferrer != null)
                            returnUrl = string.Format("{0}admin",urlReferrer.AbsoluteUri);
                    }
                    else if (role.Contains(RoleTypes.TeamLeader))
                    {
                        var urlReferrer = (HttpContext.Request).UrlReferrer;
                        if (urlReferrer != null)
                            returnUrl = string.Format("{0}teamleader", urlReferrer.AbsoluteUri);
                    }
                    else if (role.Contains(RoleTypes.TeamMember))
                    {
                        var urlReferrer = (HttpContext.Request).UrlReferrer;
                        if (urlReferrer != null)
                            returnUrl = string.Format("{0}teammember", urlReferrer.AbsoluteUri);
                    }
                    else if (role.Contains(RoleTypes.Buddy))
                    {
                        var urlReferrer = (HttpContext.Request).UrlReferrer;
                        if (urlReferrer != null)
                            returnUrl = string.Format("{0}buddy", urlReferrer.AbsoluteUri);
                    }

                    return Json(new { success = true, redirect = returnUrl });
                }
                else
                {
                    ModelState.AddModelError("", "The user name or password provided is incorrect.");
                }
            }

            // If we got this far, something failed
            return Json(new { errors = GetErrorsFromModelState() });
        }

        //
        // POST: /Account/Login

        [AllowAnonymous]
        [HttpPost]
        public ActionResult Login(LoginModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                if (_unitOfWork.Accounts.IsValidLogin(model.UserName, model.Password))
                {
                    //FormsAuthentication.SetAuthCookie(model.UserName, model.RememberMe);
                    var aa = _unitOfWork.Accounts.GetUserProfileByUserName(model.UserName);
                    if (Url.IsLocalUrl(returnUrl))
                    {
                        return Redirect(returnUrl);
                    }
                    return RedirectToAction("Index", "Home");
                }
                    ModelState.AddModelError("", "The user name or password provided is incorrect.");
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/LogOff

        public ActionResult LogOff()
        {
            FormsAuthentication.SignOut();

            return RedirectToAction("Index", "Home");
        }

        //
        // GET: /Account/Register

        [AllowAnonymous]
        public ActionResult Register()
        {
            return ContextDependentView();
        }

        //
        // POST: /Account/JsonRegister

        [AllowAnonymous]
        [HttpPost]
        public ActionResult JsonRegister(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Attempt to register the user
                MembershipCreateStatus createStatus;
                //Membership.CreateUser(model.UserName, model.Password, model.Email, passwordQuestion: null, passwordAnswer: null, isApproved: true, providerUserKey: null, status: out createStatus);
                Membership.CreateUser(model.Email, model.Password, model.Email, passwordQuestion: null, passwordAnswer: null, isApproved: true, providerUserKey: null, status: out createStatus);
                if (createStatus == MembershipCreateStatus.Success)
                {
                    FormsAuthentication.SetAuthCookie(model.Email, createPersistentCookie: false);
                    return Json(new { success = true });
                }
                else
                {
                    ModelState.AddModelError("", ErrorCodeToString(createStatus));
                }
            }

            // If we got this far, something failed
            return Json(new { errors = GetErrorsFromModelState() });
        }

        //
        // POST: /Account/Register

        [AllowAnonymous]
        [HttpPost]
        public ActionResult Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
               // Attempt to register the user
                var createStatus = _unitOfWork.Accounts.CreateUser(Mapper.Map<RegisterViewModel, UserModel>(model));

                if (createStatus == MembershipCreateStatus.Success)
                {
                    //Send confirmation email
                    try {
                        WebMail.SmtpServer = "smtp.gmail.com";
                    WebMail.SmtpPort = 587;
                    WebMail.EnableSsl = true;
                    WebMail.UserName = "admin@simpleit.somee.com";
                    WebMail.Password = "Eoo62oo8";
                    WebMail.From = "admin@simpleit.somee.com";

                    WebMail.Send(model.Email, "Registration Confirmation",
                        "Hi " + model.FamilyName + ", you are now member of the HTPBP online application.");
            
                    } catch (Exception) {
                        //@:<b>Sorry - we couldn't send the email to confirm your RSVP.</b> 
                    }
                    return RedirectToAction("Index", "Home");
                }
                ModelState.AddModelError("", ErrorCodeToString(createStatus));
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ChangePassword

        public ActionResult ChangePassword()
        {
            return View();
        }

        //
        // POST: /Account/ChangePassword

        [HttpPost]
        public ActionResult ChangePassword(ChangePasswordModel model)
        {
            if (ModelState.IsValid)
            {

                // ChangePassword will throw an exception rather
                // than return false in certain failure scenarios.
                bool changePasswordSucceeded;
                try
                {
                    MembershipUser currentUser = Membership.GetUser(User.Identity.Name, userIsOnline: true);
                    changePasswordSucceeded = currentUser.ChangePassword(model.OldPassword, model.NewPassword);
                }
                catch (Exception)
                {
                    changePasswordSucceeded = false;
                }

                if (changePasswordSucceeded)
                {
                    return RedirectToAction("ChangePasswordSuccess");
                }
                else
                {
                    ModelState.AddModelError("", "The current password is incorrect or the new password is invalid.");
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ChangePasswordSuccess

        public ActionResult ChangePasswordSuccess()
        {
            return View();
        }

        private ActionResult ContextDependentView()
        {
            string actionName = ControllerContext.RouteData.GetRequiredString("action");
            if (Request.QueryString["content"] != null)
            {
                ViewBag.FormAction = "Json" + actionName;
                return PartialView();
            }
            else
            {
                ViewBag.FormAction = actionName;
                return View();
            }
        }

        protected override void Dispose(bool disposing)
        {
            _unitOfWork.Dispose();
            base.Dispose(disposing);
        }

        private IEnumerable<string> GetErrorsFromModelState()
        {
            return ModelState.SelectMany(x => x.Value.Errors.Select(error => error.ErrorMessage));
        }

        #region Status Codes
        private static string ErrorCodeToString(MembershipCreateStatus createStatus)
        {
            // See http://go.microsoft.com/fwlink/?LinkID=177550 for
            // a full list of status codes.
            switch (createStatus)
            {
                case MembershipCreateStatus.DuplicateUserName:
                    return "User name already exists. Please enter a different user name.";

                case MembershipCreateStatus.DuplicateEmail:
                    return "A user name for that e-mail address already exists. Please enter a different e-mail address.";

                case MembershipCreateStatus.InvalidPassword:
                    return "The password provided is invalid. Please enter a valid password value.";

                case MembershipCreateStatus.InvalidEmail:
                    return "The e-mail address provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidAnswer:
                    return "The password retrieval answer provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidQuestion:
                    return "The password retrieval question provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidUserName:
                    return "The user name provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.ProviderError:
                    return "The authentication provider returned an error. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                case MembershipCreateStatus.UserRejected:
                    return "The user creation request has been canceled. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                default:
                    return "An unknown error occurred. Please verify your entry and try again. If the problem persists, please contact your system administrator.";
            }
        }
        #endregion
    }
}
