using System.Text.Json;
using Microsoft.Extensions.Logging;
using RecipeSearch.Application.Interfaces;
using RecipeSearch.Domain.Models;

namespace RecipeSearch.Infrastructure.AI;

public class AzureOpenAiQueryInterpretationService : IQueryInterpretationService
{
    private readonly IChatClient _chatClient;
    private readonly ILogger<AzureOpenAiQueryInterpretationService> _logger;

    public AzureOpenAiQueryInterpretationService(IChatClient chatClient,
        ILogger<AzureOpenAiQueryInterpretationService> logger)
    {
        _chatClient = chatClient;
        _logger = logger;
    }

    public async Task<InterpretedQuery> InterpretAsync(
        RecipeSearchQuery query,
        CancellationToken cancellationToken = default)
    {
        var systemPrompt = """
        You are a recipe query interpreter.
        Return ONLY valid JSON.
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

        var userPrompt = $"""
        Ingredients: {string.Join(", ", query.Ingredients)}
        Query: {query.OriginalQuery}
        Language: {query.Language}

        Convert everything to normalized English tokens.
        """;

        try
        {
            var json = await _chatClient.GetCompletionAsync(
                systemPrompt,
                userPrompt,
                cancellationToken);

            var model = JsonSerializer.Deserialize<OpenAiInterpretedQueryResponse>(
                json,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

            if (model == null)
            {
                return BuildFallback(query);
            }

            return new InterpretedQuery
            {
                Ingredients = model.Ingredients,
                Keywords = model.Keywords,
                TranslatedQuery = model.TranslatedQuery,
                DetectedLanguage = model.DetectedLanguage
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Azure OpenAI interpretation failed. Using fallback.");
            return BuildFallback(query);
        }
    }

    private static InterpretedQuery BuildFallback(RecipeSearchQuery query)
    {

        var keywords = query.OriginalQuery
            .ToLowerInvariant()
            .Split(' ', StringSplitOptions.RemoveEmptyEntries)
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