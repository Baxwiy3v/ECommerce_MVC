using System.ComponentModel.DataAnnotations;

namespace Malefashion.Areas.Admin.ViewModels;

public class UpdateTagVM
{
    [Required]
    [MaxLength(75)]
    public string Name { get; set; }
}
