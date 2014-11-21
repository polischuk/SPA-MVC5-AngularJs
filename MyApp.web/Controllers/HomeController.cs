using System.Web.Mvc;
using MyApp.data;
using MyApp.data.Repositories;

namespace MyApp.web.Controllers
{
    public class HomeController : Controller
    {
        private ApplicationDbContext _context;
        public HomeController(ApplicationDbContext context,ApplicationUserRepository rep)
        {
            _context = context;
        }
        public ActionResult Index()
        {
           
          
            return View();
        }

        
    }
}