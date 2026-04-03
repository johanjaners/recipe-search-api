using Moq;
using RecipeSearch.Application.Interfaces;
using RecipeSearch.Application.Services;
using RecipeSearch.Domain.Models;

namespace RecipeSearch.Tests.Services;

public class RecipeSearchServiceTests
{
    [Fact]
    public async Task SearchAsync_ShouldInterpretQuery_GetRecipes_AndReturnRankedResults()
    {
        var inputQuery = new RecipeSearchQuery
        {
            Ingredients = new List<string> { "fish" },
            OriginalQuery = "spicy fish",
            Language = "en",
            Top = 10
        };

        var interpretedQuery = new InterpretedQuery
        {
            Ingredients = new List<string> { "fish" },
            Keywords = new List<string> { "spicy" },
            TranslatedQuery = "spicy fish",
            DetectedLanguage = "en"
        };

        var recipes = new List<Recipe>
        {
            new()
            {
                Id = "1",
                Name = "Fish Curry",
                IngredientsRaw = "fish\nchili",
                Ingredients = new List<string> { "fish", "chili" }
            }
        };

        var rankedResults = new List<RankedRecipeResult>
        {
            new()
            {
                Recipe = recipes[0],
                Score = 7
            }
        };

        var interpretationServiceMock = new Mock<IQueryInterpretationService>();
        interpretationServiceMock
            .Setup(x => x.InterpretAsync(inputQuery, It.IsAny<CancellationToken>()))
            .ReturnsAsync(interpretedQuery);

        var repositoryMock = new Mock<IRecipeRepository>();
        repositoryMock
            .Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(recipes);

        var rankingServiceMock = new Mock<IRecipeRankingService>();
        rankingServiceMock
            .Setup(x => x.Rank(recipes, interpretedQuery, inputQuery.Top))
            .Returns(rankedResults);

        var service = new RecipeSearchService(
            repositoryMock.Object,
            interpretationServiceMock.Object,
            rankingServiceMock.Object);

        var (query, results) = await service.SearchAsync(inputQuery);

        Assert.Equal(interpretedQuery, query);
        Assert.Single(results);
        Assert.Equal("1", results[0].Recipe.Id);
        Assert.Equal(7, results[0].Score);

        interpretationServiceMock.Verify(
            x => x.InterpretAsync(inputQuery, It.IsAny<CancellationToken>()),
            Times.Once);

        repositoryMock.Verify(
            x => x.GetAllAsync(It.IsAny<CancellationToken>()),
            Times.Once);

        rankingServiceMock.Verify(
            x => x.Rank(recipes, interpretedQuery, inputQuery.Top),
            Times.Once);
    }

    [Fact]
    public async Task SearchAsync_ShouldPassTopValueToRankingService()
    {
        var inputQuery = new RecipeSearchQuery
        {
            Ingredients = new List<string> { "fish" },
            OriginalQuery = "fish curry",
            Language = "en",
            Top = 3
        };

        var interpretedQuery = new InterpretedQuery
        {
            Ingredients = new List<string> { "fish" },
            Keywords = new List<string> { "curry" },
            TranslatedQuery = "fish curry",
            DetectedLanguage = "en"
        };

        var recipes = new List<Recipe>
        {
            new() { Id = "1", Name = "Fish Curry", Ingredients = new List<string> { "fish" } }
        };

        var rankedResults = new List<RankedRecipeResult>();

        var interpretationServiceMock = new Mock<IQueryInterpretationService>();
        interpretationServiceMock
            .Setup(x => x.InterpretAsync(inputQuery, It.IsAny<CancellationToken>()))
            .ReturnsAsync(interpretedQuery);

        var repositoryMock = new Mock<IRecipeRepository>();
        repositoryMock
            .Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(recipes);

        var rankingServiceMock = new Mock<IRecipeRankingService>();
        rankingServiceMock
            .Setup(x => x.Rank(recipes, interpretedQuery, 3))
            .Returns(rankedResults);

        var service = new RecipeSearchService(
            repositoryMock.Object,
            interpretationServiceMock.Object,
            rankingServiceMock.Object);

        await service.SearchAsync(inputQuery);

        rankingServiceMock.Verify(
            x => x.Rank(recipes, interpretedQuery, 3),
            Times.Once);
    }
}