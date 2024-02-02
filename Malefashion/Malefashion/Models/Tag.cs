using Malefashion.Models.Base;

namespace Malefashion.Models
{
    public class Tag : BaseNameableEntity
    {
        public List<ProductTag>? ProductTags { get; set; }
    }
}
