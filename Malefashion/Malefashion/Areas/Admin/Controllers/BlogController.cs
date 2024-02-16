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
public class BlogController : Controller
{
    private readonly AppDbContext _context;
    private readonly IWebHostEnvironment _env;

    public BlogController(AppDbContext context,IWebHostEnvironment env)
    {
        _context = context;
        _env = env;
    }
    [Authorize(Roles = "Admin,Moderator")]
    public async Task<IActionResult> Index(int page)
    {
        double count = await _context.Blogs.CountAsync();

        List<Blog> blogs = await _context.Blogs.Skip(page * 3).Take(3).ToListAsync();

        PaginationVM < Blog> pagination = new()
        {
            TotalPage = Math.Ceiling(count / 3),

            CurrentPage = page,

            Items = blogs
        };
        return View(pagination);
    }
    [Authorize(Roles = "Admin,Moderator")]
    public IActionResult Create()
    {
        return View();
    }
    [HttpPost]

    public async Task<IActionResult> Create(CreateBlogVM blogVM)
    {

        if(!ModelState.IsValid) return View(blogVM);

        bool result = await _context.Blogs.AnyAsync(c => c.Name.Trim().ToLower() == blogVM.Name.Trim().ToLower());

        if (result)
        {
            ModelState.AddModelError("Name", "This blog already exists");
            return View(blogVM);
        }

        if (!blogVM.Photo.ValidateType("image/"))
        {
            ModelState.AddModelError("Photo", "Wrong file type");
            return View(blogVM);
        }
        if (!blogVM.Photo.ValidateSize(4))
        {
            ModelState.AddModelError("Photo", "It shouldn't exceed 4 mb");
            return View(blogVM);
        }

        string fileName = await blogVM.Photo.CreateFile(_env.WebRootPath,  "img","blog" );

        if (blogVM.ButtonTitle is null) { blogVM.ButtonTitle = "Read More"; }

        if (blogVM.ButtonTitle is null) blogVM.ButtonTitle = "Read More";

        Blog blog = new Blog 
        {

            Name = blogVM.Name,
            Data = blogVM.Data,
            ImageUrl=fileName

        };

        await _context.Blogs.AddAsync(blog);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id, bool confirim)
    {
        if (id <= 0) throw new WrongRequestException("Your request is wrong");
		Blog? Blog = await _context.Blogs.FirstOrDefaultAsync(s => s.Id == id);
        if (Blog == null) throw new NotFoundException("There is no such blog");
		if (confirim)
        {

            Blog.ImageUrl.DeleteFile(_env.WebRootPath, "img", "blog");
            _context.Blogs.Remove(Blog);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        else
        {

            return View(Blog);
        }
    }
    [Authorize(Roles = "Admin,Moderator")]
    public async Task<IActionResult> Update(int id)
    {
        if (id <= 0) throw new WrongRequestException("Your request is wrong");
		Blog? Blog = await _context.Blogs.FirstOrDefaultAsync(c => c.Id == id);
        if (Blog is null) throw new NotFoundException("There is no such blog");

		UpdateBlogVM vm = new UpdateBlogVM
        {
             Name = Blog.Name,
             Data = Blog.Data,
            ImageUrl = Blog.ImageUrl,
            ButtonTitle = Blog.ButtonTitle
        };

        return View(vm);
    }

    [HttpPost]
    public async Task<IActionResult> Update(int id, UpdateBlogVM blogvm)
    {
		if (id <= 0) throw new WrongRequestException("Your request is wrong");
		if (!ModelState.IsValid) return View(blogvm);

        Blog? existed = await _context.Blogs.FirstOrDefaultAsync(c => c.Id == id);

        if (existed is null) throw new NotFoundException("There is no such blog");

		bool result = await _context.Blogs.AnyAsync(c => c.Name.Trim().ToLower() == blogvm.Name.Trim().ToLower() && c.Id != id);
        if (result)
        {
            ModelState.AddModelError("Name", "There is already such Name");

            return View(blogvm);
        }
       

        if (blogvm.Photo is not null)
        {
            if (!blogvm.Photo.ValidateType())
            {
                ModelState.AddModelError("Photo", "Wrong file type");
                return View(blogvm);
            }
            if (blogvm.Photo.ValidateSize(4 * 1024))
            {
                ModelState.AddModelError("Photo", "It shouldn't exceed 4 mb");
                return View(blogvm);
            }
            string newImage = await blogvm.Photo.CreateFile(_env.WebRootPath, "img", "blog");
            existed.ImageUrl.DeleteFile(_env.WebRootPath, "img", "blog");
            existed.ImageUrl = newImage;

        }
        existed.Name = blogvm.Name;
        existed.Data = blogvm.Data;
        existed.ButtonTitle = blogvm.ButtonTitle;


        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

}
