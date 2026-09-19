namespace RecipeSearch.Api.Contracts.Responses;

public class RecipeDto
{
    public string Id { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public IReadOnlyList<string> Ingredients { get; init; } = Array.Empty<string>();
    public string? Url { get; init; }
    public string? PrepTime { get; init; }
    public string? CookTime { get; init; }
    public string? Source { get; init; }
    public int Score { get; init; }
}