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
public class TeamController : Controller
{
	private readonly AppDbContext _context;
	private readonly IWebHostEnvironment _env;
	public TeamController(AppDbContext context, IWebHostEnvironment env)
	{
		_context = context;

		_env = env;
	}

    [Authorize(Roles = "Admin,Moderator")]
    public async Task<IActionResult> Index(int page)
	{
		double count = await _context.Teams.CountAsync();

		List<Team> teams = await _context.Teams.Include(d=>d.Department).Skip(page * 3).Take(3).ToListAsync();
		PaginationVM<Team> pagination = new()
		{
			TotalPage = Math.Ceiling(count / 3),

			CurrentPage = page,

			Items = teams
		};
		return View(pagination);
	}

    [Authorize(Roles = "Admin,Moderator")]
    public async Task<IActionResult>  Create()
	{
		var vm = new CreateTeamVM
		{
			Departments = await _context.Departments.ToListAsync()
			
		};
		return View(vm);
	}
	[HttpPost]
	public async Task<IActionResult> Create(CreateTeamVM teamVM)
	{
		if (!ModelState.IsValid) 
		{
			teamVM.Departments = await _context.Departments.ToListAsync();
			return View(teamVM);
		} 


		if (!teamVM.Photo.ValidateType())
		{
			teamVM.Departments = await _context.Departments.ToListAsync();
			ModelState.AddModelError("Photo", "Wrong file type");
			return View(teamVM);
		}
		if (teamVM.Photo.ValidateSize(4 * 1024))
		{
			teamVM.Departments = await _context.Departments.ToListAsync();
			ModelState.AddModelError("Photo", "It shouldn't exceed 4 mb");
			return View(teamVM);
		}
		string fileName = await teamVM.Photo.CreateFile(_env.WebRootPath, "img", "about");
		

		Team team = new Team
		{
			Name = teamVM.Name,
	        DepartmentId=(int)teamVM.DepartmentId,
			ImageUrl = fileName
		
		};
		await _context.Teams.AddAsync(team);
		await _context.SaveChangesAsync();
		return RedirectToAction(nameof(Index));
	}
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id, bool confirim)
	{
		if (id <= 0) throw new WrongRequestException("Your request is wrong");
		Team? team = await _context.Teams.FirstOrDefaultAsync(s => s.Id == id);
		if (team == null) throw new NotFoundException("There is no such team");
		if (confirim)
		{

			team.ImageUrl.DeleteFile(_env.WebRootPath, "img", "about");
			_context.Teams.Remove(team);
			await _context.SaveChangesAsync();
			return RedirectToAction(nameof(Index));
		}
		else
		{

			return View(team);
		}
	}
    [Authorize(Roles = "Admin,Moderator")]
    public async Task<IActionResult> Update(int id)
	{
		if (id <= 0) throw new WrongRequestException("Your request is wrong");
		Team? team = await _context.Teams.FirstOrDefaultAsync(c => c.Id == id);
		if (team is null) throw new NotFoundException("There is no such team");

		UpdateTeamVM vm = new UpdateTeamVM
		{
			Name = team.Name,
			ImageUrl = team.ImageUrl,
			DepartmentId = team.DepartmentId,
			Departments=await _context.Departments.ToListAsync()

		};

		return View(vm);
	}

	[HttpPost]
	public async Task<IActionResult> Update(int id, UpdateTeamVM teamVM)
	{
		if (id <= 0) throw new WrongRequestException("Your request is wrong");

		if (!ModelState.IsValid)
		{
			teamVM.Departments = await _context.Departments.ToListAsync();

			return View(teamVM);
		}

		Team? existed = await _context.Teams.FirstOrDefaultAsync(c => c.Id == id);

		if (existed is null)  throw new NotFoundException("There is no such team");


		if (teamVM.Photo is not null)
		{
			if (!teamVM.Photo.ValidateType())
			{
				teamVM.Departments = await _context.Departments.ToListAsync();
				ModelState.AddModelError("Photo", "Wrong file type");
				return View(teamVM);
			}
			if (teamVM.Photo.ValidateSize(4 * 1024))
			{
				teamVM.Departments = await _context.Departments.ToListAsync();
				ModelState.AddModelError("Photo", "It shouldn't exceed 4 mb");
				return View(teamVM);
			}
			string newImage = await teamVM.Photo.CreateFile(_env.WebRootPath, "img", "about");
			existed.ImageUrl.DeleteFile(_env.WebRootPath, "img", "about");
			existed.ImageUrl = newImage;

		}
		existed.Name = teamVM.Name;
		existed.DepartmentId=(int)teamVM.DepartmentId;

		await _context.SaveChangesAsync();
		return RedirectToAction(nameof(Index));
	}
}
