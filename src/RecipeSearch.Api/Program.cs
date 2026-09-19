using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using RecipeSearch.Api;
using RecipeSearch.Api.Data;
using RecipeSearch.Api.Extensions;
using RecipeSearch.Application.Interfaces;
using RecipeSearch.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddUserSecrets<Program>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
if (string.IsNullOrWhiteSpace(connectionString))
{
    throw new InvalidOperationException("Missing ConnectionStrings:DefaultConnection");
}

builder.Services.AddDbContext<RecipeDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddRateLimiter(options =>
{
    options.AddFixedWindowLimiter("api", limiterOptions =>
    {
        limiterOptions.PermitLimit = 5;
        limiterOptions.Window = TimeSpan.FromMinutes(1);
        limiterOptions.QueueLimit = 0;
        limiterOptions.AutoReplenishment = true;
    });

    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
});

builder.Services.AddAzureOpenAI(builder.Configuration);
builder.Services.AddApplicationServices();
builder.Services.AddScoped<IRecipeRepository, EfRecipeRepository>();

var app = builder.Build();

await using (var scope = app.Services.CreateAsyncScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<RecipeDbContext>();
    await dbContext.Database.MigrateAsync();

    if (!await dbContext.Recipes.AnyAsync())
    {
        var recipes = await RecipeDataLoader.LoadAsync(builder.Configuration, builder.Environment);
        await dbContext.Recipes.AddRangeAsync(recipes);
        await dbContext.SaveChangesAsync();
    }
}

app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();
app.UseRateLimiter();
app.MapControllers().RequireRateLimiting("api");
app.Run();