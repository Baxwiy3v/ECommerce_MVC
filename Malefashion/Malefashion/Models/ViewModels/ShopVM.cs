namespace Malefashion.Models.ViewModels;

public class ShopVM
{
	public List<Product> Products { get; set; }

	public List<Category> Categories { get; set; }

    public int? Order { get; set; }
	public int? CategoryID { get; set; }
	public string? Search { get; set; }
}
