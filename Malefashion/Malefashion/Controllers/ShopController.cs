using Malefashion.DAL;
using Malefashion.Models;
using Malefashion.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Immutable;

namespace Malefashion.Controllers;

public class ShopController : Controller
{
	private readonly AppDbContext _context;

	public ShopController(AppDbContext context)
	{
		_context = context;
	}
	public async Task<IActionResult> Index(string? search, int? order,int? categoryId, int page, decimal? minPrice, decimal? maxPrice)
	{

		


		IQueryable<Product> queryable = _context.Products;

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
		if (minPrice.HasValue)
		{
			queryable = queryable.Where(p => p.Price >= minPrice.Value);
		}

		if (maxPrice.HasValue)
		{
			queryable = queryable.Where(p => p.Price <= maxPrice.Value);
		}

		if (!String.IsNullOrEmpty(search))
		{
			queryable = queryable.Where(p => p.Name.ToLower().Trim().Contains(search.ToLower().Trim()));
		}

		if(categoryId!=null)
		{ 
			queryable=queryable.Where(c=>c.CategoryId==categoryId);
		}

        double count = queryable.Count();

        queryable = queryable.Skip(page * 6).Take(6).Include(p => p.ProductImages);
      

        var produtcs = await queryable.ToListAsync();

        ShopVM vm = new ShopVM
		{
			TotalPage = Math.Ceiling(count / 6),
			CurrentPage = page,
			Categories = await _context.Categories.Include(c => c.Products).ToListAsync(),
			Products = produtcs,
            CategoryID = categoryId,
			Order = order,
			Search = search,
			MaxPrice = maxPrice,
			MinPrice = minPrice

		};
		return View(vm);
	}

	
}
