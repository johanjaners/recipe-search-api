using Azure.Storage.Blobs;
using RecipeSearch.Application.Interfaces;
using RecipeSearch.Application.Services;
using RecipeSearch.Infrastructure.AI;

namespace RecipeSearch.Api.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAzureOpenAI(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddSingleton<IChatClient>(sp =>
        {
            var endpoint = configuration["AzureOpenAI:Endpoint"]
                ?? throw new InvalidOperationException("Missing AzureOpenAI:Endpoint");

            var apiKey = configuration["AzureOpenAI:ApiKey"]
                ?? throw new InvalidOperationException("Missing AzureOpenAI:ApiKey");

            var deployment = configuration["AzureOpenAI:DeploymentName"]
                ?? throw new InvalidOperationException("Missing AzureOpenAI:DeploymentName");

            return new AzureChatClient(endpoint, apiKey, deployment);
        });

        return services;
    }

    public static IServiceCollection AddApplicationServices(
        this IServiceCollection services)
    {
        services.AddScoped<IQueryInterpretationService, AzureOpenAiQueryInterpretationService>();
        services.AddScoped<IRecipeRankingService, RecipeRankingService>();
        services.AddScoped<IRecipeSearchService, RecipeSearchService>();

        return services;
    }
}