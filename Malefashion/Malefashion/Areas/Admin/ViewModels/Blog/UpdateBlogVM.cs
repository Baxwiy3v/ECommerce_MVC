﻿

namespace Malefashion.Areas.Admin.ViewModels;

public class UpdateBlogVM
{
 
    public string Name { get; set; }

    public IFormFile? Photo { get; set; }
    public string? ImageUrl { get; set; }

    public string ButtonTitle { get; set; } = "Read More";

    public DateTime Data { get; set; }
}
