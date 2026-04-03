using RecipeSearch.Domain.Models;

namespace RecipeSearch.Application.Interfaces;

public interface IRecipeRankingService
{
    IReadOnlyList<RankedRecipeResult> Rank(
        IReadOnlyList<Recipe> recipes,
        InterpretedQuery query,
        int top);
}