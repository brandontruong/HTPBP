﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web.Mvc;
using AutoMapper;
using BP.Domain.Abstract;
using BP.Domain.Entities;
using BP.Helpers;
using BP.Infrastructure;
using BP.Infrastructure.iMvcPdf;
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
    }
}
