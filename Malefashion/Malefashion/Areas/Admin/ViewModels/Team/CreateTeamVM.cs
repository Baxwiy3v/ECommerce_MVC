using Malefashion.Models;
using System.ComponentModel.DataAnnotations;

namespace Malefashion.Areas.Admin.ViewModels;

public class CreateTeamVM
{
    public string Name { get; set; }

    public IFormFile Photo { get; set; }

	[Required]
	public int? DepartmentId { get; set; }

	public List<Department>? Departments { get; set; }

}
