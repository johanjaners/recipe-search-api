using RecipeSearch.Application.Interfaces;
using RecipeSearch.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);

var loader = new JsonRecipeLoader();
var dataPath = Path.Combine(builder.Environment.ContentRootPath, "data", "20170107-061401-recipeitems.json");
var recipes = await loader.LoadAsync(dataPath);

builder.Services.AddSingleton<IRecipeRepository>(
    new InMemoryRecipeRepository(recipes));

builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.Run();
