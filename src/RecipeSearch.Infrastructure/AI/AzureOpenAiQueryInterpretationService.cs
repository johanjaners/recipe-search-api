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
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

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

        Convert everything to normalized English tokens.
        """;

    private static InterpretedQuery MapToInterpretedQuery(OpenAiInterpretedQueryResponse model) =>
        new()
        {
            Ingredients = model.Ingredients,
            Keywords = model.Keywords,
            TranslatedQuery = model.TranslatedQuery,
            DetectedLanguage = model.DetectedLanguage
        };

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