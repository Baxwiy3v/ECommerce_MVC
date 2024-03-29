﻿using AutoMapper;
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
public class SlideController : Controller
{
	private readonly AppDbContext _context;
	private readonly IWebHostEnvironment _env;
	public SlideController(AppDbContext context, IWebHostEnvironment env)
	{
		_context = context;

		_env = env;
	}
    [Authorize(Roles = "Admin,Moderator")]
    public async Task<IActionResult> Index(int page)
    {
        double count = await _context.Slides.CountAsync();

        List<Slide> slides = await _context.Slides.Skip(page * 3).Take(3).ToListAsync();
        PaginationVM<Slide> pagination = new()
        {
            TotalPage = Math.Ceiling(count / 3),

            CurrentPage = page,

            Items = slides
        };
        return View(pagination);
	}
    [Authorize(Roles = "Admin,Moderator")]
    public IActionResult Create()
	{
		return View();
	}
	[HttpPost]
	public async Task<IActionResult> Create(CreateSlideVM slideVM)
	{
		if (!ModelState.IsValid) return View(slideVM);

		bool result = await _context.Slides.AnyAsync(s => s.Title.Trim().ToLower() == slideVM.Title.Trim().ToLower());

		if (result)
		{
			ModelState.AddModelError("Title", "This Title already exists");
			return View(slideVM);

		}

		bool order = await _context.Slides.AnyAsync(s => s.Order == slideVM.Order);

		if (order)
		{
			ModelState.AddModelError("Order", "This Order already exists");
			return View(slideVM);

		}

		if (!slideVM.Photo.ValidateType())
		{
			ModelState.AddModelError("Photo", "Wrong file type");
			return View(slideVM);
		}
		if (slideVM.Photo.ValidateSize(4 * 1024))
		{
			ModelState.AddModelError("Photo", "It shouldn't exceed 4 mb");
			return View(slideVM);
		}
		string fileName = await slideVM.Photo.CreateFile(_env.WebRootPath, "img", "hero");
		if (slideVM.ButtonTitle is null) { slideVM.ButtonTitle = "Shop Now"; }
		if (slideVM.FbLink is null) { slideVM.FbLink = "https://www.facebook.com/"; }
		if (slideVM.IgLink is null) { slideVM.IgLink = "https://www.instagram.com/"; }
		if (slideVM.TwLink is null) { slideVM.TwLink = "https://twitter.com/login"; }
		if (slideVM.PtLink is null) { slideVM.PtLink = "https://www.pinterest.com/"; }

		Slide slide = new Slide
		{
			Title = slideVM.Title,
			SubTitle = slideVM.SubTitle,
			Description = slideVM.Description,
			Order = slideVM.Order,
			ButtonTitle = slideVM.ButtonTitle,
			ImageUrl = fileName,
			PtLink = slideVM.PtLink,
			FbLink = slideVM.FbLink,
			IgLink = slideVM.IgLink,
			TwLink = slideVM.TwLink
		};
		await _context.Slides.AddAsync(slide);
		await _context.SaveChangesAsync();
		return RedirectToAction(nameof(Index));
	}
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id, bool confirim)
	{
		if (id <= 0) throw new WrongRequestException("Your request is wrong");
		Slide? slide = await _context.Slides.FirstOrDefaultAsync(s => s.Id == id);
		if (slide == null) throw new NotFoundException("There is no such Slide");
		if (confirim)
		{

			slide.ImageUrl.DeleteFile(_env.WebRootPath, "img", "hero");
			_context.Slides.Remove(slide);
			await _context.SaveChangesAsync();
			return RedirectToAction(nameof(Index));
		}
		else
		{

			return View(slide);
		}
	}
    [Authorize(Roles = "Admin,Moderator")]
    public async Task<IActionResult> Update(int id)
	{
		if (id <= 0) throw new WrongRequestException("Your request is wrong");
		Slide? slide = await _context.Slides.FirstOrDefaultAsync(c => c.Id == id);
		if (slide is null) throw new NotFoundException("There is no such Slide");

		UpdateSlideVM vm = new UpdateSlideVM
		{
			Title = slide.Title,
			SubTitle = slide.SubTitle,
			Description = slide.Description,
			Order = slide.Order,
			ButtonTitle = slide.ButtonTitle,
			ImageUrl = slide.ImageUrl
		};

		return View(vm);
	}

	[HttpPost]
	public async Task<IActionResult> Update(int id, UpdateSlideVM slidevm)
	{
		if (id <= 0) throw new WrongRequestException("Your request is wrong");

		if (!ModelState.IsValid) return View(slidevm);

		Slide? existed = await _context.Slides.FirstOrDefaultAsync(c => c.Id == id);

		if (existed is null) throw new NotFoundException("There is no such Slide");

		bool result = await _context.Slides.AnyAsync(c => c.Title.Trim().ToLower() == slidevm.Title.Trim().ToLower()  && c.Id != id);
		if (result)
		{
			ModelState.AddModelError("Title", "There is already such title");
			
			return View(slidevm);
		}
		bool order = await _context.Slides.AnyAsync(c => c.Order == slidevm.Order && c.Id!=id);
		if (order)
		{
			ModelState.AddModelError("Order", "There is already such Order");

			return View(slidevm);
		}


		if (slidevm.Photo is not null)
		{
			if (!slidevm.Photo.ValidateType())
			{
				ModelState.AddModelError("Photo", "Wrong file type");
				return View(slidevm);
			}
			if (slidevm.Photo.ValidateSize(4 * 1024))
			{
				ModelState.AddModelError("Photo", "It shouldn't exceed 4 mb");
				return View(slidevm);
			}
			string newImage = await slidevm.Photo.CreateFile(_env.WebRootPath, "img", "hero");
			existed.ImageUrl.DeleteFile(_env.WebRootPath, "img", "hero");
			existed.ImageUrl = newImage;

		}
       
        existed.Title = slidevm.Title;
		existed.Description = slidevm.Description;
		existed.SubTitle = slidevm.SubTitle;
		existed.Order = slidevm.Order;
		existed.ButtonTitle = slidevm.ButtonTitle;

		await _context.SaveChangesAsync();
		return RedirectToAction(nameof(Index));
	}
		
}


		
	

