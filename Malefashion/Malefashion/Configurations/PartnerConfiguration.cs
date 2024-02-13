using Malefashion.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Malefashion.Configurations
{
	public class PartnerConfiguration : IEntityTypeConfiguration<Partner>
	{
		public void Configure(EntityTypeBuilder<Partner> builder)
		{
			

			builder.Property(s => s.ImageUrl).IsRequired();

			
		}
	}
}
