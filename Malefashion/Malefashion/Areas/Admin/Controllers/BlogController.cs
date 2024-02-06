using Malefashion.Areas.Admin.ViewModels;
using Malefashion.DAL;
using Malefashion.Models;
using Malefashion.Utilities.Extentions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Malefashion.Areas.Admin.Controllers
{
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
            if (!blogVM.Photo.ValidateSize(3))
            {
                ModelState.AddModelError("Photo", "Wrong file size");
                return View(blogVM);
            }

            string fileName = await blogVM.Photo.CreateFile(_env.WebRootPath,  "img","blog" );

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
    }
}
