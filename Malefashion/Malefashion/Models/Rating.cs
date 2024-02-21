using Malefashion.Models.Base;
using System.ComponentModel.DataAnnotations;

namespace Malefashion.Models
{
	public class Rating: BaseEntity
	{
		
		public int Stars { get; set; } 
		public int ProductId { get; set; }
		public Product Product { get; set; }
		public AppUser User { get; set; }
		public string UserId { get; set; }
	}
}
