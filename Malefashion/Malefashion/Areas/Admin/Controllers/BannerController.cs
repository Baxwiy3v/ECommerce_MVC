using Malefashion.Areas.Admin.ViewModels;
using Malefashion.DAL;
using Malefashion.Models;
using Malefashion.Utilities.Exceptions;
using Malefashion.Utilities.Extentions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Malefashion.Areas.Admin.Controllers;

[Area("Admin")]
public class BannerController : Controller
{
	private readonly AppDbContext _context;
	private readonly IWebHostEnvironment _env;
	public BannerController(AppDbContext context, IWebHostEnvironment env)
	{
		_context = context;

		_env = env;
	}
	[Authorize(Roles = "Admin,Moderator")]
	public async Task<IActionResult> Index(int page)
	{
		double count = await _context.Banners.CountAsync();

		List<Banner> Banners = await _context.Banners.Skip(page * 3).Take(3).ToListAsync();
		PaginationVM<Banner> pagination = new()
		{
			TotalPage = Math.Ceiling(count / 3),

			CurrentPage = page,

			Items = Banners
		};
		return View(pagination);
	}
	[Authorize(Roles = "Admin,Moderator")]
	public IActionResult Create()
	{
		return View();
	}
	[HttpPost]
	public async Task<IActionResult> Create(CreateBannerVM BannerVM)
	{
		if (!ModelState.IsValid) return View(BannerVM);

		bool result = await _context.Banners.AnyAsync(s => s.Name.Trim().ToLower() == BannerVM.Name.Trim().ToLower());

		if (result)
		{
			ModelState.AddModelError("Name", "This Name already exists");
			return View(BannerVM);

		}

		bool order = await _context.Banners.AnyAsync(s => s.Order == BannerVM.Order);

		if (order)
		{
			ModelState.AddModelError("Order", "This Order already exists");
			return View(BannerVM);

		}

		if (!BannerVM.Photo.ValidateType())
		{
			ModelState.AddModelError("Photo", "Wrong file type");
			return View(BannerVM);
		}
		if (BannerVM.Photo.ValidateSize(4 * 1024))
		{
			ModelState.AddModelError("Photo", "It shouldn't exceed 4 mb");
			return View(BannerVM);
		}
		string fileName = await BannerVM.Photo.CreateFile(_env.WebRootPath, "img", "banner");
		if (BannerVM.ButtonTitle is null) { BannerVM.ButtonTitle = "Shop Now"; }
		

		Banner Banner = new Banner
		{
			Name = BannerVM.Name,
			Order = BannerVM.Order,
			ButtonTitle = BannerVM.ButtonTitle,
			ImageUrl = fileName
	
		};
		await _context.Banners.AddAsync(Banner);
		await _context.SaveChangesAsync();
		return RedirectToAction(nameof(Index));
	}
	[Authorize(Roles = "Admin")]
	public async Task<IActionResult> Delete(int id, bool confirim)
	{
		if (id <= 0) throw new WrongRequestException("Your request is wrong");
		Banner? Banner = await _context.Banners.FirstOrDefaultAsync(s => s.Id == id);
		if (Banner == null) throw new NotFoundException("There is no such Banner");
		if (confirim)
		{

			Banner.ImageUrl.DeleteFile(_env.WebRootPath, "img", "banner");
			_context.Banners.Remove(Banner);
			await _context.SaveChangesAsync();
			return RedirectToAction(nameof(Index));
		}
		else
		{

			return View(Banner);
		}
	}
	[Authorize(Roles = "Admin,Moderator")]
	public async Task<IActionResult> Update(int id)
	{
		if (id <= 0) throw new WrongRequestException("Your request is wrong");
		Banner? Banner = await _context.Banners.FirstOrDefaultAsync(c => c.Id == id);
		if (Banner is null) throw new NotFoundException("There is no such Banner");

		UpdateBannerVM vm = new UpdateBannerVM
		{
			Name = Banner.Name,
			Order = Banner.Order,
			ButtonTitle = Banner.ButtonTitle,
			ImageUrl = Banner.ImageUrl
		};

		return View(vm);
	}

	[HttpPost]
	public async Task<IActionResult> Update(int id, UpdateBannerVM Bannervm)
	{
		if (id <= 0) throw new WrongRequestException("Your request is wrong");

		if (!ModelState.IsValid) return View(Bannervm);

		Banner? existed = await _context.Banners.FirstOrDefaultAsync(c => c.Id == id);

		if (existed is null) throw new NotFoundException("There is no such Banner");

		bool result = await _context.Banners.AnyAsync(c => c.Name.Trim().ToLower() == Bannervm.Name.Trim().ToLower() && c.Id != id);
		if (result)
		{
			ModelState.AddModelError("Name", "There is already such Name");

			return View(Bannervm);
		}
		bool order = await _context.Banners.AnyAsync(c => c.Order == Bannervm.Order && c.Id != id);
		if (order)
		{
			ModelState.AddModelError("Order", "There is already such Order");

			return View(Bannervm);
		}
		


		if (Bannervm.Photo is not null)
		{
			if (!Bannervm.Photo.ValidateType())
			{
				ModelState.AddModelError("Photo", "Wrong file type");
				return View(Bannervm);
			}
			if (Bannervm.Photo.ValidateSize(4 * 1024))
			{
				ModelState.AddModelError("Photo", "It shouldn't exceed 4 mb");
				return View(Bannervm);
			}
			string newImage = await Bannervm.Photo.CreateFile(_env.WebRootPath, "img", "banner");
			existed.ImageUrl.DeleteFile(_env.WebRootPath, "img", "banner");
			existed.ImageUrl = newImage;

		}

		existed.Name = Bannervm.Name;
		existed.Order = Bannervm.Order;
		existed.ButtonTitle = Bannervm.ButtonTitle;

		await _context.SaveChangesAsync();
		return RedirectToAction(nameof(Index));
	}

}
