using Malefashion.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Malefashion.Configurations
{
    public class TeamConfiguration : IEntityTypeConfiguration<Team>
    {
        public void Configure(EntityTypeBuilder<Team> builder)
        {
            builder.Property(p => p.Name).IsRequired().HasMaxLength(50);

            builder.Property(p => p.ImageUrl).IsRequired();
        }
    }
}
