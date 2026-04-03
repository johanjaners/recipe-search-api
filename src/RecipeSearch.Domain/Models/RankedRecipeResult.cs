namespace RecipeSearch.Domain.Models;

public class RankedRecipeResult
{
    public Recipe Recipe { get; init; } = new();
    public int Score { get; init; }
}