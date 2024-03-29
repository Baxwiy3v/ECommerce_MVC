﻿using Malefashion.Models.Base;


namespace Malefashion.Models
{
    public class Product: BaseNameableEntity
    {
        public decimal Price { get; set; }
        public decimal? OldPrice { get; set; }
        public string? Description { get; set; }
		public string? Filter { get; set; }
		public string SKU { get; set; }
		public double? AverageRating { get; set; }
		public List<Rating> Ratings { get; set; }
		public List<Comment> Comments { get; set; }
		public int CategoryId { get; set; }
        public Category Category { get; set; }
        public List<ProductImage>? ProductImages { get; set; }
        public List<ProductSize>? ProductSizes { get; set; }
        public List<ProductColor>? ProductColors { get; set; }
        public List<ProductTag>? ProductTags { get; set; }
        public Product()
        {
            ProductImages = new();
            ProductColors = new();
            ProductSizes = new();
            ProductTags = new();
        }
		
	}
}
