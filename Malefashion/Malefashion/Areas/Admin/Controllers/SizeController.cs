
using Malefashion.Areas.Admin.ViewModels;
using Malefashion.DAL;
using Malefashion.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Malefashion.Areas.Admin.Controllers
{
	[Area("Admin")]
	public class SizeController : Controller
	{
        private readonly AppDbContext _context;

        public SizeController(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index(int page)
        {
            double count = await _context.Sizes.CountAsync();

            List<Size> sizes = await _context.Sizes.Skip(page * 3).Take(3).Include(c => c.ProductSizes).ToListAsync();

            PaginationVM<Size> pagination = new()
            {
                TotalPage = Math.Ceiling(count / 3),

                CurrentPage = page,

                Items = sizes
            };
            return View(pagination);
        }
        public IActionResult Create()
        {


            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(CreateSizeVM sizeVM)
        {
            if (!ModelState.IsValid) return View(sizeVM);

            bool result = await _context.Sizes.AnyAsync(c => c.Name.Trim().ToLower() == sizeVM.Name.Trim().ToLower());

            if (result)
            {
                ModelState.AddModelError("Name", "This size already exists");
                return View(sizeVM);
            }

            Size size = new Size
            {

                Name = sizeVM.Name

            };
            await _context.Sizes.AddAsync(size);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Update(int id)
        {
            if (id <= 0) return BadRequest();


            Size size= await _context.Sizes.FirstOrDefaultAsync(c => c.Id == id);

            if (size == null) return NotFound();

            UpdateSizeVM sizeVM = new UpdateSizeVM
            {

                Name = size.Name
            };




            return View(sizeVM);
        }

        [HttpPost]

        public async Task<IActionResult> Update(int id, UpdateSizeVM sizeVM)
        {
            if (id <= 0) return BadRequest();

            if (!ModelState.IsValid) return View(sizeVM);

            Size existed = await _context.Sizes.FirstOrDefaultAsync(c => c.Id == id);

            if (existed == null) return NotFound();

            bool result = await _context.Sizes.AnyAsync(c => c.Name.Trim().ToLower() == sizeVM.Name.Trim().ToLower() && c.Id != id);

            if (result)
            {
                ModelState.AddModelError("Name", "There is already such size");
                return View(sizeVM);
            }

            existed.Name = sizeVM.Name;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }


        public async Task<IActionResult> Delete(int id, bool confirim)
        {
            if (id <= 0) return BadRequest();

            var existed = await _context.Sizes.FirstOrDefaultAsync(c => c.Id == id);

            if (existed is null) return NotFound();

            if (confirim)
            {

                _context.Sizes.Remove(existed);
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
