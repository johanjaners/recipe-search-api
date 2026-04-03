using RecipeSearch.Application.Interfaces;
using RecipeSearch.Application.Services;
using RecipeSearch.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var loader = new JsonRecipeLoader();
var dataPath = Path.Combine(builder.Environment.ContentRootPath, "..", "..", "data", "20170107-061401-recipeitems.json");
var fullDataPath = Path.GetFullPath(dataPath);
var recipes = await loader.LoadAsync(fullDataPath);

builder.Services.AddSingleton<IRecipeRepository>(
    new InMemoryRecipeRepository(recipes));

builder.Services.AddScoped<IQueryInterpretationService, SimpleQueryInterpretationService>();
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
