using Azure.Storage.Blobs;
using RecipeSearch.Domain.Models;
using RecipeSearch.Infrastructure.Data;

namespace RecipeSearch.Api;

public static class RecipeDataLoader
{
    public static async Task<IReadOnlyList<Recipe>> LoadAsync(
        IConfiguration configuration,
        IWebHostEnvironment environment)
    {
        var jsonRecipeLoader = new JsonRecipeLoader();
        var useBlobStorage = configuration.GetValue<bool>("UseBlobStorage");

        if (useBlobStorage)
        {
            var connectionString = configuration["BlobStorage:ConnectionString"]
                ?? throw new InvalidOperationException("Missing BlobStorage:ConnectionString");

            var containerName = configuration["BlobStorage:ContainerName"]
                ?? throw new InvalidOperationException("Missing BlobStorage:ContainerName");

            var blobName = configuration["BlobStorage:BlobName"]
                ?? throw new InvalidOperationException("Missing BlobStorage:BlobName");

            var containerClient = new BlobContainerClient(connectionString, containerName);
            var blobLoader = new AzureBlobRecipeLoader(containerClient, jsonRecipeLoader);

            return await blobLoader.LoadAsync(blobName);
        }

        Console.WriteLine("Loading recipe dataset from local file.");

        var dataPath = Path.GetFullPath(Path.Combine(
            environment.ContentRootPath,
            "..", "..", "data", "20170107-061401-recipeitems.json"));

        return await jsonRecipeLoader.LoadAsync(dataPath);
    }
}