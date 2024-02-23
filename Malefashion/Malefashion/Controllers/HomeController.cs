using Malefashion.DAL;
using Malefashion.Models.ViewModels;
using Malefashion.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Malefashion.Utilities.Exceptions;
using System.Security.Claims;
using Malefashion.Migrations;

namespace Malefashion.Controllers;

public class HomeController : Controller
{
	private readonly AppDbContext _context;
	private readonly UserManager<AppUser> _userManager;

	public HomeController(AppDbContext context, UserManager<AppUser> userManager)
	{
		_context = context;
		_userManager = userManager;
	}
	public async Task<IActionResult> Index()
	{
		List<Slide> slides = await _context.Slides.OrderBy(s => s.Order).ToListAsync();
		List<Banner> banners = await _context.Banners.OrderBy(s => s.Order).ToListAsync();
		List<Category> categories = await _context.Categories.Include(c => c.Products).Where(c => c.Products.Count > 0).ToListAsync();
		List<Blog> blogs = await _context.Blogs.ToListAsync();
	
		HomeVM vm = new()
		{
			Slides = slides,
			Categories = categories,
			Banners = banners,
			Blogs = blogs,
			

		};
		return View(vm);
	}

	public async Task<IActionResult> About()
	{
		List<Team> teams = await _context.Teams.Include(d => d.Department).ToListAsync();
		List<Partner> partners = await _context.Partners.ToListAsync();

		AboutVM vm = new AboutVM
		{
			Partners = partners,
			Teams = teams
		};

		return View(vm);
	}

	public async Task<IActionResult> Blog()
	{
		List<Blog> blogs = await _context.Blogs.ToListAsync();


		return View(blogs);
	}


	[Authorize]
	public IActionResult Wishlist()
	{
		var wishlist = _context.WishLists.Include(x => x.AppUser).Include(x => x.Product).ThenInclude(x => x.ProductImages).Where(x => x.AppUser.UserName == User.Identity.Name).ToList();
		if (wishlist is null) wishlist = new();
		return View(wishlist);
	}

	public async Task<IActionResult> RemoveWishlist(int id)
	{
		var item = await _context.WishLists.FirstOrDefaultAsync(x => x.Id == id);
		if (item is null)
			return NotFound();
		var user = await _userManager.FindByNameAsync(User.Identity.Name);

		if (item.AppUserId != user.Id)
		{
			return BadRequest();
		}



		_context.WishLists.Remove(item);
		await _context.SaveChangesAsync();
		return RedirectToAction("Wishlist");
	}

	[Authorize]
	public async Task<IActionResult> AddWishlist(int id)
	{
		var product = await _context.Products.FirstOrDefaultAsync(x => x.Id == id);
		if (product is null)
			return NotFound();
		var user = await _userManager.FindByNameAsync(User.Identity.Name);


		var exist = await _context.WishLists.FirstOrDefaultAsync(x => x.AppUserId == user.Id && x.ProductId == product.Id);
		if (exist is not null)
		{
			_context.WishLists.Remove(exist);
			await _context.SaveChangesAsync();
			return Redirect(Request.Headers["Referer"]);
		}

		WishList wishList = new()
		{
			Product = product,
			AppUser = user
		};
		await _context.WishLists.AddAsync(wishList);
		await _context.SaveChangesAsync();
		return Redirect(Request.Headers["Referer"]);
	}

	public async Task<IActionResult> Detail(int id)
	{
		if (id <= 0) throw new WrongRequestException("Your request is wrong");
		Product product = await _context.Products.Include(p => p.Ratings).Include(c => c.Category).Include(p => p.ProductTags).ThenInclude(pt => pt.Tag).Include(p => p.ProductColors).ThenInclude(pc => pc.Color).Include(p => p.ProductSizes).ThenInclude(ps => ps.Size).Include(p => p.ProductImages).FirstOrDefaultAsync(p => p.Id == id);
		if (product == null) throw new NotFoundException("There is no such product");
		var relatedProducts = await _context.Products.Include(p => p.ProductImages).Where(p => p.CategoryId == product.CategoryId && p.Id != product.Id).ToListAsync();
		DetailVM vm = new()
		{
			Product = product,
			RelatedProducts = relatedProducts
		};
		return View(vm);
	}
	private double CalculateAverageRating(int productId)
	{
		
		var ratingsForProduct = _context.Ratings.Where(r => r.ProductId == productId).ToList();

		if (ratingsForProduct.Count > 0)
		{
		
			int totalStars = 0;
			int numberOfRatings = ratingsForProduct.Count;

		
			foreach (var rating in ratingsForProduct)
			{
				totalStars += rating.Stars;
			}

			
			double averageRating = (double)totalStars / numberOfRatings;

			return averageRating;
		}
		else
		{
			
			return 0;
		}
	}





	[HttpPost]
	[Authorize]
	public async Task<IActionResult> AddRating(int productId, int stars)
	{
		var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

		var product = await _context.Products.FindAsync(productId);

		if (product == null)
		{
			return NotFound();
		}

		var existingRating = await _context.Ratings.FirstOrDefaultAsync(r => r.ProductId == productId && r.UserId == userId);

		if (existingRating != null)
		{
			_context.Ratings.Remove(existingRating);
		}

		var rating = new Rating
		{
			Stars = stars,
			ProductId = productId,
			UserId = userId
		};
		
		_context.Ratings.Add(rating);

		product.AverageRating = CalculateAverageRating(productId);

		await _context.SaveChangesAsync();



		return Redirect(Request.Headers["Referer"]);
	}

	[HttpPost]
	[Authorize]

	public async Task<IActionResult> AddComment(int productId, string content)
	{
		var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);


		var product = await _context.Products.FindAsync(productId);

		if (product == null)
		{
			return NotFound();
		}

		var comment = new Comment
		{
			Content = content,
			ProductId = productId,
			CreatedAt = DateTime.Now,
			UserId=userId
		};

		_context.Comments.Add(comment);
		await _context.SaveChangesAsync();

		return Redirect(Request.Headers["Referer"]);
	}



	public IActionResult ErrorPage(string error)
	{
		return View(model: error);
	}

}
