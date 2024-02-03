using System.ComponentModel.DataAnnotations;

namespace Malefashion.Areas.Admin.ViewModels;

public class UpdateBlogVM
{
    [Required]
    [MaxLength(75)]
    public string Name { get; set; }

    public IFormFile? Photo { get; set; }
    public string ImageUrl { get; set; }

    public string ButtonTitle { get; set; } = "Read More";
    [Required]

    public DateTime Data { get; set; }
}
