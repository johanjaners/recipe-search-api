using Microsoft.EntityFrameworkCore;
using RecipeSearch.Application.Interfaces;
using RecipeSearch.Domain.Models;

namespace RecipeSearch.Api.Data;

public class EfRecipeRepository(RecipeDbContext dbContext) : IRecipeRepository
{
    public async Task<IReadOnlyList<Recipe>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        return await dbContext.Recipes
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }
}
