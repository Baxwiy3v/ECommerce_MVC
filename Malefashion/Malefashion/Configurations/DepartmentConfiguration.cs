using Malefashion.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace Malefashion.Configurations;

public class DepartmentConfiguration :IEntityTypeConfiguration<Department>
{
    public void Configure(EntityTypeBuilder<Department> builder)
    {
        builder.Property(c => c.Name).IsRequired().HasMaxLength(50);
        builder.HasIndex(c => c.Name).IsUnique();
    }
}


