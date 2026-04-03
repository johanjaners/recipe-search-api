namespace RecipeSearch.Domain.Models;

public class InterpretedQuery
{
    public IReadOnlyList<string> Ingredients { get; init; } = Array.Empty<string>();
    public IReadOnlyList<string> Keywords { get; init; } = Array.Empty<string>();
    public string TranslatedQuery { get; init; } = string.Empty;
    public string DetectedLanguage { get; init; } = "en";
}