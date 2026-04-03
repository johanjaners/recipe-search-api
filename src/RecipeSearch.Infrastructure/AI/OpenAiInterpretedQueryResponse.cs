namespace RecipeSearch.Infrastructure.AI;

public class OpenAiInterpretedQueryResponse
{
    public List<string> Ingredients { get; set; } = new();
    public List<string> Keywords { get; set; } = new();
    public string TranslatedQuery { get; set; } = string.Empty;
    public string DetectedLanguage { get; set; } = "en";
}