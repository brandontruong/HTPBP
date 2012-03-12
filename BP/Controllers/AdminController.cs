using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using AutoMapper;
using BP.Domain.Abstract;
using BP.Domain.Entities;
using BP.Helpers;
using BP.Infrastructure;
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
    }
}
