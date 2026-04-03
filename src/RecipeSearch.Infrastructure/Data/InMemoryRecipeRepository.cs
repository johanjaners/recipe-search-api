using RecipeSearch.Application.Interfaces;
using RecipeSearch.Domain.Models;

namespace RecipeSearch.Infrastructure.Data;

public class InMemoryRecipeRepository : IRecipeRepository
{
    private readonly IReadOnlyList<Recipe> _recipes;

    public InMemoryRecipeRepository(IReadOnlyList<Recipe> recipes)
    {
        _recipes = recipes;
    }

    public Task<IReadOnlyList<Recipe>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_recipes);
    }
}