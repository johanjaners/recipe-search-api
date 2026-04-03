namespace RecipeSearch.Domain.Models;

public class RecipeSearchQuery
{
    public IReadOnlyList<string> Ingredients { get; init; } = Array.Empty<string>();
    public string OriginalQuery { get; init; } = string.Empty;
    public string Language { get; init; } = "en";
    public int Top { get; init; } = 10;
}