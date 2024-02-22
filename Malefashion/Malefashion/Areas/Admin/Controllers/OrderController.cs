
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
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders.Include(b => b.BasketItems).ThenInclude(p => p.Product).Include(o => o.AppUser).FirstOrDefaultAsync(m => m.Id == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        public async Task<IActionResult> Delete(int id, bool confirim)
        {
            if (id <= 0) throw new WrongRequestException("Your request is wrong");

            var order = await _context.Orders.FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
            {
                throw new NotFoundException("There is no such Order");
            }
            

            if (confirim)
            {

                _context.Orders.Remove(order);
           
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            else
            {

                return View(order);
            }

        }
        // GET: Order/UpdateStatus/5
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

            Order existed =new Order 
            { 
               
                Status = order.Status
            };

            return View(order);
        }

        // POST: Order/UpdateStatus/5
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
