using hashmakersol.pdfmaker;
using System.Web.Mvc;

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