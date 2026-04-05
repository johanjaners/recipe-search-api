using System.Text.Json;
using Microsoft.Extensions.Logging;
using RecipeSearch.Application.Interfaces;
using RecipeSearch.Domain.Models;

namespace RecipeSearch.Infrastructure.AI;

public class AzureOpenAiQueryInterpretationService(
    IChatClient chatClient,
    ILogger<AzureOpenAiQueryInterpretationService> logger)
    : IQueryInterpretationService
{
    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public async Task<InterpretedQuery> InterpretAsync(
        RecipeSearchQuery query,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var json = await chatClient.GetCompletionAsync(
                BuildSystemPrompt(),
                BuildUserPrompt(query),
                cancellationToken);

            var model = JsonSerializer.Deserialize<OpenAiInterpretedQueryResponse>(
                json,
                _jsonOptions);

            return model is null
                ? BuildFallback(query)
                : MapToInterpretedQuery(model);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Azure OpenAI interpretation failed.");
            return BuildFallback(query);
        }
    }

    private static string BuildSystemPrompt() =>
        """
        You are a recipe query interpreter.
        Return only valid JSON.
        No markdown.
        No explanations.

        Extract only terms that improve recipe retrieval.

        Rules:
        1. Translate everything to normalized English.
        2. Put concrete food items in "ingredients".
        3. Put only recipe distinguishing terms in "keywords".
        4. Valid keywords are things like dish type, flavor, style, method, dietary preference, or meal context.
        5. Do not include generic cooking verbs or filler words.
        6. Exclude words such as "cook", "prepare", "make", "want", "need", "something", "food", "dish", "recipe", "with", "using", "for".
        7. If the query only expresses a general wish to cook something, return no keywords.
        8. If a term is already in "ingredients", do not repeat it in "keywords".
        9. Keep tokens short and useful for recipe search only.

        JSON schema:
        {
        "ingredients": ["string"],
        "keywords": ["string"],
        "translatedQuery": "string",
        "detectedLanguage": "string"
        }
        """;

    private static string BuildUserPrompt(RecipeSearchQuery query) =>
        $"""
        Ingredients: {string.Join(", ", query.Ingredients)}
        Query: {query.OriginalQuery}
        Language: {query.Language}

        Convert this into normalized English recipe search terms.

        Include in "ingredients":
        concrete food items only.

        Include in "keywords":
        only dish type, flavor, cuisine, cooking style, dietary preference, or meal type.

        Do not include generic verbs or filler words.
        If none exist, return an empty keywords array.
        """;

    private static InterpretedQuery MapToInterpretedQuery(OpenAiInterpretedQueryResponse model)
    {
        var ingredients = model.Ingredients
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

        var keywords = model.Keywords
            .Except(ingredients, StringComparer.OrdinalIgnoreCase)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        return new InterpretedQuery
        {
            Ingredients = ingredients,
            Keywords = keywords,
            TranslatedQuery = model.TranslatedQuery,
            DetectedLanguage = model.DetectedLanguage
        };
    }

    private static InterpretedQuery BuildFallback(RecipeSearchQuery query)
    {
        var keywords = query.OriginalQuery
            .ToLowerInvariant()
            .Split(new[] { ' ', ',', '.', '!', '?', ';', ':', '\n', '\r', '\t' }, StringSplitOptions.RemoveEmptyEntries)
            .Where(x => x.Length > 2)
            .Distinct()
            .ToList();

        return new InterpretedQuery
        {
            Ingredients = query.Ingredients,
            Keywords = keywords,
            TranslatedQuery = query.OriginalQuery,
            DetectedLanguage = query.Language
        };
    }
}