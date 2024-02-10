﻿using Malefashion.DAL;
using Malefashion.Models;
using Malefashion.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Malefashion.Controllers;

public class ShopController : Controller
{
	private readonly AppDbContext _context;

	public ShopController(AppDbContext context)
	{
		_context = context;
	}
	public async Task<IActionResult> Index(string? search, int? order,int? categoryId )
	{
		IQueryable<Product> queryable = _context.Products.Include(p => p.ProductImages).AsQueryable();

		switch (order)
		{
			case 1:
				queryable = queryable.OrderBy(p => p.Name);
				break;

			case 2:
				queryable = queryable.OrderBy(p => p.Price);
				break;

			case 3:
				queryable = queryable.OrderByDescending(p => p.Id);
				break;

		}

		if (!String.IsNullOrEmpty(search))
		{
			queryable = queryable.Where(p => p.Name.ToLower().Trim().Contains(search.ToLower().Trim()));
		}

		if(categoryId!=null)
		{ 
			queryable=queryable.Where(c=>c.CategoryId==categoryId);
		}


		ShopVM vm = new ShopVM
		{
			Categories = await _context.Categories.Include(c => c.Products).ToListAsync(),
			Products = await queryable.ToListAsync(),
			CategoryID = categoryId,
			Order = order,
			Search = search

		};
		return View(vm);
	}//
}