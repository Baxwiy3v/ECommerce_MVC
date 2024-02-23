using Malefashion.DAL;
using Malefashion.Models;
using Malefashion.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Malefashion.Controllers
{
	[Authorize]
	public class OrderController : Controller
	{
		private readonly AppDbContext _context;

		public OrderController(AppDbContext context)
        {
			_context = context;
		}
        public async Task<IActionResult> Index(int page = 1, int pageSize = 5)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var userOrders = await _context.Orders
                                            .Where(o => o.AppUserId == userId)
                                            .Skip((page - 1) * pageSize)
                                            .Take(pageSize)
                                            .ToListAsync();

            var totalOrdersCount = await _context.Orders
                                                  .Where(o => o.AppUserId == userId)
                                                  .CountAsync();

            var totalPages = Math.Ceiling((double)totalOrdersCount / pageSize);

            var paginationVM = new PaginationVM<Order>
            {
                TotalPage = totalPages,
                CurrentPage = page,
                Items = userOrders
            };

            return View(paginationVM);
        }


    }
}
