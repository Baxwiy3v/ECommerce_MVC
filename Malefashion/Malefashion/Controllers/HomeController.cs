﻿using Malefashion.DAL;
using Malefashion.Models.ViewModels;
using Malefashion.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Malefashion.Controllers;

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
        List<Category> categories = await _context.Categories.Include(c => c.Products).Where(c => c.Products.Count > 0).ToListAsync();
        HomeVM vm = new()
        {
            Slides = slides,
            Categories = categories

        };
        return View(vm);
    }

	public async Task<IActionResult> Blog()
	{
		List<Blog> blogs = await _context.Blogs.ToListAsync();

		
		return View(blogs);
	}


	public async Task<IActionResult> Detail(int id)
    {
        if (id <= 0) return BadRequest();
        Product product = await _context.Products.Include(c=>c.Category).Include(p => p.ProductTags).ThenInclude(pt => pt.Tag).Include(p => p.ProductColors).ThenInclude(pc => pc.Color).Include(p => p.ProductSizes).ThenInclude(ps => ps.Size).Include(p => p.ProductImages).FirstOrDefaultAsync(p => p.Id == id);
        if (product == null) return NotFound();
        var relatedProducts = await _context.Products.Include(p => p.ProductImages).Where(p => p.CategoryId == product.CategoryId && p.Id != product.Id).ToListAsync();
        DetailVM vm = new()
        {
            Product = product,
            RelatedProducts = relatedProducts
        };
        return View(vm);
    }

	

	public async Task<IActionResult> ShopPage(int page, int sortId, int? catId)
    {
        List<Product> products;
        double count;
        if (catId <= 0) return BadRequest();
        if (catId is not null)
        {
            products = await _context.Products.Skip(page * 3).Take(3).Include(c => c.Category).Include(p => p.ProductImages).Where(p => p.CategoryId == catId).ToListAsync();
            count = await _context.Products.Include(c => c.Category).Where(p => p.CategoryId == catId).CountAsync();
        }
        else
        {
            products = await _context.Products.Skip(page * 3).Take(3).Include(c => c.Category).Include(p => p.ProductImages).ToListAsync();
            count = await _context.Products.CountAsync();
        }

        switch (sortId)
        {
            case 1:
                products = products.OrderBy(p => p.CreatedTime).ToList();
                break;
            case 2:
                products = products.OrderBy(p => p.Name).ToList();
                break;
            case 3:
                products = products.OrderBy(p => p.Price).ToList();
                break;
            default:
                break;
        }

        PaginationVM<Product> pagination = new()
        {
            TotalPage = Math.Ceiling(count / 3),
            CurrentPage = page,
            Items = products
        };
        return View(pagination);
    }


}
