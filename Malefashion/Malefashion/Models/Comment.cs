using Malefashion.Models.Base;

namespace Malefashion.Models
{
	public class Comment : BaseEntity
	{

		public string Content { get; set; }
		public DateTime CreatedAt { get; set; }
		public int ProductId { get; set; }
		public Product Product { get; set; }
		public AppUser User { get; set; }
		public string UserId { get; set; }
	}
}
