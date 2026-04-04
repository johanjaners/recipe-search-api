using Microsoft.AspNetCore.RateLimiting;
using RecipeSearch.Api;
using RecipeSearch.Api.Extensions;
using RecipeSearch.Application.Interfaces;
using RecipeSearch.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddUserSecrets<Program>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddRateLimiter(options =>
{
    options.AddFixedWindowLimiter("api", limiterOptions =>
    {
        limiterOptions.PermitLimit = 20;
        limiterOptions.Window = TimeSpan.FromMinutes(1);
        limiterOptions.QueueLimit = 0;
        limiterOptions.AutoReplenishment = true;
    });

    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
});

builder.Services.AddAzureOpenAI(builder.Configuration);
builder.Services.AddApplicationServices();

var recipes = await RecipeDataLoader.LoadAsync(builder.Configuration, builder.Environment);
builder.Services.AddSingleton<IRecipeRepository>(new InMemoryRecipeRepository(recipes));

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();
app.UseRateLimiter();
app.MapControllers().RequireRateLimiting("api");
app.Run();