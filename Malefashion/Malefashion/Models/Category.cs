using Malefashion.Models.Base;

namespace Malefashion.Models
{
    public class Category : BaseNameableEntity
    {
        public List<Product>? Products { get; set; }
    }
}
