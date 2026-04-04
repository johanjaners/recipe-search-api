using Azure.Storage.Blobs;
using RecipeSearch.Domain.Models;

namespace RecipeSearch.Infrastructure.Data;

public class AzureBlobRecipeLoader(
    BlobContainerClient containerClient,
    JsonRecipeLoader jsonRecipeLoader)
{
    public async Task<IReadOnlyList<Recipe>> LoadAsync(
        string blobName,
        CancellationToken cancellationToken = default)
    {
        var blobClient = containerClient.GetBlobClient(blobName);

        var response = await blobClient.DownloadStreamingAsync(
            cancellationToken: cancellationToken);

        await using var stream = response.Value.Content;

        return await jsonRecipeLoader.LoadAsync(stream, cancellationToken);
    }
}