namespace Malefashion.Models.ViewModels;

public class ShopVM
{
	public List<Product> Products { get; set; }
	public List<Category> Categories { get; set; }
	public int? Order { get; set; }
	public decimal? MinPrice { get; set; }
	public decimal? MaxPrice { get; set; }
	public int? CategoryID { get; set; }
	public string? Search { get; set; }
	public double TotalPage { get; set; }
	public int CurrentPage { get; set; }
}
