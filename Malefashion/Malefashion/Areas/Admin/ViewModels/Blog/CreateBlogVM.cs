

using System.ComponentModel.DataAnnotations;

namespace Malefashion.Areas.Admin.ViewModels;

public class CreateBlogVM
{
    [MaxLength(75)]
    public string Name { get; set; }

    public IFormFile Photo { get; set; }

    public string? ButtonTitle { get; set; } = "Read More";

    public DateTime Data { get; set; }
}
