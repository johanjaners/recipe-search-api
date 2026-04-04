

using Microsoft.AspNetCore.Mvc;
using RecipeSearch.Api.Contracts.Requests;
using RecipeSearch.Api.Contracts.Responses;
using RecipeSearch.Application.Interfaces;
using RecipeSearch.Domain.Models;

namespace RecipeSearch.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RecipeController(IRecipeSearchService recipeSearchService) : ControllerBase
{
    [HttpPost("search")]
    [ProducesResponseType(typeof(RecipeSearchResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<RecipeSearchResponse>> Search(
        [FromBody] RecipeSearchRequest request,
        CancellationToken cancellationToken)
    {
        if ((request.Ingredients == null || request.Ingredients.Count == 0) &&
            string.IsNullOrWhiteSpace(request.Query))
        {
            return BadRequest("At least one ingredient or a query must be provided.");
        }

        if (request.Top <= 0 || request.Top > 50)
        {
            return BadRequest("Top must be between 1 and 50.");
        }

        var searchQuery = new RecipeSearchQuery
        {
            Ingredients = (request.Ingredients ?? [])
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Select(x => x.Trim())
                .ToList(),
            OriginalQuery = request.Query?.Trim() ?? string.Empty,
            Language = string.IsNullOrWhiteSpace(request.Language) ? "en" : request.Language.Trim(),
            Top = request.Top
        };

        var (interpretedQuery, results) = await recipeSearchService.SearchAsync(searchQuery, cancellationToken);

        var response = new RecipeSearchResponse
        {
            NormalizedIngredients = interpretedQuery.Ingredients,
            NormalizedKeywords = interpretedQuery.Keywords,
            Results = results.Select(result => new RecipeDto
            {
                Id = result.Recipe.Id,
                Name = result.Recipe.Name,
                Ingredients = result.Recipe.Ingredients,
                Url = result.Recipe.Url,
                PrepTime = result.Recipe.PrepTime,
                CookTime = result.Recipe.CookTime,
                Source = result.Recipe.Source,
                Score = result.Score
            }).ToList()
        };

        return Ok(response);
    }

}