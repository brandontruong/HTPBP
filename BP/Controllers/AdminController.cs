using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Security;
using AutoMapper;
using BP.Domain.Abstract;
using BP.Domain.Entities;
using BP.Infrastructure;
using BP.ViewModels;
using BP.ViewModels.Admin;

namespace BP.Controllers
{
    [Authorize(Roles = "admin")]
    public class AdminController : Controller
    {
        //
        // GET: /Admin/
        private readonly IUnitOfWork _unitOfWork;

        public AdminController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public ActionResult Index()
        {
            var viewmodel = new AdminViewModel
                                {
                                    Milestones =
                                        Mapper.Map<IEnumerable<Milestone>, IEnumerable<MilestoneViewModel>>(
                                            _unitOfWork.Milestones.Get()),
                                            CustomProfile =  CustomProfile.GetProfile(Profile.UserName),
                                };

            return View(viewmodel);
        }

        public ActionResult Register()
        {
            ViewBag.Organizations = _unitOfWork.Organizations.Get();
            return View();
        }

        protected override void Dispose(bool disposing)
        {
            _unitOfWork.Dispose();
            base.Dispose(disposing);
        }

        public ActionResult Milestone(Guid milestoneId, Guid? stepId)
        {
            var viewModel = new AdminViewModel
            {
                Milestones =
                    Mapper.Map<IEnumerable<Milestone>, IEnumerable<MilestoneViewModel>>(
                        _unitOfWork.Milestones.Get()),
                Steps = Mapper.Map<IEnumerable<Step>, IEnumerable<StepViewModel>>(
                        _unitOfWork.Steps.Get(s => s.MilestoneId == milestoneId).OrderBy(s => s.DisplayOrder)),
              
            };

            viewModel.SelectedMilestone = viewModel.Milestones.FirstOrDefault(m => m.MilestoneId == milestoneId);
            viewModel.SelectedStep = stepId == null ? viewModel.Steps.First() : viewModel.Steps.FirstOrDefault(s => s.StepId == stepId);
            return View(viewModel);
            
        }

        [HttpPost]
        public ActionResult Milestone(AdminViewModel viewModel)
        {
            var entityToUpdate = _unitOfWork.Steps.GetById(viewModel.SelectedStep.StepId);
            entityToUpdate.Guidance = viewModel.SelectedStep.Guidance;
            _unitOfWork.Steps.Update(entityToUpdate);
            _unitOfWork.Commit();
            return View(viewModel);
        }

        public ActionResult Users(string error)
        {
            ViewBag.Error = error;
            var users = _unitOfWork.Accounts.GetUsers();
            return View(users);
        }

        public ActionResult DeleteUser(Guid id)
        {
            string error;
            return _unitOfWork.Accounts.DeleteUser(id, out error ) ? RedirectToAction("Users") : RedirectToAction("Users", new { Error = error });
        }

        public ActionResult EditUser(Guid id)
        {
            var entityToEdit = _unitOfWork.Accounts.GetUsers().FirstOrDefault(u => u.UserId == id);
            var viewModel = Mapper.Map<UserModel, RegisterViewModel>(entityToEdit);
            ViewBag.Organizations = _unitOfWork.Organizations.Get();
            return View(viewModel);
        }

        [HttpPost]
        public ActionResult EditUser(RegisterViewModel viewModel)
        {
            // Dont care about password for now
            ModelState.Remove("Password");
            
            if (ModelState.IsValid)
            {
                string error;
                if (_unitOfWork.Accounts.UpdateUser(Mapper.Map<RegisterViewModel, UserModel>(viewModel), out error))
                {
                    return RedirectToAction("Users");
                }

                ModelState.AddModelError("", error);
            }

            // If we got this far, something failed, redisplay form
            return View(viewModel);
        }

        public JsonResult GetOrganizations(string term)
        {
            var organizations = _unitOfWork.Organizations.Get(o => o.Name.Contains(term)).Select(o => new {label = o.Name, value = o.OrganizationId});
            return Json(organizations, JsonRequestBehavior.AllowGet);
        }
    }
}
