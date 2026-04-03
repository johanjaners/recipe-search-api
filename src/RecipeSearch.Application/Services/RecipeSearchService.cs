using RecipeSearch.Application.Interfaces;
using RecipeSearch.Domain.Models;

namespace RecipeSearch.Application.Services;

public class RecipeSearchService : IRecipeSearchService
{
    private readonly IRecipeRepository _recipeRepository;
    private readonly IQueryInterpretationService _queryInterpretationService;
    private readonly IRecipeRankingService _recipeRankingService;

    public RecipeSearchService(
        IRecipeRepository recipeRepository,
        IQueryInterpretationService queryInterpretationService,
        IRecipeRankingService recipeRankingService)
    {
        _recipeRepository = recipeRepository;
        _queryInterpretationService = queryInterpretationService;
        _recipeRankingService = recipeRankingService;
    }

    public async Task<(InterpretedQuery Query, IReadOnlyList<RankedRecipeResult> Results)> SearchAsync(
        RecipeSearchQuery query,
        CancellationToken cancellationToken = default)
    {
        var interpretedQuery = await _queryInterpretationService.InterpretAsync(query, cancellationToken);
        var recipes = await _recipeRepository.GetAllAsync(cancellationToken);
        var rankedResults = _recipeRankingService.Rank(recipes, interpretedQuery, query.Top);

        return (interpretedQuery, rankedResults);
    }
}