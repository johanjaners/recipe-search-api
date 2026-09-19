using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using RecipeSearch.Domain.Models;

namespace RecipeSearch.Api.Data;

public class RecipeDbContext(DbContextOptions<RecipeDbContext> options) : DbContext(options)
{
    public DbSet<Recipe> Recipes => Set<Recipe>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var ingredientsComparer = new ValueComparer<IReadOnlyList<string>>(
            (left, right) => left != null && right != null && left.SequenceEqual(right),
            value => value.Aggregate(0, (hash, item) => HashCode.Combine(hash, item.GetHashCode())),
            value => value.ToList());

        modelBuilder.Entity<Recipe>(entity =>
        {
            entity.HasKey(recipe => recipe.Id);
            entity.Property(recipe => recipe.Id).ValueGeneratedNever();
            entity.Property(recipe => recipe.Name).IsRequired();
            entity.Property(recipe => recipe.IngredientsRaw).IsRequired();
            entity.Property(recipe => recipe.Ingredients)
                .HasColumnType("jsonb")
                .HasConversion(
                    ingredients => JsonSerializer.Serialize(ingredients, (JsonSerializerOptions?)null),
                    ingredients => JsonSerializer.Deserialize<List<string>>(ingredients, (JsonSerializerOptions?)null)
                        ?? new List<string>())
                .Metadata.SetValueComparer(ingredientsComparer);
            entity.Property(recipe => recipe.Url).IsRequired();
            entity.Property(recipe => recipe.CookTime).IsRequired();
            entity.Property(recipe => recipe.PrepTime).IsRequired();
            entity.Property(recipe => recipe.Source).IsRequired();
            entity.Property(recipe => recipe.RecipeYield).IsRequired();
        });
    }
}
