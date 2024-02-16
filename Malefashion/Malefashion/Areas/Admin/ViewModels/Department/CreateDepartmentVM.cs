using System.ComponentModel.DataAnnotations;

namespace Malefashion.Areas.Admin.ViewModels;

public class CreateDepartmentVM
{
	[MaxLength(50)]
	public string Name { get; set; }
}
