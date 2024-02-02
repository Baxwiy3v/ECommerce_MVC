using Malefashion.DAL;
using Malefashion.Models.ViewModels;
using Malefashion.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Malefashion.Controllers
{
	public class HomeController : Controller
	{
		private readonly AppDbContext _context;

		public HomeController(AppDbContext context)
        {
			_context = context;
		}
		public async Task<IActionResult> Index()
		{
			List<Slide> slides = await _context.Slides.OrderBy(s => s.Order).ToListAsync();
			
			HomeVM vm = new()
			{
				Slides = slides
				
			};
			return View(vm);
		}
	}
}
