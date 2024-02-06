using Malefashion.Areas.Admin.ViewModels;
using Malefashion.DAL;
using Malefashion.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Malefashion.Areas.Admin.Controllers;

[Area("Admin")]
public class CategoryController : Controller
{
	private readonly AppDbContext _context;

	public CategoryController(AppDbContext context)
	{
		_context = context;
	}
	public async Task<IActionResult> Index(int page)
	{
		double count = await _context.Categories.CountAsync();

		List<Category> categories = await _context.Categories.Skip(page * 3).Take(3).Include(p => p.Products).ToListAsync();

		PaginationVM<Category> pagination = new()
		{
			TotalPage = Math.Ceiling(count / 3),

			CurrentPage = page,

			Items = categories
		};
		return View(pagination);
	}
	public IActionResult Create()
	{
		return View();
	}
	[HttpPost]
	public async Task<IActionResult> Create(CreateCategoryVM categoryVM)
	{
		if (!ModelState.IsValid) return View(categoryVM);

		bool result = await _context.Categories.AnyAsync(c => c.Name.Trim().ToLower() == categoryVM.Name.Trim().ToLower());

		if (result)
		{
			ModelState.AddModelError("Name", "This name exists");
			return View(categoryVM);
		}

		Category category = new Category
		{

			Name = categoryVM.Name

		};
		await _context.Categories.AddAsync(category);
		await _context.SaveChangesAsync();

		return RedirectToAction(nameof(Index));
	}
	public async Task<IActionResult> Update(int id)
	{
		if (id <= 0) return BadRequest();


		Category? category = await _context.Categories.FirstOrDefaultAsync(c => c.Id == id);

		if (category == null) return NotFound();

		UpdateCategoryVM categoryVM = new UpdateCategoryVM
		{

			Name = category.Name
		};

		return View(categoryVM);
	}

	[HttpPost]

	public async Task<IActionResult> Update(int id, UpdateCategoryVM categoryVM)
	{
		if (id <= 0) return BadRequest();

		if (!ModelState.IsValid) return View(categoryVM);

		Category? existed = await _context.Categories.FirstOrDefaultAsync(c => c.Id == id);

		if (existed == null) return NotFound();

		bool result = await _context.Categories.AnyAsync(c => c.Name.Trim().ToLower() == categoryVM.Name.Trim().ToLower() && c.Id != id);

		if (result)
		{
			ModelState.AddModelError("Name", "This name exists");
			return View(categoryVM);
		}
		existed.Name = categoryVM.Name;

		await _context.SaveChangesAsync();
		return RedirectToAction(nameof(Index));
	}


	public async Task<IActionResult> Delete(int id, bool confirim)
	{
		if (id <= 0) return BadRequest();

		var existed = await _context.Categories.FirstOrDefaultAsync(c => c.Id == id);

		if (existed is null) return NotFound();

		if (confirim)
		{

			_context.Categories.Remove(existed);
			await _context.SaveChangesAsync();
			return RedirectToAction(nameof(Index));
		}
		else
		{

			return View(existed);
		}
	}
}
