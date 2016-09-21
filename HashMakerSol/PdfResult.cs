using iTextSharp.text;
using System;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Web.Mvc;

namespace HashMakerSol.PDFMaker
{
    public class PdfResult : ActionResult
    {
        // Properties
        private string Action { get; set; }

        private object Model { get; set; }

        private string _contentType;

        public string ContentType
        {
            get { return (_contentType ?? "application/pdf"); }
            set { _contentType = value; }
        }

        private string _fileDownloadName;

        public string FileDownloadName
        {
            get { return (_fileDownloadName ?? string.Empty); }
            set { _fileDownloadName = value; }
        }

        private Rectangle _pageSize;

        public Rectangle DocPageSize
        {
            get { return (_pageSize ?? PageSize.A4); }
            set { _pageSize = value; }
        }

        private string _masterViewName;

        public string MasterViewName
        {
            get { return (_masterViewName ?? string.Empty); }
            set { _masterViewName = value; }
        }

        public PdfResult()
        { }

        public PdfResult(string Action)
        {
            ContentType = "application/pdf";
            this.Action = Action;
        }

        public PdfResult(object Object)
        {
            ContentType = "application/pdf";
            Model = Object;
        }

        public PdfResult(string Action, object Object)
        {
            ContentType = "application/pdf";
            this.Action = Action;
            Model = Object;
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
                FileName = FileDownloadName,
                DispositionType = "Inline",
                CreationDate = DateTime.Now
            };
            response.AppendHeader("Content-Disposition", cd.ToString());

            string CheckAction = string.IsNullOrWhiteSpace(Action) ? context.RouteData.Values["action"].ToString() : Action;
            string pHtml = RenderRazorViewToString(CheckAction, Model, context);

            PdfBytes Pdf = new PdfBytes()
            {
                DocPageSize = DocPageSize
            };
            var PdfBytes = Pdf.GetPdfBytesArray(pHtml);
            response.OutputStream.Write(PdfBytes, 0, PdfBytes.Count());
        }

        private string RenderRazorViewToString(string viewName, object model, ControllerContext Context)
        {
            Context.Controller.ViewData.Model = model;
            Context.Controller.ViewBag.PDF = true;
            using (var sw = new StringWriter())
            {
                ViewEngineResult viewResult = null;
                if (string.IsNullOrEmpty(MasterViewName))
                    viewResult = ViewEngines.Engines.FindPartialView(Context, viewName);
                else
                    viewResult = ViewEngines.Engines.FindView(Context, viewName, MasterViewName);

                if (viewResult == null)
                {
                    throw new Exception("View Not Found");
                }

                var viewContext = new ViewContext(Context, viewResult.View,
                                             Context.Controller.ViewData, Context.Controller.TempData, sw);
                viewResult.View.Render(viewContext, sw);
                viewResult.ViewEngine.ReleaseView(Context, viewResult.View);
                return sw.GetStringBuilder().ToString();
            }
        }
    }
}