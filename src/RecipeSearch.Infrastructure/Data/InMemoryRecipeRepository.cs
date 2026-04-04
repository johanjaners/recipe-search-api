using RecipeSearch.Application.Interfaces;
using RecipeSearch.Domain.Models;

namespace RecipeSearch.Infrastructure.Data;

public class InMemoryRecipeRepository(IReadOnlyList<Recipe> recipes) : IRecipeRepository
{
    public Task<IReadOnlyList<Recipe>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult(recipes);
    }
}