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
using BP.ViewModels.BikePlan;

namespace BP.Controllers
{
    [Authorize(Roles = "team leader")]
    public class BikePlanController : Controller
    {
        //
        // GET: /Admin/
        private readonly IUnitOfWork _unitOfWork;

        public BikePlanController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public ActionResult Index()
        {
           
           
            return RedirectToAction("Milestone", new {milestoneOrder = "1", stepOrder="1"});

            
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

        //public ActionResult Milestone(Guid milestoneId, Guid? stepId)
        //{
        //    var viewModel = new AdminViewModel
        //    {
        //        Milestones =
        //            Mapper.Map<IEnumerable<Milestone>, IEnumerable<MilestoneViewModel>>(
        //                _unitOfWork.Milestones.Get()),
        //        Steps = Mapper.Map<IEnumerable<Step>, IEnumerable<StepViewModel>>(
        //                _unitOfWork.Steps.Get(s => s.MilestoneId == milestoneId).OrderBy(s => s.DisplayOrder)),
              
        //    };

        //    viewModel.SelectedMilestone = viewModel.Milestones.FirstOrDefault(m => m.MilestoneId == milestoneId);
        //    viewModel.SelectedStep = stepId == null ? viewModel.Steps.First() : viewModel.Steps.FirstOrDefault(s => s.StepId == stepId);
        //    return View(viewModel);
            
        //}

        public ActionResult Milestone(string milestoneOrder, string stepOrder)
        {
            int iMilestoneOrder;
            int iStepOrder;
            int.TryParse(milestoneOrder, out iMilestoneOrder);
            int.TryParse(stepOrder, out iStepOrder);
            
            if (iMilestoneOrder > 0) iMilestoneOrder--;
            if (iStepOrder > 0) iStepOrder--;

            var viewModel = new BikePlanViewModel
                                {
                                    Milestones =
                                        Mapper.Map<IEnumerable<Milestone>, IEnumerable<MilestoneViewModel>>(
                                            _unitOfWork.Milestones.Get().OrderBy(m => m.DisplayOrder)),

                                 };
            viewModel.SelectedMilestone = viewModel.Milestones.ElementAt(iMilestoneOrder);
            
            viewModel.Steps = Mapper.Map<IEnumerable<Step>, IEnumerable<StepViewModel>>(
                _unitOfWork.Steps.Get(s => s.MilestoneId == viewModel.SelectedMilestone.MilestoneId).OrderBy(
                    s => s.DisplayOrder));

            viewModel.SelectedStep = viewModel.Steps.ElementAt(iStepOrder);
            viewModel.Tasks = Mapper.Map<IEnumerable<Task>, IEnumerable<TaskViewModel>>(
                _unitOfWork.Tasks.Get(t => t.StepId == viewModel.SelectedStep.StepId).OrderBy(t => t.DisplayOrder));

            var profile = CustomProfile.GetProfile(Profile.UserName);
            viewModel.Outcomes = Mapper.Map<IEnumerable<TaskOutcome>, IEnumerable<TaskOutcomeViewModel>>(_unitOfWork.TaskOutcomes.Get(t => t.BikePlanApplicationId == profile.BikePlanApplicationId));
            
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
    }
}
