using Microsoft.AspNetCore.Mvc;
using Malefashion.Interfaces;
using Malefashion.DAL;
using Malefashion.Models.ViewModels;

namespace Malefashion.Controllers
{
    public class ContactController : Controller
    {
		private readonly AppDbContext _context;
		private readonly IEmailService _emailService;

		public ContactController(AppDbContext context, IEmailService emailService)
        {
			_context = context;
			_emailService = emailService;
		}
        public IActionResult Index()
        {
            return View();
        }
		[HttpPost]
		public async Task<ActionResult>  Index(ContactViewModel model)
		{
            if (ModelState.IsValid)
            {
             
                await _emailService.SendMailAsync(model.Email, model.Subject, model.Message);
                ViewBag.Message = "Your message has been sent successfully.";
                ModelState.Clear(); 
            }
            return View();
        }
	}
}
