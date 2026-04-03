namespace RecipeSearch.Api.Contracts.Requests;

public class RecipeSearchRequest
{
    public List<string> Ingredients { get; init; } = new();
    public string? Query { get; init; }
    public string? Language { get; init; }
    public int Top { get; init; } = 10;
}