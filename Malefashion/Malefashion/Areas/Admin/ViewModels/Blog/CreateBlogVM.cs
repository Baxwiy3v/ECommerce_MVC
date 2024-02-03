using System.ComponentModel.DataAnnotations;

namespace Malefashion.Areas.Admin.ViewModels;

public class CreateBlogVM
{
    [Required]
    [MaxLength(75)]
    public string Name { get; set; }
    [Required]
    public IFormFile Photo { get; set; }

    public string ButtonTitle { get; set; } = "Read More";
    [Required]
    public DateTime Data { get; set; }
}
