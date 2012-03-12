using System.Web.Mvc;

namespace BP.Infrastructure.iMvcPdf
{
    public static class ControllerExtensions
    {
        public static PdfFromHtmlResult PdfFromHtml(this Controller controller)
        {
            return PdfFromHtml(controller, null, null);
        }

        public static PdfFromHtmlResult PdfFromHtml(this Controller controller, object model)
        {
            return PdfFromHtml(controller, null, model);
        }

        public static PdfFromHtmlResult PdfFromHtml(this Controller controller, string viewName)
        {
            return PdfFromHtml(controller, viewName, null);
        }

        public static PdfFromHtmlResult PdfFromHtml(this Controller controller, string viewName, object model)
        {
            if (model != null)
            {
                controller.ViewData.Model = model;
            }

            var result = new PdfFromHtmlResult()
            {
                ViewName = viewName,
                ViewData = controller.ViewData,
                TempData = controller.TempData,
            };
            return result;
        }

        public static PdfResult Pdf(this Controller controller)
        {
            return Pdf(controller, null, null);
        }
        public static PdfResult Pdf(this Controller controller, string viewName, object model)
        {
            if (model != null)
            {
                controller.ViewData.Model = model;
            }

            var result = new PdfResult()
            {
                ViewName = viewName,
                ViewData = controller.ViewData,
                TempData = controller.TempData,
            };
            return result;
        }

    }
}