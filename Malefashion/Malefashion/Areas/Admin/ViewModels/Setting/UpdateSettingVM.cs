using System.ComponentModel.DataAnnotations;

namespace Malefashion.Areas.Admin.ViewModels;

public class UpdateSettingVM
{
    [MaxLength(50)]
    public string Key { get; set; }
    [MaxLength(100)]
    public string Value { get; set; }
}
