using Microsoft.AspNetCore.Mvc;

namespace Malefashion.Areas.Admin.Controllers
{
	[Area("Admin")]
	public class TagController : Controller
	{
		public IActionResult Index()
		{
			return View();
		}
	}
}
