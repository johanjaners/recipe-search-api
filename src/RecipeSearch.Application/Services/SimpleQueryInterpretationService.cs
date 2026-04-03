using RecipeSearch.Application.Interfaces;
using RecipeSearch.Domain.Models;

namespace RecipeSearch.Application.Services;

public class SimpleQueryInterpretationService : IQueryInterpretationService
{
    public Task<InterpretedQuery> InterpretAsync(
        RecipeSearchQuery query,
        CancellationToken cancellationToken = default)
    {
        var keywords = query.OriginalQuery
            .ToLowerInvariant()
            .Split(new[] { ' ', ',', '.', '!', '?', ';', ':', '\n', '\r', '\t' }, StringSplitOptions.RemoveEmptyEntries)
            .Where(x => x.Length > 2)
            .Distinct()
            .ToList();

        var interpretedQuery = new InterpretedQuery
        {
            Ingredients = query.Ingredients,
            Keywords = keywords,
            TranslatedQuery = query.OriginalQuery,
            DetectedLanguage = query.Language
        };

        return Task.FromResult(interpretedQuery);
    }
}