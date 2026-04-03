using RecipeSearch.Infrastructure.Data;

namespace RecipeSearch.Tests.Data;

public class JsonRecipeLoaderTests
{
    [Fact]
    public async Task LoadAsync_ShouldReturnRecipesFromLineDelimitedJsonFile()
    {
        var json = """
        { "_id": { "$oid": "recipe-1" }, "name": "Coconut fish curry", "ingredients": "fish\ncoconut milk\nchili", "url": "http://example.com/recipe-1", "cookTime": "PT30M", "prepTime": "PT15M", "source": "bbcfood", "recipeYield": "Serves 2" }
        """;

        var filePath = Path.GetTempFileName();

        try
        {
            await File.WriteAllTextAsync(filePath, json);

            var loader = new JsonRecipeLoader();

            var recipes = await loader.LoadAsync(filePath);

            var recipe = Assert.Single(recipes);
            Assert.Equal("recipe-1", recipe.Id);
            Assert.Equal("Coconut fish curry", recipe.Name);
            Assert.Equal("http://example.com/recipe-1", recipe.Url);
            Assert.Equal("PT30M", recipe.CookTime);
            Assert.Equal("PT15M", recipe.PrepTime);
            Assert.Equal("bbcfood", recipe.Source);
            Assert.Equal("Serves 2", recipe.RecipeYield);
        }
        finally
        {
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }
    }

    [Fact]
    public async Task LoadAsync_ShouldSplitIngredientsIntoList()
    {
        var json = """
        { "_id": { "$oid": "recipe-1" }, "name": "Coconut fish curry", "ingredients": "fish\ncoconut milk\nchili", "url": "http://example.com/recipe-1", "cookTime": "PT30M", "prepTime": "PT15M", "source": "bbcfood", "recipeYield": "Serves 2" }
        """;

        var filePath = Path.GetTempFileName();

        try
        {
            await File.WriteAllTextAsync(filePath, json);

            var loader = new JsonRecipeLoader();

            var recipes = await loader.LoadAsync(filePath);

            var recipe = Assert.Single(recipes);
            Assert.Equal(3, recipe.Ingredients.Count);
            Assert.Contains("fish", recipe.Ingredients);
            Assert.Contains("coconut milk", recipe.Ingredients);
            Assert.Contains("chili", recipe.Ingredients);
        }
        finally
        {
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }
    }

    [Fact]
    public async Task LoadAsync_ShouldReturnMultipleRecipes_WhenFileContainsMultipleLines()
    {
        var json = """
        { "_id": { "$oid": "recipe-1" }, "name": "Coconut fish curry", "ingredients": "fish\ncoconut milk\nchili", "url": "http://example.com/recipe-1", "cookTime": "PT30M", "prepTime": "PT15M", "source": "bbcfood", "recipeYield": "Serves 2" }
        { "_id": { "$oid": "recipe-2" }, "name": "Berry cake", "ingredients": "berries\nflour\nsugar", "url": "http://example.com/recipe-2", "cookTime": "PT40M", "prepTime": "PT20M", "source": "example", "recipeYield": "Serves 4" }
        """;

        var filePath = Path.GetTempFileName();

        try
        {
            await File.WriteAllTextAsync(filePath, json);

            var loader = new JsonRecipeLoader();

            var recipes = await loader.LoadAsync(filePath);

            Assert.Equal(2, recipes.Count);
            Assert.Equal("recipe-1", recipes[0].Id);
            Assert.Equal("recipe-2", recipes[1].Id);
        }
        finally
        {
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }
    }

    [Fact]
    public async Task LoadAsync_ShouldReturnEmptyList_WhenFileIsEmpty()
    {
        var filePath = Path.GetTempFileName();

        try
        {
            await File.WriteAllTextAsync(filePath, string.Empty);

            var loader = new JsonRecipeLoader();

            var recipes = await loader.LoadAsync(filePath);

            Assert.Empty(recipes);
        }
        finally
        {
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }
    }
}