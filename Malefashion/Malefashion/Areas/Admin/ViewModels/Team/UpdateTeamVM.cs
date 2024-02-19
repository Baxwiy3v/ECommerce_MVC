using Malefashion.Models;
using System.ComponentModel.DataAnnotations;

namespace Malefashion.Areas.Admin.ViewModels;

public class UpdateTeamVM
{
    [MaxLength(50)]
    public string Name { get; set; }

	public IFormFile? Photo { get; set; }
	public string? ImageUrl { get; set; }
	[Required]
	public int? DepartmentId { get; set; }

	public List<Department>? Departments { get; set; }
}
