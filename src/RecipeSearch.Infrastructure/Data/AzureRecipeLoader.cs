using Azure.Storage.Blobs;
using RecipeSearch.Domain.Models;

namespace RecipeSearch.Infrastructure.Data;

public class AzureBlobRecipeLoader
{
    private readonly BlobContainerClient _containerClient;
    private readonly JsonRecipeLoader _jsonRecipeLoader;

    public AzureBlobRecipeLoader(
        BlobContainerClient containerClient,
        JsonRecipeLoader jsonRecipeLoader)
    {
        _containerClient = containerClient;
        _jsonRecipeLoader = jsonRecipeLoader;
    }

    public async Task<IReadOnlyList<Recipe>> LoadAsync(
        string blobName,
        CancellationToken cancellationToken = default)
    {
        var blobClient = _containerClient.GetBlobClient(blobName);

        var response = await blobClient.DownloadStreamingAsync(
            cancellationToken: cancellationToken);

        await using var stream = response.Value.Content;

        return await _jsonRecipeLoader.LoadAsync(stream, cancellationToken);
    }
}