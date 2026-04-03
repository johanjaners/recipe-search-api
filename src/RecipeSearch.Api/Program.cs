using RecipeSearch.Application.Interfaces;
using RecipeSearch.Application.Services;
using RecipeSearch.Infrastructure.Data;
using RecipeSearch.Infrastructure.AI;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddUserSecrets<Program>();

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

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var loader = new JsonRecipeLoader();
var dataPath = Path.Combine(builder.Environment.ContentRootPath, "..", "..", "data", "20170107-061401-recipeitems.json");
var fullDataPath = Path.GetFullPath(dataPath);
var recipes = await loader.LoadAsync(fullDataPath);

builder.Services.AddSingleton<IRecipeRepository>(
    new InMemoryRecipeRepository(recipes));

builder.Services.AddScoped<IQueryInterpretationService, AzureOpenAiQueryInterpretationService>();
builder.Services.AddScoped<IRecipeRankingService, RecipeRankingService>();
builder.Services.AddScoped<IRecipeSearchService, RecipeSearchService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();
