using Malefashion.Models.Base;

namespace Malefashion.Models
{
    public class ProductImage : BaseEntity
    {
        public string Url { get; set; }
        public bool? IsPrimary { get; set; }
        public int ProductId { get; set; }
        public Product Product { get; set; }
    }
}
