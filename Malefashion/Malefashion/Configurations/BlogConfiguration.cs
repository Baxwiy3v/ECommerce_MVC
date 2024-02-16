using Malefashion.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Malefashion.Configurations
{
	public class BlogConfiguration : IEntityTypeConfiguration<Blog>
	{
		public void Configure(EntityTypeBuilder<Blog> builder)
		{
			builder.Property(c => c.Name).IsRequired().HasMaxLength(75);
			builder.HasIndex(c => c.Name).IsUnique();


			builder.Property(s => s.Data).IsRequired();
		}
	}
}
