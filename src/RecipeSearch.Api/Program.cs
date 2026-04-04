using Azure.Storage.Blobs;
using RecipeSearch.Application.Interfaces;
using RecipeSearch.Application.Services;
using RecipeSearch.Infrastructure.AI;
using RecipeSearch.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddUserSecrets<Program>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<IChatClient>(sp =>
{
    var endpoint = builder.Configuration["AzureOpenAI:Endpoint"]
        ?? throw new InvalidOperationException("Missing endpoint");

    var apiKey = builder.Configuration["AzureOpenAI:ApiKey"]
        ?? throw new InvalidOperationException("Missing api key");

    var deployment = builder.Configuration["AzureOpenAI:DeploymentName"]
        ?? throw new InvalidOperationException("Missing deployment");

    return new AzureChatClient(endpoint, apiKey, deployment);
});

var jsonRecipeLoader = new JsonRecipeLoader();
IReadOnlyList<RecipeSearch.Domain.Models.Recipe> recipes;

var useBlobStorage = builder.Configuration.GetValue<bool>("UseBlobStorage");

if (useBlobStorage)
{
    var connectionString = builder.Configuration["BlobStorage:ConnectionString"]
        ?? throw new InvalidOperationException("Missing blob connection string");

    var containerName = builder.Configuration["BlobStorage:ContainerName"]
        ?? throw new InvalidOperationException("Missing blob container name");

    var blobName = builder.Configuration["BlobStorage:BlobName"]
        ?? throw new InvalidOperationException("Missing blob name");

    var containerClient = new BlobContainerClient(connectionString, containerName);
    var blobLoader = new AzureBlobRecipeLoader(containerClient, jsonRecipeLoader);

    recipes = await blobLoader.LoadAsync(blobName);
}
else
{
    Console.WriteLine("Loading recipe dataset from local file.");

    var dataPath = Path.Combine(
        builder.Environment.ContentRootPath,
        "..",
        "..",
        "data",
        "20170107-061401-recipeitems.json");

    var fullDataPath = Path.GetFullPath(dataPath);
    recipes = await jsonRecipeLoader.LoadAsync(fullDataPath);
}

builder.Services.AddSingleton<IRecipeRepository>(
    new InMemoryRecipeRepository(recipes));

builder.Services.AddScoped<IQueryInterpretationService, AzureOpenAiQueryInterpretationService>();
builder.Services.AddScoped<IRecipeRankingService, RecipeRankingService>();
builder.Services.AddScoped<IRecipeSearchService, RecipeSearchService>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();

app.MapControllers();
app.Run();