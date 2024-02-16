using System.ComponentModel.DataAnnotations;

namespace Malefashion.Areas.Admin.ViewModels;

public class UpdateBannerVM
{
	[MaxLength(50)]
	public string Name { get; set; }
	public IFormFile? Photo { get; set; }
	public string? ImageUrl { get; set; }
	public string ButtonTitle { get; set; }
	public int Order { get; set; }
}
