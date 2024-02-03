using Malefashion.Areas.Admin.ViewModels;
using Malefashion.DAL;
using Malefashion.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Malefashion.Areas.Admin.Controllers
{
	[Area("Admin")]
	public class ColorController : Controller
	{
        private readonly AppDbContext _context;

        public ColorController(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            List<Color> colors = await _context.Colors.Include(c => c.ProductColors).ToListAsync();
            return View(colors);
        }
        public IActionResult Create()
        {


            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(CreateColorVM colorVM)
        {
            if (!ModelState.IsValid) return View(colorVM);

            bool result = await _context.Colors.AnyAsync(c => c.Name.Trim().ToLower() == colorVM.Name.Trim().ToLower());

            if (result)
            {
                ModelState.AddModelError("Name", "This color already exists");
                return View(colorVM);
            }

            Color color = new Color
            {

                Name = colorVM.Name

            };
            await _context.Colors.AddAsync(color);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Update(int id)
        {
            if (id <= 0) return BadRequest();


            Color color = await _context.Colors.FirstOrDefaultAsync(c => c.Id == id);

            if (color == null) return NotFound();

            UpdateColorVM colorVM = new UpdateColorVM
            {

                Name = color.Name
            };




            return View(colorVM);
        }

        [HttpPost]

        public async Task<IActionResult> Update(int id, UpdateColorVM colorVM)
        {
            if (id <= 0) return BadRequest();

            if (!ModelState.IsValid) return View(colorVM);

            Color existed = await _context.Colors.FirstOrDefaultAsync(c => c.Id == id);

            if (existed == null) return NotFound();

            bool result = await _context.Colors.AnyAsync(c => c.Name.Trim().ToLower() == colorVM.Name.Trim().ToLower() && c.Id != id);

            if (result)
            {
                ModelState.AddModelError("Name", "There is already such color");
                return View(colorVM);
            }

            existed.Name = colorVM.Name;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }


        public async Task<IActionResult> Delete(int id, bool confirim)
        {
            if (id <= 0) return BadRequest();

            var existed = await _context.Colors.FirstOrDefaultAsync(c => c.Id == id);

            if (existed is null) return NotFound();

            if (confirim)
            {

                _context.Colors.Remove(existed);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            else
            {

                return View(existed);
            }
        }

    }
}
