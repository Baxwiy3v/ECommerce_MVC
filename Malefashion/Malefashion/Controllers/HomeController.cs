using Malefashion.DAL;
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



}
