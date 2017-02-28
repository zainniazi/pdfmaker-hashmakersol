using System.Web.Mvc;
using hashmakersol.pdfmaker;

namespace hashmakersol.Sample.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        public ActionResult Index()
        {
            return new PdfResult();
        }
    }
}