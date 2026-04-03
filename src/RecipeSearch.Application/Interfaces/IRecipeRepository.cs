using RecipeSearch.Domain.Models;

namespace RecipeSearch.Application.Interfaces;

public interface IRecipeRepository
{
    Task<IReadOnlyList<Recipe>> GetAllAsync(
        CancellationToken cancellationToken = default);
}