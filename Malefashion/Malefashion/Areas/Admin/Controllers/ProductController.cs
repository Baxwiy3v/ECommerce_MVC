using AutoMapper;
using Malefashion.Areas.Admin.ViewModels;
using Malefashion.DAL;
using Malefashion.Models;
using Malefashion.Utilities.Exceptions;
using Malefashion.Utilities.Extentions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Malefashion.Areas.Admin.Controllers
{
	[Area("Admin")]
	public class ProductController : Controller
	{
		private readonly AppDbContext _context;
		private readonly IWebHostEnvironment _env;

		public ProductController(AppDbContext context,  IWebHostEnvironment env)
		{
			_context = context;
			_env = env;
		}
        [Authorize(Roles = "Admin,Moderator")]
        public async Task<IActionResult> Index(int page)
		{
			double count = await _context.Products.CountAsync();

			List<Product> products = await _context.Products.Skip(page * 3).Take(3).Include(p => p.ProductImages).Include(p => p.Category).ToListAsync();

			PaginationVM<Product> pagination = new()
			{
				TotalPage = Math.Ceiling(count / 3),

				CurrentPage = page,

				Items = products
			};
			return View(pagination);
		}
        [Authorize(Roles = "Admin,Moderator")]
        public async Task<IActionResult> Create()
		{
			var vm = new CreateProductVM
			{
				Categories = await _context.Categories.ToListAsync(),
				Sizes = await _context.Sizes.ToListAsync(),
				Colors = await _context.Colors.ToListAsync(),
				Tags = await _context.Tags.ToListAsync(),
			};
			return View(vm);
		}

		[HttpPost]

		public async Task<IActionResult> Create(CreateProductVM productVM)
		{
			if (!ModelState.IsValid)
			{
				productVM.Categories = await _context.Categories.ToListAsync();
				productVM.Sizes = await _context.Sizes.ToListAsync();
				productVM.Colors = await _context.Colors.ToListAsync();
				productVM.Tags = await _context.Tags.ToListAsync();
				return View(productVM);
			}

			bool existed= await _context.Products.AnyAsync(p=>p.Name.Trim().ToLower()==productVM.Name.Trim().ToLower());

			if (existed)
			{
				productVM.Categories = await _context.Categories.ToListAsync();
				productVM.Sizes = await _context.Sizes.ToListAsync();
				productVM.Colors = await _context.Colors.ToListAsync();
				productVM.Tags = await _context.Tags.ToListAsync();
				ModelState.AddModelError("Name", "This name already exists");
				return View(productVM);

			}


			bool result = await _context.Categories.AnyAsync(c => c.Id == productVM.CategoryId);
			if (!result)
			{
				productVM.Categories = await _context.Categories.ToListAsync();
				productVM.Sizes = await _context.Sizes.ToListAsync();
				productVM.Colors = await _context.Colors.ToListAsync();
				productVM.Tags = await _context.Tags.ToListAsync();
				ModelState.AddModelError("CategoryId", "There is no such category");
				return View(productVM);
			}
			foreach (int id in productVM.SizeIds)
			{
				bool sizeResult = await _context.Sizes.AnyAsync(t => t.Id == id);
				if (!sizeResult)
				{
					productVM.Categories = await _context.Categories.ToListAsync();
					productVM.Sizes = await _context.Sizes.ToListAsync();
					productVM.Colors = await _context.Colors.ToListAsync();
					productVM.Tags = await _context.Tags.ToListAsync();
					ModelState.AddModelError("SizeIds", "There is no such size");
					return View(productVM);
				}
			}
			foreach (int id in productVM.TagIds)
			{
				bool sizeResult = await _context.Tags.AnyAsync(t => t.Id == id);
				if (!sizeResult)
				{
					productVM.Categories = await _context.Categories.ToListAsync();
					productVM.Sizes = await _context.Sizes.ToListAsync();
					productVM.Colors = await _context.Colors.ToListAsync();
					productVM.Tags = await _context.Tags.ToListAsync();
					ModelState.AddModelError("TagIds", "There is no such tag");
					return View(productVM);
				}
			}
			foreach (int id in productVM.ColorIds)
			{
				bool sizeResult = await _context.Colors.AnyAsync(t => t.Id == id);
				if (!sizeResult)
				{
					productVM.Categories = await _context.Categories.ToListAsync();
					productVM.Sizes = await _context.Sizes.ToListAsync();
					productVM.Colors = await _context.Colors.ToListAsync();
					productVM.Tags = await _context.Tags.ToListAsync();
					ModelState.AddModelError("ColorIds", "There is no such color");
					return View(productVM);
				}
			}
			if (!productVM.MainPhoto.ValidateType())
			{
				productVM.Sizes = await _context.Sizes.ToListAsync();
				productVM.Categories = await _context.Categories.ToListAsync();
				productVM.Colors = await _context.Colors.ToListAsync();
				productVM.Tags = await _context.Tags.ToListAsync();
				ModelState.AddModelError("MainPhoto", "Wrong file type");
				return View(productVM);
			}
			if (!productVM.MainPhoto.ValidateSize(6))
			{
				productVM.Categories = await _context.Categories.ToListAsync();
				productVM.Sizes = await _context.Sizes.ToListAsync();
				productVM.Colors = await _context.Colors.ToListAsync();
				productVM.Tags = await _context.Tags.ToListAsync();
				ModelState.AddModelError("MainPhoto", "Wrong file size");
				return View(productVM);
			}
			ProductImage image = new ProductImage
			{
				IsPrimary = true,
				Url = await productVM.MainPhoto.CreateFile(_env.WebRootPath, "img","product")
			};

			Product product = new Product
			{
				Name = productVM.Name,
				Price = productVM.Price,
				CategoryId = (int)productVM.CategoryId,
				Description = productVM.Description,
				Filter = productVM.Filter,
				SKU = productVM.SKU,
				ProductTags = new(),
				ProductColors = new(),
				ProductSizes = new(),
				ProductImages = new()
				{
					image
				}
			};

			foreach (int id in productVM.TagIds)
			{
				var pTag = new ProductTag
				{
					TagId = id,
					Product = product
				};
				product.ProductTags.Add(pTag);
			}

			foreach (int id in productVM.SizeIds)
			{
				var pSize = new ProductSize
				{
					SizeId = id,
					Product = product
				};
				product.ProductSizes.Add(pSize);
			}
			foreach (int id in productVM.ColorIds)
			{
				var pColor = new ProductColor
				{
					ColorId = id,
					Product = product
				};
				product.ProductColors.Add(pColor);
			}
			TempData["Message"] = "";
			if (productVM.Photos is not null)
			{
				foreach (IFormFile photo in productVM.Photos)
				{
					if (!photo.ValidateType())
					{
						TempData["Message"] += $"<p class=\"text-danger\">{photo.FileName} file type wrong</p>";
						continue;
					}
					if (!photo.ValidateSize(6))
					{
						TempData["Message"] += $"<p class=\"text-danger\">{photo.FileName} file size wrong</p>";
						continue;
					}

					product.ProductImages.Add(new ProductImage
					{
						IsPrimary = false,
						Url = await photo.CreateFile(_env.WebRootPath, "img", "product")
					});

				}
			}
			await _context.Products.AddAsync(product);
			await _context.SaveChangesAsync();

			return RedirectToAction(nameof(Index));
		}
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id,bool confirim)
		{
			if (id <= 0) throw new WrongRequestException("Your request is wrong");
			var existed = await _context.Products.Include(p => p.ProductImages).FirstOrDefaultAsync(c => c.Id == id);
			if (existed is null) throw new NotFoundException("There is no such product");

			if (confirim)
			{

				foreach (ProductImage image in existed.ProductImages)
				{
					image.Url.DeleteFile(_env.WebRootPath, "img", "product");
				}
				_context.Products.Remove(existed);
				await _context.SaveChangesAsync();
				return RedirectToAction(nameof(Index));
			}
			else
			{

				return View(existed);
			}
			
		}
        [Authorize(Roles = "Admin,Moderator")]
        public async Task<IActionResult> Update(int id)
		{
			if (id <= 0) throw new WrongRequestException("Your request is wrong");
			var product = await _context.Products
				.Include(p => p.ProductImages)
				.Include(p => p.ProductSizes)
				.Include(p => p.ProductColors)
				.Include(p => p.ProductTags)
				.FirstOrDefaultAsync(p => p.Id == id);
			if (product == null) throw new NotFoundException("There is no such product");
			var vm = new UpdateProductVM
			{
				Name = product.Name,
				Description = product.Description,
				Filter = product.Filter,
				SKU = product.SKU,
				Price = product.Price,
				CategoryId = product.CategoryId,
				ProductImages = product.ProductImages,
				TagIds = product.ProductTags.Select(ps => ps.TagId).ToList(),
				SizeIds = product.ProductSizes.Select(ps => ps.SizeId).ToList(),
				ColorIds = product.ProductColors.Select(ps => ps.ColorId).ToList(),
				Categories = await _context.Categories.ToListAsync(),
				Tags = await _context.Tags.ToListAsync(),
				Sizes = await _context.Sizes.ToListAsync(),
				Colors = await _context.Colors.ToListAsync()

			};
			return View(vm);
		}
		[HttpPost]

		public async Task<IActionResult> Update(int id, UpdateProductVM productVM)
		{
			if (id <= 0) throw new WrongRequestException("Your request is wrong");

			Product? existed = await _context.Products
				.Include(p => p.ProductImages)
				.Include(p => p.ProductSizes)
				.Include(p => p.ProductColors)
				.Include(p => p.ProductTags)
				.FirstOrDefaultAsync(c => c.Id == id);
			productVM.ProductImages = existed.ProductImages;
			if (!ModelState.IsValid)
			{
				productVM.Categories = await _context.Categories.ToListAsync();
				productVM.Sizes = await _context.Sizes.ToListAsync();
				productVM.Colors = await _context.Colors.ToListAsync();
				productVM.Tags = await _context.Tags.ToListAsync();

				return View(productVM);
			}
			if (existed is null) throw new NotFoundException("There is no such product"); 

			bool result = await _context.Products.AnyAsync(c => c.CategoryId == productVM.CategoryId);
		
			foreach (int idS in productVM.SizeIds)
			{
				bool sizeResult = await _context.Sizes.AnyAsync(t => t.Id == idS);
				if (!sizeResult)
				{
					productVM.Categories = await _context.Categories.ToListAsync();
					productVM.Sizes = await _context.Sizes.ToListAsync();
					productVM.Colors = await _context.Colors.ToListAsync();
					productVM.Tags = await _context.Tags.ToListAsync();
					ModelState.AddModelError("SizeIds", "There is no such size");
					return View(productVM);
				}
			}
			foreach (int idS in productVM.TagIds)
			{
				bool TagResult = await _context.Tags.AnyAsync(t => t.Id == idS);
				if (!TagResult)
				{
					productVM.Categories = await _context.Categories.ToListAsync();
					productVM.Sizes = await _context.Sizes.ToListAsync();
					productVM.Colors = await _context.Colors.ToListAsync();
					productVM.Tags = await _context.Tags.ToListAsync();
					ModelState.AddModelError("TagIds", "There is no such tag");
					return View(productVM);
				}
			}
			foreach (int idS in productVM.ColorIds)
			{
				bool colorResult = await _context.Colors.AnyAsync(t => t.Id == idS);
				if (!colorResult)
				{
					productVM.Categories = await _context.Categories.ToListAsync();
					productVM.Sizes = await _context.Sizes.ToListAsync();
					productVM.Colors = await _context.Colors.ToListAsync();
					productVM.Tags = await _context.Tags.ToListAsync();
					ModelState.AddModelError("ColorIds", "There is no such color");
					return View(productVM);
				}
			}

			 result = _context.Products.Any(c => c.Name == productVM.Name && c.Id != id);
			if (result)
			{
				productVM.Categories = await _context.Categories.ToListAsync();
				productVM.Colors = await _context.Colors.ToListAsync();
				productVM.Sizes = await _context.Sizes.ToListAsync();
				productVM.Tags = await _context.Tags.ToListAsync();
				ModelState.AddModelError("Name", "There is already such product");
				return View(productVM);
			}

			if (productVM.MainPhoto is not null)
			{
				if (!productVM.MainPhoto.ValidateType())
				{
					productVM.Categories = await _context.Categories.ToListAsync();
					productVM.Sizes = await _context.Sizes.ToListAsync();
					productVM.Colors = await _context.Colors.ToListAsync();
					productVM.Tags = await _context.Tags.ToListAsync();
					ModelState.AddModelError("MainPhoto", "File type is not valid");
					return View(productVM);
				}
				if (!productVM.MainPhoto.ValidateSize(4))
				{
					productVM.Categories = await _context.Categories.ToListAsync();
					productVM.Sizes = await _context.Sizes.ToListAsync();
					productVM.Colors = await _context.Colors.ToListAsync();
					productVM.Tags = await _context.Tags.ToListAsync();
					ModelState.AddModelError("MainPhoto", "File size is not valid");
					return View(productVM);
				}
			}
			if (productVM.MainPhoto is not null)
			{
				string fileName = await productVM.MainPhoto.CreateFile(_env.WebRootPath, "img", "product");
				ProductImage mainImage = existed.ProductImages.FirstOrDefault(pi => pi.IsPrimary == true);
				mainImage.Url.DeleteFile(_env.WebRootPath, "img", "product");
				_context.ProductImages.Remove(mainImage);
				existed.ProductImages.Add(new ProductImage
				{
					IsPrimary = true,
					Url = fileName
				});
			}

			if (productVM.ImageIds is null)
			{
				productVM.ImageIds = new();
			}
			var removeable = existed.ProductImages.Where(pi => !productVM.ImageIds.Exists(imgId => imgId == pi.Id) && pi.IsPrimary == false).ToList();
			foreach (ProductImage pi in removeable)
			{
				pi.Url.DeleteFile(_env.WebRootPath, "img", "product");
				existed.ProductImages.Remove(pi);
			}

			existed.ProductSizes.RemoveAll(pSize => !productVM.SizeIds.Contains(pSize.Id));

			existed.ProductSizes.AddRange(productVM.SizeIds.Where(sizeId => !existed.ProductSizes.Any(pt => pt.Id == sizeId))
								 .Select(sizeId => new ProductSize { SizeId = sizeId }));

			existed.ProductTags.RemoveAll(pTag => !productVM.TagIds.Contains(pTag.Id));

			existed.ProductTags.AddRange(productVM.TagIds.Where(tagId => !existed.ProductTags.Any(pt => pt.Id == tagId))
								 .Select(tagId => new ProductTag { TagId = tagId }));

			existed.ProductColors.RemoveAll(pColor => !productVM.ColorIds.Contains(pColor.Id));

			existed.ProductColors.AddRange(productVM.ColorIds.Where(colorId => !existed.ProductColors.Any(pt => pt.Id == colorId))
								 .Select(colorId => new ProductColor { ColorId = colorId }));

			TempData["Message"] = "";
			if (productVM.Photos is not null)
			{
				foreach (IFormFile photo in productVM.Photos)
				{
					if (!photo.ValidateType())
					{
						TempData["Message"] += $"<p class=\"text-danger\">{photo.FileName} file type wrong</p>";
						continue;
					}
					if (!photo.ValidateSize(6))
					{
						TempData["Message"] += $"<p class=\"text-danger\">{photo.FileName} file size wrong</p>";
						continue;
					}

					existed.ProductImages.Add(new ProductImage
					{
						IsPrimary = false,
						Url = await photo.CreateFile(_env.WebRootPath, "img", "product")
					});
				}
			}

			existed.Name = productVM.Name;
			existed.Description = productVM.Description;
			existed.Filter = productVM.Filter;
			existed.SKU = productVM.SKU;
			if (productVM.Price != existed.Price) existed.OldPrice = existed.Price;

			existed.Price = productVM.Price;
			existed.CategoryId = productVM.CategoryId;
			await _context.SaveChangesAsync();
			return RedirectToAction(nameof(Index));
		}




        [Authorize(Roles = "Admin,Moderator")]
        public async Task<IActionResult> Details(int id)
        {
            if (id <= 0) return BadRequest();
            var product = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.ProductTags).ThenInclude(pt => pt.Tag)
                .Include(p => p.ProductColors).ThenInclude(pc => pc.Color)
                .Include(p => p.ProductImages)
                .Include(p => p.ProductSizes).ThenInclude(ps => ps.Size)
                .FirstOrDefaultAsync(p => p.Id == id);
            if (product == null) return NotFound();

            return View(product);
        }

    }
}
