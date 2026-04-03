using Azure;
using Azure.AI.OpenAI;
using OpenAI.Chat;

namespace RecipeSearch.Infrastructure.AI;

public class AzureChatClient : IChatClient
{
    private readonly ChatClient _chatClient;

    public AzureChatClient(string endpoint, string apiKey, string deploymentName)
    {
        var client = new AzureOpenAIClient(
            new Uri(endpoint),
            new AzureKeyCredential(apiKey));

        _chatClient = client.GetChatClient(deploymentName);
    }

    public async Task<string> GetCompletionAsync(
        string systemPrompt,
        string userPrompt,
        CancellationToken cancellationToken = default)
    {
        var messages = new List<ChatMessage>
        {
            new SystemChatMessage(systemPrompt),
            new UserChatMessage(userPrompt)
        };

        var options = new ChatCompletionOptions
        {
            ResponseFormat = ChatResponseFormat.CreateJsonObjectFormat(),
            Temperature = 0.2f,
            MaxOutputTokenCount = 500
        };

        var completion = await _chatClient.CompleteChatAsync(
            messages,
            options,
            cancellationToken);

        return completion.Value.Content[0].Text;
    }
}