using Malefashion.Models.Base;

namespace Malefashion.Models
{
	public class Banner: BaseNameableEntity
	{
	
		public string ImageUrl { get; set; } = null!;

		public int Order { get; set; }
		public string ButtonTitle { get; set; } = "Shop Now";
	}
}
