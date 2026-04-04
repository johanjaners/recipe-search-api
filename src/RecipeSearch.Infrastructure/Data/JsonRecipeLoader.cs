using System.Text.Json;
using RecipeSearch.Domain.Models;

namespace RecipeSearch.Infrastructure.Data;

public class JsonRecipeLoader
{
    public async Task<IReadOnlyList<Recipe>> LoadAsync(
        string filePath,
        CancellationToken cancellationToken = default)
    {
        await using var stream = File.OpenRead(filePath);
        return await LoadAsync(stream, cancellationToken);
    }

    public async Task<IReadOnlyList<Recipe>> LoadAsync(
        Stream stream,
        CancellationToken cancellationToken = default)
    {
        var recipes = new List<Recipe>();

        using var reader = new StreamReader(stream);

        while (true)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var line = await reader.ReadLineAsync();

            if (line is null)
            {
                break;
            }

            if (string.IsNullOrWhiteSpace(line))
            {
                continue;
            }

            var rawItem = JsonSerializer.Deserialize<RawRecipeItem>(line);

            if (rawItem is null)
            {
                continue;
            }

            recipes.Add(MapToRecipe(rawItem));
        }

        return recipes;
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