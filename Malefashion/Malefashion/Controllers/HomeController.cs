using Microsoft.AspNetCore.Mvc;

namespace Malefashion.Controllers
{
	public class HomeController : Controller
	{
		public IActionResult Index()
		{
			return View();
		}
	}
}
