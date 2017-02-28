using System.Web.Mvc;
using hashmakersol.pdfmaker;
using hashmakersol.Sample.Models;
using iTextSharp.text;

namespace hashmakersol.Sample.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        public ActionResult Index()
        {
            return new PdfResult();
        }

        public ActionResult ViewName()
        {
            return new PdfResult("Index", docPageSize: PageSize.A4);
        }

        public ActionResult Model()
        {
            var model = new TestModel
            {
                Name = "John Doe"
            };
            return new PdfResult(model, "PDFLayout", docPageSize: PageSize.A4);
        }
    }
}