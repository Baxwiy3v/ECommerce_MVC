using Malefashion.DAL;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Malefashion.Controllers
{
	public class OrderController : Controller
	{
		private readonly AppDbContext _context;

		public OrderController(AppDbContext context)
        {
			_context = context;
		}
		public IActionResult Index()
		{

			var userOrders = _context.Orders.Where(o => o.AppUserId == User.FindFirstValue(ClaimTypes.NameIdentifier)).ToList();

			return View(userOrders);
		}
	}
}
