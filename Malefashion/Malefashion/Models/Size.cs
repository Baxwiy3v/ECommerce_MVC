using Malefashion.Models.Base;

namespace Malefashion.Models
{
    public class Size:BaseNameableEntity
    {
        public List<ProductSize>? ProductSizes { get; set; }
    }
}
