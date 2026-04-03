using RecipeSearch.Application.Interfaces;
using RecipeSearch.Domain.Models;

namespace RecipeSearch.Application.Services;

public class RecipeRankingService : IRecipeRankingService
{
    public IReadOnlyList<RankedRecipeResult> Rank(
        IReadOnlyList<Recipe> recipes,
        InterpretedQuery query,
        int top)
    {
        var results = recipes
            .Select(recipe => new RankedRecipeResult
            {
                Recipe = recipe,
                Score = CalculateScore(recipe, query)
            })
            .Where(result => result.Score > 0)
            .OrderByDescending(result => result.Score)
            .Take(top)
            .ToList();

        return results;
    }

    private static int CalculateScore(Recipe recipe, InterpretedQuery query)
    {
        var score = 0;

        foreach (var ingredient in query.Ingredients)
        {
            var hasIngredientMatch = recipe.Ingredients.Any(x =>
                x.Contains(ingredient, StringComparison.OrdinalIgnoreCase));

            if (hasIngredientMatch)
            {
                score += 5;
            }
        }

        foreach (var keyword in query.Keywords)
        {
            if (recipe.Name.Contains(keyword, StringComparison.OrdinalIgnoreCase))
            {
                score += 3;
            }
            else if (recipe.IngredientsRaw.Contains(keyword, StringComparison.OrdinalIgnoreCase))
            {
                score += 2;
            }
        }

        return score;
    }
}