using Malefashion.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Malefashion.Configurations
{
    public class BannerConfiguration : IEntityTypeConfiguration<Banner>
    {
        public void Configure(EntityTypeBuilder<Banner> builder)
        {
            builder.Property(s => s.Name).IsRequired().HasMaxLength(50);
            builder.HasIndex(s => s.Name).IsUnique();

			builder.Property(s => s.ImageUrl).IsRequired();

			builder.HasIndex(s => s.Order).IsUnique();
        }
    }
}
