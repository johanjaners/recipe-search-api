public class Recipe
{
    public string Id { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public string IngredientsRaw { get; init; } = string.Empty;
    public IReadOnlyList<string> Ingredients { get; init; } = Array.Empty<string>();
    public string Url { get; init; } = string.Empty;
    public string CookTime { get; init; } = string.Empty;
    public string PrepTime { get; init; } = string.Empty;
    public string Source { get; init; } = string.Empty;
    public string RecipeYield { get; init; } = string.Empty;
}