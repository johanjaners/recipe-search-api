using RecipeSearch.Api;
using RecipeSearch.Api.Extensions;
using RecipeSearch.Application.Interfaces;
using RecipeSearch.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddUserSecrets<Program>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAzureOpenAI(builder.Configuration);
builder.Services.AddApplicationServices();

var recipes = await RecipeDataLoader.LoadAsync(builder.Configuration, builder.Environment);
builder.Services.AddSingleton<IRecipeRepository>(new InMemoryRecipeRepository(recipes));

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();
app.MapControllers();
app.Run();