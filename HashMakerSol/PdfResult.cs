using System;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Web.Mvc;
using iTextSharp.text;

namespace hashmakersol.pdfmaker
{
    public class PdfResult : ActionResult
    {
        private const string ContentType = "application/pdf";
        private string ViewName { get; }
        private object Model { get; }
        private readonly string _fileDownloadName;
        private readonly Rectangle _pageSize;
        private readonly string _masterViewName;
        public PdfResult()
            : this(null, null, null, null, null)
        {
        }
        public PdfResult(string viewName, string masterViewName = null,
            string fileDownloadName = null,
            Rectangle docPageSize = null)
            : this(viewName, null, masterViewName, fileDownloadName, docPageSize)
        {
        }

        public PdfResult(object model, string masterViewName = null,
            string fileDownloadName = null,
            Rectangle docPageSize = null)
            : this(null, model, masterViewName, fileDownloadName, docPageSize)
        {
        }

        public PdfResult(string viewName, object model, string masterViewName = null,
            string fileDownloadName = null,
            Rectangle docPageSize = null)
        {
            ViewName = viewName;
            Model = model;
            _masterViewName = masterViewName;
            _fileDownloadName = fileDownloadName;
            _pageSize = docPageSize ?? PageSize.A4;
        }

        public override void ExecuteResult(ControllerContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            var response = context.HttpContext.Response;
            response.ContentType = ContentType;
            var cd = new ContentDisposition
            {
                Inline = false,
                FileName = _fileDownloadName,
                DispositionType = "Inline",
                CreationDate = DateTime.Now
            };
            response.AppendHeader("Content-Disposition", cd.ToString());

            var checkAction = string.IsNullOrWhiteSpace(ViewName) ?
                context.RouteData.Values["action"].ToString() : ViewName;
            var pHtml = RenderRazorViewToString(checkAction, Model, context);

            var pdf = new PdfBytes()
            {
                DocPageSize = _pageSize
            };
            var pdfBytes = pdf.GetPdfBytesArray(pHtml);
            response.OutputStream.Write(pdfBytes, 0, pdfBytes.Length);
        }

        private string RenderRazorViewToString(string viewName, object model, ControllerContext context)
        {
            context.Controller.ViewData.Model = model;
            context.Controller.ViewBag.PDF = true;
            using (var sw = new StringWriter())
            {
                var viewResult = string.IsNullOrEmpty(_masterViewName) ? 
                    ViewEngines.Engines.FindPartialView(context, viewName) :
                    ViewEngines.Engines.FindView(context, viewName, _masterViewName);
                if (viewResult == null)
                {
                    throw new Exception("View Not Found");
                }
                var viewContext = new ViewContext(context, viewResult.View,
                                             context.Controller.ViewData, context.Controller.TempData, sw);
                viewResult.View.Render(viewContext, sw);
                viewResult.ViewEngine.ReleaseView(context, viewResult.View);
                return sw.GetStringBuilder().ToString();
            }
        }
    }
}