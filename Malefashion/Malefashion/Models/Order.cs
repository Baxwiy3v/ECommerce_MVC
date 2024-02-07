using Malefashion.Models.Base;

namespace Malefashion.Models
{
    public class Order : BaseEntity
    {
        public bool? Status { get; set; }
        public string Address { get; set; }
        public decimal TotalPrice { get; set; }
        public DateTime PurchaseAt { get; set; }
        public string AppUserId { get; set; }
        public AppUser AppUser { get; set; }
        public List<BasketItem> BasketItems { get; set; }
    }
}
