using System.Text.Json;
using RecipeSearch.Domain.Models;

namespace RecipeSearch.Infrastructure.Data;

public class JsonRecipeLoader
{
    public async Task<IReadOnlyList<Recipe>> LoadAsync(string filePath)
    {
        await using var stream = File.OpenRead(filePath);

        var rawItems = await JsonSerializer.DeserializeAsync<List<RawRecipeItem>>(stream)
                       ?? new List<RawRecipeItem>();

        return rawItems.Select(MapToRecipe).ToList();
    }

    private static Recipe MapToRecipe(RawRecipeItem raw)
    {
        return new Recipe
        {
            Id = raw.Id?.Oid ?? string.Empty,
            Name = raw.Name,
            IngredientsRaw = raw.Ingredients,
            Ingredients = raw.Ingredients
                .Split('\n', StringSplitOptions.RemoveEmptyEntries)
                .Select(x => x.Trim())
                .ToList(),
            Url = raw.Url,
            CookTime = raw.CookTime,
            PrepTime = raw.PrepTime,
            Source = raw.Source,
            RecipeYield = raw.RecipeYield
        };
    }
}