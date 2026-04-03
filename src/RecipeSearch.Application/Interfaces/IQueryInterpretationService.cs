using RecipeSearch.Domain.Models;

namespace RecipeSearch.Application.Interfaces;

public interface IQueryInterpretationService
{
    Task<InterpretedQuery> InterpretAsync(
        RecipeSearchQuery query,
        CancellationToken cancellationToken = default);
}