namespace Malefashion.Models.ViewModels
{
    public class DetailVM
    {
        public Product Product { get; set; }
        public List<Product> RelatedProducts { get; set; }
        public List<Comment> Comments { get; set; }
		public List<Rating> Ratings { get; set; }
	}
}
