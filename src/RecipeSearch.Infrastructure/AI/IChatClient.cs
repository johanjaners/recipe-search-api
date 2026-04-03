namespace RecipeSearch.Infrastructure.AI;

public interface IChatClient
{
    Task<string> GetCompletionAsync(
        string systemPrompt,
        string userPrompt,
        CancellationToken cancellationToken = default);
}