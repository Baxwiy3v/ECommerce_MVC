using System.ComponentModel.DataAnnotations;

namespace Malefashion.Areas.Admin.ViewModels;

public class CreateBannerVM
{

	[MaxLength(50)]
	public string Name { get; set; }
	public IFormFile Photo { get; set; }
	public string? ButtonTitle { get; set; }
	public int Order { get; set; }

}
