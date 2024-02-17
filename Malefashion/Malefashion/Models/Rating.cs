using Malefashion.Models.Base;

namespace Malefashion.Models
{
	public class Rating: BaseEntity
	{
		public int Stars { get; set; } 
		public int ProductId { get; set; }
		public Product Product { get; set; }
	}
}
