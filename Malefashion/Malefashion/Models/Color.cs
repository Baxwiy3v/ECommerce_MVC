using Malefashion.Models.Base;

namespace Malefashion.Models
{
    public class Color:BaseNameableEntity
    {
        public List<ProductColor>? ProductColors { get; set; }
    }
}
