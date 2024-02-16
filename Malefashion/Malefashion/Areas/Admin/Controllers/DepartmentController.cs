using Malefashion.Areas.Admin.ViewModels;
using Malefashion.DAL;
using Malefashion.Models;
using Malefashion.Utilities.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Malefashion.Areas.Admin.Controllers;

[Area("Admin")]
public class DepartmentController : Controller
{
    private readonly AppDbContext _context;

    public DepartmentController(AppDbContext context)
    {
        _context = context;
    }
    [Authorize(Roles = "Admin,Moderator")]
    public async Task<IActionResult> Index(int page)
    {
        double count = await _context.Departments.CountAsync();

        List<Department> Departments = await _context.Departments.Skip(page * 3).Take(3).Include(p => p.Teams).ToListAsync();

        PaginationVM<Department> pagination = new()
        {
            TotalPage = Math.Ceiling(count / 3),

            CurrentPage = page,

            Items = Departments
        };
        return View(pagination);
    }
    [Authorize(Roles = "Admin,Moderator")]
    public IActionResult Create()
    {
        return View();
    }
    [HttpPost]
    public async Task<IActionResult> Create(CreateDepartmentVM DepartmentVM)
    {
        if (!ModelState.IsValid) return View(DepartmentVM);

        bool result = await _context.Departments.AnyAsync(c => c.Name.Trim().ToLower() == DepartmentVM.Name.Trim().ToLower());

        if (result)
        {
            ModelState.AddModelError("Name", "This name exists");
            return View(DepartmentVM);
        }

        Department Department = new Department
        {

            Name = DepartmentVM.Name

        };
        await _context.Departments.AddAsync(Department);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }
    [Authorize(Roles = "Admin,Moderator")]
    public async Task<IActionResult> Update(int id)
    {
        if (id <= 0) throw new WrongRequestException("Your request is wrong");


		Department? Department = await _context.Departments.FirstOrDefaultAsync(c => c.Id == id);

        if (Department == null) throw new NotFoundException("There is no such Department");

		UpdateDepartmentVM DepartmentVM = new UpdateDepartmentVM
        {

            Name = Department.Name
        };

        return View(DepartmentVM);
    }

    [HttpPost]

    public async Task<IActionResult> Update(int id, UpdateDepartmentVM DepartmentVM)
    {
        if (id <= 0) throw new WrongRequestException("Your request is wrong");

		if (!ModelState.IsValid) return View(DepartmentVM);

        Department? existed = await _context.Departments.FirstOrDefaultAsync(c => c.Id == id);

        if (existed == null) throw new NotFoundException("There is no such Department");

		bool result = await _context.Departments.AnyAsync(c => c.Name.Trim().ToLower() == DepartmentVM.Name.Trim().ToLower() && c.Id != id);

        if (result)
        {
            ModelState.AddModelError("Name", "This name exists");
            return View(DepartmentVM);
        }
        existed.Name = DepartmentVM.Name;

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id, bool confirim)
    {
        if (id <= 0) throw new WrongRequestException("Your request is wrong");

        var existed = await _context.Departments.FirstOrDefaultAsync(c => c.Id == id);

        if (existed is null) throw new NotFoundException("There is no such Department");

		if (confirim)
        {

            _context.Departments.Remove(existed);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        else
        {

            return View(existed);
        }
    }
}
