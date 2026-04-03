using System.Text.Json.Serialization;

namespace RecipeSearch.Infrastructure.Data;

public class RawRecipeItem
{
    [JsonPropertyName("_id")]
    public RawObjectId? Id { get; init; }

    [JsonPropertyName("name")]
    public string Name { get; init; } = string.Empty;

    [JsonPropertyName("ingredients")]
    public string Ingredients { get; init; } = string.Empty;

    [JsonPropertyName("url")]
    public string Url { get; init; } = string.Empty;

    [JsonPropertyName("cookTime")]
    public string CookTime { get; init; } = string.Empty;

    [JsonPropertyName("prepTime")]
    public string PrepTime { get; init; } = string.Empty;

    [JsonPropertyName("source")]
    public string Source { get; init; } = string.Empty;

    [JsonPropertyName("recipeYield")]
    public string RecipeYield { get; init; } = string.Empty;
}

public class RawObjectId
{
    [JsonPropertyName("$oid")]
    public string Oid { get; init; } = string.Empty;
}