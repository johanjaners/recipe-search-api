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
            score += GetIngredientMatchScore(recipe, ingredient);

            if (recipe.Name.Contains(ingredient, StringComparison.OrdinalIgnoreCase))
            {
                score += 2;
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

    private static int GetIngredientMatchScore(Recipe recipe, string ingredient)
    {
        var target = ingredient.Trim().ToLowerInvariant();
        var targetTokens = target
            .Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        var bestScore = 0;

        foreach (var item in recipe.Ingredients)
        {
            var normalized = item.ToLowerInvariant();

            if (normalized.Equals(target, StringComparison.OrdinalIgnoreCase))
            {
                bestScore = Math.Max(bestScore, 6);
                continue;
            }

            if (normalized.Contains(target, StringComparison.OrdinalIgnoreCase))
            {
                bestScore = Math.Max(bestScore, 4);
                continue;
            }

            var allTokensPresent = targetTokens.All(token =>
                normalized.Contains(token, StringComparison.OrdinalIgnoreCase));

            if (allTokensPresent)
            {
                bestScore = Math.Max(bestScore, 4);
                continue;
            }

            var anyTokenPresent = targetTokens.Any(token =>
                normalized.Contains(token, StringComparison.OrdinalIgnoreCase));

            if (anyTokenPresent)
            {
                bestScore = Math.Max(bestScore, 1);
            }
        }

        return bestScore;
    }
}