using Malefashion.Models.Base;

namespace Malefashion.Models
{
	public class Slide : BaseEntity
	{
		public string Title { get; set; }
		public string SubTitle { get; set; }
		public string Description { get; set; }
		public string ImageUrl { get; set; } 
		public string ButtonTitle { get; set; } = "Shop Now";
		public int Order { get; set; }
		public string FbLink { get; set; }
        public string TwLink { get; set; }
        public string IgLink { get; set; }
        public string PtLink { get; set; }

    }
}
