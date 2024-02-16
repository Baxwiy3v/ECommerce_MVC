
using System.ComponentModel.DataAnnotations;

namespace Malefashion.Areas.Admin.ViewModels;

public class CreateSlideVM
{
	[MaxLength(50)]
	public string Title { get; set; }
	[MaxLength(50)]
	public string SubTitle { get; set; }

	[MaxLength(100)]
	public string? Description { get; set; }

	public IFormFile Photo { get; set; }
	public string? ButtonTitle { get; set; }
	public int Order { get; set; }
	public string? FbLink { get; set; }
	public string? TwLink { get; set; }
	public string? IgLink { get; set; }
	public string? PtLink { get; set; }
}
