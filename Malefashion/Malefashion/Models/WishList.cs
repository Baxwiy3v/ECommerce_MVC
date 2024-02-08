using Malefashion.Models.Base;

namespace Malefashion.Models
{
	public class WishList:BaseEntity
	{
		
		public AppUser AppUser { get; set; }
		public string AppUserId { get; set; }
		public Product Product { get; set; }
		public int ProductId { get; set; }
	}
}
