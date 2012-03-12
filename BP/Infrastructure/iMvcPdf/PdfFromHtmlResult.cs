using System;
using System.IO;
using System.Web.Mvc;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace BP.Infrastructure.iMvcPdf
{
    public class PdfFromHtmlResult : ViewResult
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

            // First get the html from the Html view
            using (var writer = new StringWriter())
            {
                var vwContext = new ViewContext(context, this.View, this.ViewData, this.TempData, writer);
                this.View.Render(vwContext, writer);

                // Convert to pdf

                var response = context.HttpContext.Response;

                using (var pdfStream = new MemoryStream())
                using (var pdfDoc = new Document())
                using (var pdfWriter = PdfWriter.GetInstance(pdfDoc, pdfStream))
                {

                    pdfDoc.Open();

                    using (var htmlRdr = new StringReader(writer.ToString()))
                    {

                        var parsed = iTextSharp.text.html.simpleparser.HTMLWorker.ParseToList(htmlRdr, null);

                        foreach (var parsedElement in parsed)
                        {
                            pdfDoc.Add(parsedElement);
                        }
                    }

                    pdfDoc.Close();

                    response.ContentType = "application/pdf";
                    response.AddHeader("Content-Disposition", this.ViewName + ".pdf");
                    byte[] pdfBytes = pdfStream.ToArray();
                    response.OutputStream.Write(pdfBytes, 0, pdfBytes.Length);
                }
            }
        }
    }
}