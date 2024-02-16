using Malefashion.Areas.Admin.ViewModels;
using Malefashion.DAL;
using Malefashion.Models;
using Malefashion.Utilities.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Malefashion.Areas.Admin.Controllers;

[Area("Admin")]
public class ColorController : Controller
{
	private readonly AppDbContext _context;

	public ColorController(AppDbContext context)
	{
		_context = context;
	}
    [Authorize(Roles = "Admin,Moderator")]
    public async Task<IActionResult> Index(int page)
	{
		double count = await _context.Colors.CountAsync();

		List<Color> colors = await _context.Colors.Skip(page * 3).Take(3).Include(c => c.ProductColors).ToListAsync();

		PaginationVM<Color> pagination = new()
		{
			TotalPage = Math.Ceiling(count / 3),

			CurrentPage = page,

			Items = colors
		};

		return View(pagination);
	}
    [Authorize(Roles = "Admin,Moderator")]
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
    [Authorize(Roles = "Admin,Moderator")]
    public async Task<IActionResult> Update(int id)
	{
		if (id <= 0) throw new WrongRequestException("Your request is wrong");


		Color? color = await _context.Colors.FirstOrDefaultAsync(c => c.Id == id);

		if (color == null) throw new NotFoundException("There is no such Color");

		UpdateColorVM colorVM = new UpdateColorVM
		{

			Name = color.Name
		};




		return View(colorVM);
	}

	[HttpPost]

	public async Task<IActionResult> Update(int id, UpdateColorVM colorVM)
	{
		if (id <= 0) throw new WrongRequestException("Your request is wrong");

		if (!ModelState.IsValid) return View(colorVM);

		Color? existed = await _context.Colors.FirstOrDefaultAsync(c => c.Id == id);

		if (existed == null) throw new NotFoundException("There is no such Color");

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

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id, bool confirim)
	{
		if (id <= 0) throw new WrongRequestException("Your request is wrong");

		var existed = await _context.Colors.FirstOrDefaultAsync(c => c.Id == id);

		if (existed is null) throw new NotFoundException("There is no such Color");

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
