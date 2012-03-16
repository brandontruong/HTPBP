using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web.Mvc;
using System.Web.Security;
using AutoMapper;
using BP.Domain.Abstract;
using BP.Domain.Entities;
using BP.Helpers;
using BP.Infrastructure;
using BP.Infrastructure.iMvcPdf;
using BP.ViewModels;
using BP.ViewModels.Admin;
using BP.ViewModels.BikePlan;
using System.Text;
using System.IO;
using System.Web;
using iTextSharp.text;
using iTextSharp.text.html.simpleparser;
using iTextSharp.text.pdf;
using iTextSharp.text.html;
using System.Xml;
using iTextSharp.text.xml;

namespace BP.Controllers
{
    [Authorize]
    public class BikePlanController : Controller
    {
        //
        // GET: /Admin/
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEmailService _emailService;

        public BikePlanController(IUnitOfWork unitOfWork, IEmailService emailService)
        {
            _unitOfWork = unitOfWork;
            _emailService = emailService;
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

        [HttpPost]
        public ActionResult Milestone(string milestoneOrder, string stepOrder, BikePlanViewModel viewModel)
        {
            CustomProfile customProfile = null; 
            foreach (var taskOutcome in viewModel.Outcomes)
            {
                var entityToUpdate = _unitOfWork.TaskOutcomes.GetById(taskOutcome.TaskOutcomeId);
                if (entityToUpdate == null)
                {
                    entityToUpdate = new TaskOutcome();
                    if (customProfile == null) customProfile = CustomProfile.GetProfile(Profile.UserName);
                    entityToUpdate.BikePlanApplicationId = customProfile.BikePlanApplicationId;
                    entityToUpdate.Description = taskOutcome.Description;
                    entityToUpdate.TaskId = taskOutcome.TaskId;
                    entityToUpdate.Description = taskOutcome.Description;
                    entityToUpdate.TaskOutcomeId = Guid.NewGuid();
                    _unitOfWork.TaskOutcomes.Insert(entityToUpdate);
                }
                else
                {
                    entityToUpdate.Description = taskOutcome.Description;
                    _unitOfWork.TaskOutcomes.Update(entityToUpdate);
                }
            }
            _unitOfWork.Commit();
            return RedirectToAction("Milestone", new { milestoneOrder, stepOrder});
        }

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
                                    MilestoneOrder = milestoneOrder,
                                    StepOrder = stepOrder,

                                 };
            viewModel.SelectedMilestone = viewModel.Milestones.ElementAt(iMilestoneOrder);
            
            viewModel.Steps = Mapper.Map<IEnumerable<Step>, IEnumerable<StepViewModel>>(
                _unitOfWork.Steps.Get(s => s.MilestoneId == viewModel.SelectedMilestone.MilestoneId).OrderBy(
                    s => s.DisplayOrder));

            viewModel.SelectedStep = viewModel.Steps.ElementAt(iStepOrder);
            viewModel.Tasks = Mapper.Map<IEnumerable<Task>, IEnumerable<TaskViewModel>>(
                _unitOfWork.Tasks.Get(t => t.StepId == viewModel.SelectedStep.StepId).OrderBy(t => t.DisplayOrder));

            var profile = CustomProfile.GetProfile(Profile.UserName);
            viewModel.Outcomes = Mapper.Map<IList<TaskOutcome>, IList<TaskOutcomeViewModel>>(_unitOfWork.TaskOutcomes.Get(t => t.BikePlanApplicationId == profile.BikePlanApplicationId).ToList());
            
            if (!viewModel.Outcomes.Any())
            {
                viewModel.Outcomes = new List<TaskOutcomeViewModel>();
                for (var i = 0; i < viewModel.Tasks.Count(); i++)
                {
                    viewModel.Outcomes.Add(new TaskOutcomeViewModel { TaskId = viewModel.Tasks.ElementAt(i).TaskId});
                }
                
            }
            return View(viewModel);

        }

        public ActionResult PDFView(string milestoneOrder, string stepOrder, BikePlanViewModel viewModel)
        {
            //using (MemoryStream ms = new MemoryStream())
            //{
            //    Document document = new Document(PageSize.A4, 25, 25, 30, 30);
            //    PdfWriter writer = PdfWriter.GetInstance(document, ms);
            //    document.AddAuthor("Brandon Truong");
            //    document.AddCreator("Sample application using iTextSharp");
            //    document.AddKeywords("PDF tutorial education");
            //    document.AddSubject("Document Subject - Describing the steps creating a PDF document");
            //    document.AddTitle("The document title - PDF creation using iTextSharp");

            //    document.Open();
            //    document.Add(new Paragraph("hello world !"));
            //    document.Close();
            //    writer.Close();
            //    Response.ContentType = "pdf/application";
            //    Response.AddHeader("content-disposition", "attachment;filename=First PDF Document.pdf");
            //    Response.OutputStream.Write(ms.GetBuffer(), 0, ms.GetBuffer().Length);
            //}
            //return null;

            int iMilestoneOrder;
            int iStepOrder;
            int.TryParse(milestoneOrder, out iMilestoneOrder);
            int.TryParse(stepOrder, out iStepOrder);
            if (iMilestoneOrder > 0) iMilestoneOrder--;
            if (iStepOrder > 0) iStepOrder--;

            viewModel.Milestones = Mapper.Map<IEnumerable<Milestone>, IEnumerable<MilestoneViewModel>>(
                _unitOfWork.Milestones.Get().OrderBy(m => m.DisplayOrder));

            viewModel.SelectedMilestone = viewModel.Milestones.ElementAt(iMilestoneOrder);

            viewModel.Steps = Mapper.Map<IEnumerable<Step>, IEnumerable<StepViewModel>>(
                _unitOfWork.Steps.Get(s => s.MilestoneId == viewModel.SelectedMilestone.MilestoneId).OrderBy(
                    s => s.DisplayOrder));

            viewModel.SelectedStep = viewModel.Steps.ElementAt(iStepOrder);

            viewModel.Tasks = Mapper.Map<IEnumerable<Task>, IEnumerable<TaskViewModel>>(
                _unitOfWork.Tasks.Get(t => t.StepId == viewModel.SelectedStep.StepId).OrderBy(t => t.DisplayOrder));

            var profile = CustomProfile.GetProfile(Profile.UserName);
            viewModel.Outcomes = Mapper.Map<IList<TaskOutcome>, IList<TaskOutcomeViewModel>>(_unitOfWork.TaskOutcomes.Get(t => t.BikePlanApplicationId == profile.BikePlanApplicationId).ToList());

            return this.PdfFromHtml(viewModel);
            //return ViewPdf(viewModel);
        }

        //protected string RenderActionResultToString(ActionResult result)
        //{
        //    // Create memory writer.
        //    var sb = new StringBuilder();
        //    var memWriter = new StringWriter(sb);

        //    // Create fake http context to render the view.
        //    var fakeResponse = new HttpResponse(memWriter);
        //    var fakeContext = new HttpContext(System.Web.HttpContext.Current.Request, fakeResponse);
        //    var fakeControllerContext = new ControllerContext(
        //        new HttpContextWrapper(fakeContext),
        //        this.ControllerContext.RouteData,
        //        this.ControllerContext.Controller);
        //    var oldContext = System.Web.HttpContext.Current;
        //    System.Web.HttpContext.Current = fakeContext;

        //    // Render the view.
        //    result.ExecuteResult(fakeControllerContext);

        //    // Restore data.
        //    System.Web.HttpContext.Current = oldContext;

        //    // Flush memory and return output.
        //    memWriter.Flush();
        //    return sb.ToString();
        //}

        //protected ActionResult ViewPdf(object model)
        //{
        //    // Create the iTextSharp document.
        //    Document doc = new Document();
        //    // Set the document to write to memory.
        //    MemoryStream memStream = new MemoryStream();
        //    PdfWriter writer = PdfWriter.GetInstance(doc, memStream);
        //    writer.CloseStream = false;
        //    doc.Open();

        //    // Render the view xml to a string, then parse that string into an XML dom.
        //    string xmltext = this.RenderActionResultToString(this.View(model));
        //    XmlDocument xmldoc = new XmlDocument();
        //    xmldoc.InnerXml = xmltext.Trim();

        //    // Parse the XML into the iTextSharp document.
        //    ITextHandler textHandler = new ITextHandler(doc);
        //    textHandler.Parse(xmldoc);

        //    HTMLWorker.ParseToList(new StringReader(xmltext),null);
        //    // Close and get the resulted binary data.
        //    doc.Close();
        //    byte[] buf = new byte[memStream.Position];
        //    memStream.Position = 0;
        //    memStream.Read(buf, 0, buf.Length);

        //    // Send the binary data to the browser.
        //    return new BinaryContentResult(buf, "application/pdf");
        //}

        [Authorize(Roles = "team leader")]
        public ActionResult Team()
        {
            var profile = CustomProfile.GetProfile(Profile.UserName);
            var users = _unitOfWork.Accounts.GetUsers().Where(u => u.OrganizationId == profile.OrganizationId);

            return View(users);
        }

        [Authorize(Roles = "team leader")]
        public ActionResult CreateMember()
        {
            ViewBag.Milestones = _unitOfWork.Milestones.Get().OrderBy(m => m.DisplayOrder); //.Select(m => new CheckBoxListInfo(m.MilestoneId.ToString(), m.Title, false)).ToList();
            ViewBag.Steps = _unitOfWork.Steps.Get();
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "team leader")]
        public ActionResult CreateMember(string[] milestone, string[] step, RegisterViewModel viewModel)
        {
             if (ModelState.IsValid)
            {
               // Attempt to register the user
                string error;
                var profile = CustomProfile.GetProfile(Profile.UserName);
                viewModel.OrganizationName = profile.Organization;
                viewModel.OrganizationId = profile.OrganizationId;
                var createStatus = _unitOfWork.Accounts.CreateUser(Mapper.Map<RegisterViewModel, UserModel>(viewModel), out error);

                if (createStatus == MembershipCreateStatus.Success)
                {
                    var user = _unitOfWork.Accounts.GetUserProfileByUserName(viewModel.Email);
                    // Add user permission
                    foreach (var s in step)
                    {
                        _unitOfWork.UserPermissions.Insert(new UserPermission{StepId = Guid.Parse(s), UserId = user.UserId, UserPermissionId = Guid.NewGuid()});
                    }
                    
                    _unitOfWork.Commit();

                    //_emailService.SendEmail(viewModel.Email, "Registration Confirmation",
                    //                        "Hi " + viewModel.FamilyName +
                    //                        ", you are now member of the HTPBP online application.", out error);
        
                    return RedirectToAction("Team");
                }

                ModelState.AddModelError("", String.IsNullOrEmpty(error)? Helper.ErrorCodeToString(createStatus): error);
            }

            // If we got this far, something failed, redisplay form
            return View(viewModel);

        }

        [Authorize(Roles = "team leader")]
        public ActionResult EditMember(Guid id)
        {
            ViewBag.Milestones = _unitOfWork.Milestones.Get().OrderBy(m => m.DisplayOrder);
            ViewBag.Steps = _unitOfWork.Steps.Get();
            ViewBag.UserPermissions = _unitOfWork.UserPermissions.Get(u => u.UserId == id);

            var entityToEdit = _unitOfWork.Accounts.GetUsers().FirstOrDefault(u => u.UserId == id);
            var viewModel = Mapper.Map<UserModel, RegisterViewModel>(entityToEdit);
            ViewBag.Organizations = _unitOfWork.Organizations.Get();
            return View(viewModel);
        }

        [HttpPost]
        [Authorize(Roles = "team leader")]
        public ActionResult EditMember(string[] milestone, string[] step, RegisterViewModel viewModel)
        {
            // Dont care about password for now
            ModelState.Remove("Password");

            if (ModelState.IsValid)
            {
                string error;
                
                if (_unitOfWork.Accounts.UpdateUser(Mapper.Map<RegisterViewModel, UserModel>(viewModel), out error))
                {
                    var grantedSteps = _unitOfWork.UserPermissions.Get(u => u.UserId == viewModel.UserId);
                    if (grantedSteps != null)
                    {
                        foreach (var userPermission in grantedSteps)
                        {
                            if (!step.Contains(userPermission.StepId.ToString()))
                                _unitOfWork.UserPermissions.Delete(userPermission);
                        }
                        _unitOfWork.Commit();

                        grantedSteps = _unitOfWork.UserPermissions.Get(u => u.UserId == viewModel.UserId);
                        // Add user permission
                        foreach (var s in step.Where(s => !grantedSteps.Any(g => g.StepId.ToString() == s)))
                        {
                            _unitOfWork.UserPermissions.Insert(new UserPermission{UserId = viewModel.UserId, StepId = Guid.Parse(s), UserPermissionId = Guid.NewGuid()});
                        }

                        _unitOfWork.Commit();
                    }

                    return RedirectToAction("Team");
                }

                ModelState.AddModelError("", error);
            }

            // If we got this far, something failed, redisplay form
            return View(viewModel);
        }

        [Authorize(Roles = "team leader")]
        public ActionResult DeactivateMember(Guid id)
        {
            string error;
            if (!_unitOfWork.Accounts.ActivateUser(id, false, out error)) ModelState.AddModelError("", error);
            return RedirectToAction("Team");
            
        }

        [Authorize(Roles = "team leader")]
        public ActionResult ReactivateMember(Guid id)
        {
            string error;
            if (!_unitOfWork.Accounts.ActivateUser(id, true, out error)) ModelState.AddModelError("", error);
            return RedirectToAction("Team");
        }
    }
}
