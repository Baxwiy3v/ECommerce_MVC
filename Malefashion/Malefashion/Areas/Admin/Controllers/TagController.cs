﻿using Malefashion.Areas.Admin.ViewModels;
using Malefashion.DAL;
using Malefashion.Models;
using Malefashion.Utilities.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Malefashion.Areas.Admin.Controllers;

[Area("Admin")]
public class TagController : Controller
{
	private readonly AppDbContext _context;

	public TagController(AppDbContext context)
	{
		_context = context;
	}
    [Authorize(Roles = "Admin,Moderator")]
    public async Task<IActionResult> Index(int page)
	{
		double count = await _context.Tags.CountAsync();

		List<Tag> tags = await _context.Tags.Skip(page * 3).Take(3).Include(c => c.ProductTags).ToListAsync();

		PaginationVM<Tag> pagination = new()
		{
			TotalPage = Math.Ceiling(count / 3),

			CurrentPage = page,

			Items = tags
		};

		return View(pagination);
	}
    [Authorize(Roles = "Admin,Moderator")]
    public IActionResult Create()
	{


		return View();
	}
	[HttpPost]
	public async Task<IActionResult> Create(CreateTagVM tagVM)
	{
		if (!ModelState.IsValid) return View(tagVM);

		bool result = await _context.Tags.AnyAsync(c => c.Name.Trim().ToLower() == tagVM.Name.Trim().ToLower());

		if (result)
		{
			ModelState.AddModelError("Name", "This tag already exists");
			return View(tagVM);
		}

		Tag tag = new Tag
		{

			Name = tagVM.Name

		};
		await _context.Tags.AddAsync(tag);
		await _context.SaveChangesAsync();

		return RedirectToAction(nameof(Index));
	}

    [Authorize(Roles = "Admin,Moderator")]
    public async Task<IActionResult> Update(int id)
	{
		if (id <= 0) throw new WrongRequestException("Your request is wrong");


		Tag? tag = await _context.Tags.FirstOrDefaultAsync(c => c.Id == id);

		if (tag == null) throw new NotFoundException("There is no such Tag");

		UpdateTagVM tagVM = new UpdateTagVM
		{

			Name = tag.Name
		};




		return View(tagVM);
	}

	[HttpPost]

	public async Task<IActionResult> Update(int id, UpdateTagVM tagVM)
	{
		if (id <= 0) throw new WrongRequestException("Your request is wrong"); ;

		if (!ModelState.IsValid) return View(tagVM);

		Tag? existed = await _context.Tags.FirstOrDefaultAsync(c => c.Id == id);

		if (existed == null) throw new NotFoundException("There is no such Tag");

		bool result = await _context.Tags.AnyAsync(c => c.Name.Trim().ToLower() == tagVM.Name.Trim().ToLower() && c.Id != id);

		if (result)
		{
			ModelState.AddModelError("Name", "There is already such tag");
			return View(tagVM);
		}

		existed.Name = tagVM.Name;

		await _context.SaveChangesAsync();

		return RedirectToAction(nameof(Index));
	}
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id, bool confirim)
	{
		if (id <= 0) throw new WrongRequestException("Your request is wrong");

		var existed = await _context.Tags.FirstOrDefaultAsync(c => c.Id == id);

		if (existed is null) throw new NotFoundException("There is no such Tag");

		if (confirim)
		{

			_context.Tags.Remove(existed);
			await _context.SaveChangesAsync();
			return RedirectToAction(nameof(Index));
		}
		else
		{

			return View(existed);
		}
	}
}
