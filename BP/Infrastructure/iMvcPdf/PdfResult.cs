using System;
using System.IO;
using System.Web.Mvc;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace BP.Infrastructure.iMvcPdf
{
    public class PdfResult : ViewResult
    {

        public override void ExecuteResult(ControllerContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }
            if (string.IsNullOrEmpty(this.ViewName))
            {
                this.ViewName = context.RouteData.GetRequiredString("action");
            }

            if (this.View == null)
            {
                this.View = this.FindView(context).View;
            }

            // Convert to pdf

            var response = context.HttpContext.Response;

            using (var pdfDoc = new Document())
            using (var pdfWriter = PdfWriter.GetInstance(pdfDoc, response.OutputStream))
            {
                pdfDoc.Open();

                this.ViewBag.Document = pdfDoc;
                this.ViewData["Document"] = pdfDoc;

                // First get the html from the Html view
                using (var throwAway = new StringWriter())
                {
                    var vwContext = new ViewContext(context, this.View, this.ViewData, this.TempData, throwAway);
                    this.View.Render(vwContext, throwAway);
                }

                pdfDoc.Close();
            }

            response.ContentType = "application/pdf";
            response.AddHeader("Content-Disposition", this.ViewName + ".pdf");

        }

    }
}