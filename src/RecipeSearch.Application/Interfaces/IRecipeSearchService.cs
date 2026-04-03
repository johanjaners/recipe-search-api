using RecipeSearch.Domain.Models;

namespace RecipeSearch.Application.Interfaces;

public interface IRecipeSearchService
{
    Task<(InterpretedQuery Query, IReadOnlyList<RankedRecipeResult> Results)> SearchAsync(
        RecipeSearchQuery query,
        CancellationToken cancellationToken = default);
}