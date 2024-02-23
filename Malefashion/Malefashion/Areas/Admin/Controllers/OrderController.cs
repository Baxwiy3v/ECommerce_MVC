
using Malefashion.Areas.Admin.ViewModels;
using Malefashion.DAL;
using Malefashion.Models;
using Malefashion.Utilities.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Malefashion.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class OrderController : Controller
    {
        private readonly AppDbContext _context;

        public OrderController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(int page)
        {

            double count = await _context.Orders.CountAsync();

            List<Order> orders = await _context.Orders.Skip(page * 3).Take(3).ToListAsync();

            PaginationVM<Order> pagination = new()
            {
                TotalPage = Math.Ceiling(count / 3),

                CurrentPage = page,

                Items = orders
            };

            return View(pagination);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id <=0)
            {
				throw new WrongRequestException("Your request is wrong");
			}

            var order = await _context.Orders.Include(b => b.BasketItems).ThenInclude(p => p.Product).Include(o => o.AppUser).FirstOrDefaultAsync(m => m.Id == id);

            if (order == null)
            {
				throw new NotFoundException("There is no such order");
			}

            return View(order);
        }

        public async Task<IActionResult> Update(int id)
        {
            if (id <= 0)
            {
                 throw new WrongRequestException("Your request is wrong");
            }

            var order = await _context.Orders.FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
            {
                throw new NotFoundException("There is no such Order");
            }


            return View(order);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int id, bool? newStatus)
        {
            if (id <= 0)
            {
                throw new WrongRequestException("Your request is wrong");
            }

            var order = await _context.Orders.FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
            {
                throw new NotFoundException("There is no such Order");
            }

            order.Status = newStatus;
            _context.Orders.Update(order);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }







    }
}
