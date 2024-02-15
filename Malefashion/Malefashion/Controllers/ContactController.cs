using Microsoft.AspNetCore.Mvc;
using System.Net.Mail;
using System.Net;

namespace Malefashion.Controllers
{
    public class ContactController : Controller
    {

        public IActionResult Index()
        {
            return View();
        }

    }
}
