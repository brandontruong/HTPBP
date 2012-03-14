using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.Profile;
using System.Web.Security;
using BP.Domain.Helpers;
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

                    LoadProfile(model.UserName);

                    var role = Roles.GetRolesForUser(model.UserName);

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
                            returnUrl = string.Format("{0}bikeplan", urlReferrer.AbsoluteUri);
                    }
                    //else if (role.Contains(RoleTypes.TeamMember))
                    //{
                    //    var urlReferrer = (HttpContext.Request).UrlReferrer;
                    //    if (urlReferrer != null)
                    //        returnUrl = string.Format("{0}teammember", urlReferrer.AbsoluteUri);
                    //}
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

        private void LoadProfile(string username)
        {
            var userProfile = _unitOfWork.Accounts.GetUserProfileByUserName(username);
            var role = Roles.GetRolesForUser(username);
            if (userProfile != null)
            {
                var profile = CustomProfile.GetProfile(username);

                profile.FamilyName = userProfile.FamilyName;
                profile.GivenName = userProfile.GivenName;
                profile.Role = role[0];
                profile.Organization = _unitOfWork.Organizations.GetById(userProfile.OrganizationId).Name;
                profile.OrganizationId = userProfile.OrganizationId;
                var bikePlanApplication = _unitOfWork.BikePlanApplications.Get(b => b.OrganizationId == userProfile.OrganizationId).FirstOrDefault();
                if (bikePlanApplication != null) profile.BikePlanApplicationId = bikePlanApplication.BikePlanApplicationId;

            }
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

        [AllowAnonymous]
        public JsonResult GetOrganizations(string term)
        {
            //var organizations = _unitOfWork.Organizations.Get(o => o.Name.Contains(term)).Select(o => new { label = o.Name, value = o.OrganizationId });
            var organizations = _unitOfWork.Organizations.Get(o => o.Name.Contains(term)).Select(o => o.Name);
            return Json(organizations, JsonRequestBehavior.AllowGet);
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
                string error;

                // By default, only team lead can register because they are the first person from that organization to register
                model.Role = RoleTypes.TeamLeader;
                var createStatus = _unitOfWork.Accounts.CreateUser(Mapper.Map<RegisterViewModel, UserModel>(model), out error);

                if (createStatus == MembershipCreateStatus.Success)
                {
                    FormsAuthentication.SetAuthCookie(model.Email, createPersistentCookie: false);
                    return Json(new { success = true });
                }
                else
                {
                    //ModelState.AddModelError("", ErrorCodeToString(createStatus));
                    ModelState.AddModelError("", String.IsNullOrEmpty(error) ? Helper.ErrorCodeToString(createStatus) : error);
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
                string error;

                // By default, only team lead can register because they are the first person from that organization to register
                model.Role = RoleTypes.TeamLeader;

                var createStatus = _unitOfWork.Accounts.CreateUser(Mapper.Map<RegisterViewModel, UserModel>(model), out error);

                if (createStatus == MembershipCreateStatus.Success)
                {
                    FormsAuthentication.SetAuthCookie(model.Email, createPersistentCookie: false);

                    LoadProfile(model.Email);

                    //Send confirmation email
                    //try {
                    //    WebMail.SmtpServer = "smtp.gmail.com";
                    //    WebMail.SmtpPort = 587;
                    //    WebMail.EnableSsl = true;
                    //    WebMail.UserName = "admin@simpleit.somee.com";
                    //    WebMail.Password = "Eoo62oo8";
                    //    WebMail.From = "admin@simpleit.somee.com";

                    //    WebMail.Send(model.Email, "Registration Confirmation",
                    //        "Hi " + model.FamilyName + ", you are now member of the HTPBP online application.");
            
                    //} catch (Exception) {
                    //    //@:<b>Sorry - we couldn't send the email to confirm your RSVP.</b> 
                    //}

                    return RedirectToAction("Index", "BikePlan");
                }

                ModelState.AddModelError("", String.IsNullOrEmpty(error)? Helper.ErrorCodeToString(createStatus): error);
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

        
    }
}
