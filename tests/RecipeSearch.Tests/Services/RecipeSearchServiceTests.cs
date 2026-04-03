using RecipeSearch.Application.Services;
using RecipeSearch.Domain.Models;

namespace RecipeSearch.Tests.Services;

public class RecipeRankingServiceTests
{
    [Fact]
    public void Rank_ShouldReturnBestMatchFirst()
    {
        var recipes = new List<Recipe>
        {
            new()
            {
                Id = "1",
                Name = "Fish Curry",
                IngredientsRaw = "fish\ncoconut milk\nchili",
                Ingredients = new List<string> { "fish", "coconut milk", "chili" },
                Url = "http://test1.com",
                PrepTime = "PT10M",
                CookTime = "PT20M",
                Source = "test"
            },
            new()
            {
                Id = "2",
                Name = "Fish Salad",
                IngredientsRaw = "fish\nlettuce",
                Ingredients = new List<string> { "fish", "lettuce" },
                Url = "http://test2.com",
                PrepTime = "PT5M",
                CookTime = "PT0M",
                Source = "test"
            }
        };

        var query = new InterpretedQuery
        {
            Ingredients = new List<string> { "fish", "coconut milk" },
            Keywords = new List<string> { "curry" },
            TranslatedQuery = "fish curry with coconut milk",
            DetectedLanguage = "en"
        };

        var service = new RecipeRankingService();

        var results = service.Rank(recipes, query, 10);

        Assert.Equal(2, results.Count);
        Assert.Equal("1", results[0].Recipe.Id);
        Assert.Equal("Fish Curry", results[0].Recipe.Name);
        Assert.True(results[0].Score > results[1].Score);
    }

    [Fact]
    public void Rank_ShouldFilterOutRecipesWithZeroScore()
    {
        var recipes = new List<Recipe>
        {
            new()
            {
                Id = "1",
                Name = "Beef Stew",
                IngredientsRaw = "beef\npotato",
                Ingredients = new List<string> { "beef", "potato" },
                Url = "http://test1.com",
                PrepTime = "PT10M",
                CookTime = "PT20M",
                Source = "test"
            }
        };

        var query = new InterpretedQuery
        {
            Ingredients = new List<string> { "fish" },
            Keywords = new List<string> { "curry" },
            TranslatedQuery = "fish curry",
            DetectedLanguage = "en"
        };

        var service = new RecipeRankingService();

        var results = service.Rank(recipes, query, 10);

        Assert.Empty(results);
    }

    [Fact]
    public void Rank_ShouldRespectTopLimit()
    {
        var recipes = new List<Recipe>
        {
            new()
            {
                Id = "1",
                Name = "Fish Curry",
                IngredientsRaw = "fish\ncoconut milk",
                Ingredients = new List<string> { "fish", "coconut milk" }
            },
            new()
            {
                Id = "2",
                Name = "Fish Soup",
                IngredientsRaw = "fish\nwater",
                Ingredients = new List<string> { "fish", "water" }
            },
            new()
            {
                Id = "3",
                Name = "Fish Rice",
                IngredientsRaw = "fish\nrice",
                Ingredients = new List<string> { "fish", "rice" }
            }
        };

        var query = new InterpretedQuery
        {
            Ingredients = new List<string> { "fish" },
            Keywords = new List<string>()
        };

        var service = new RecipeRankingService();

        var results = service.Rank(recipes, query, 2);

        Assert.Equal(2, results.Count);
    }
}