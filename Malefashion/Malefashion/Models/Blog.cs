using Malefashion.Models.Base;

namespace Malefashion.Models
{
	public class Blog:BaseNameableEntity
	{
		public string ImageUrl { get; set; } = null!;

		public string ButtonTitle { get; set; } = "Read More";

		public DateTime Data { get; set; }
	}
}
