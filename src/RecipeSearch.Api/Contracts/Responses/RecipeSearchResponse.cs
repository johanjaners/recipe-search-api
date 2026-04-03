namespace RecipeSearch.Api.Contracts.Responses;

public class RecipeSearchResponse
{
    public IReadOnlyList<string> NormalizedIngredients { get; init; } = Array.Empty<string>();
    public IReadOnlyList<string> NormalizedKeywords { get; init; } = Array.Empty<string>();
    public IReadOnlyList<RecipeDto> Results { get; init; } = Array.Empty<RecipeDto>();
}