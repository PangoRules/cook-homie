using CookHomie.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CookHomie.Infrastructure.Persistence.Configurations;

public class ShoppingItemConfiguration : IEntityTypeConfiguration<ShoppingItem>
{
    public void Configure(EntityTypeBuilder<ShoppingItem> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Name).IsRequired();
        builder.Property(e => e.CreatedAt).IsRequired();
        builder
            .HasOne(e => e.LinkedRecipe)
            .WithMany()
            .HasForeignKey(e => e.LinkedRecipeId)
            .OnDelete(DeleteBehavior.SetNull)
            .IsRequired(false);
    }
}

