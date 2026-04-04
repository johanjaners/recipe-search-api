using RecipeSearch.Application.Interfaces;
using RecipeSearch.Domain.Models;

namespace RecipeSearch.Application.Services;

public class RecipeSearchService(
    IRecipeRepository recipeRepository,
    IQueryInterpretationService queryInterpretationService,
    IRecipeRankingService recipeRankingService
    ) : IRecipeSearchService
{
    public async Task<(InterpretedQuery Query, IReadOnlyList<RankedRecipeResult> Results)> SearchAsync(
        RecipeSearchQuery query,
        CancellationToken cancellationToken = default)
    {
        var interpretedQuery = await queryInterpretationService.InterpretAsync(query, cancellationToken);
        var recipes = await recipeRepository.GetAllAsync(cancellationToken);
        var rankedResults = recipeRankingService.Rank(recipes, interpretedQuery, query.Top);

        return (interpretedQuery, rankedResults);
    }
}