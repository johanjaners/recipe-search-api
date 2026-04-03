# Recipe Search API

ASP.NET Core Web API for recipe search using ingredients and natural language queries.

The API supports deterministic recipe ranking and Azure OpenAI based multilingual query interpretation.

---

## Project Overview

This project provides a backend API for searching recipes from a JSON dataset.

Supported search modes:

- ingredient based search
- free text search
- mixed search
- multilingual query interpretation
- deterministic ranking
- request validation
- Swagger UI testing

The system separates AI based query understanding from deterministic retrieval and ranking.

---

## Architecture

```text
src/
 ├── RecipeSearch.Api
 ├── RecipeSearch.Application
 ├── RecipeSearch.Domain
 └── RecipeSearch.Infrastructure

tests/
 └── RecipeSearch.Tests
```

### API

Responsible for:

- controllers
- request and response DTOs
- validation
- Swagger configuration
- dependency injection

### Application

Responsible for:

- search orchestration
- ranking logic
- service contracts
- query interpretation abstraction

### Domain

Contains:

- Recipe
- RecipeSearchQuery
- InterpretedQuery
- RankedRecipeResult

### Infrastructure

Responsible for:

- JSON dataset loading
- in memory repository
- Azure OpenAI query interpretation
- external service integration

---

## Search Flow

```text
Request
→ validation
→ AI query interpretation
→ recipe retrieval
→ deterministic ranking
→ top N results
→ response
```

Endpoint:

```http
POST /api/Recipe/search
```

Request fields:

- `ingredients`
- `query`
- `language`
- `top`

---

## Ranking and Scoring

Recipe search uses deterministic scoring.

Each searched ingredient is scored by match strength:

- `+6` exact ingredient phrase match
- `+4` strong phrase or full token match within one ingredient line
- `+1` weak partial token match

Keywords are scored as:

- `+3` keyword match in recipe name
- `+2` keyword match in ingredients text

An additional `+2` bonus is added if a searched ingredient is present in the recipe name.

The final score is the sum of all matched rules.

Results with score `0` are excluded.

Results are returned in descending score order.

---

## AI Usage

AI is used only for query interpretation.

This includes:

- multilingual input normalization
- translation to English search terms
- ingredient extraction from free text
- keyword extraction from cooking intent

The AI output is converted into a structured query model containing:

- normalized ingredients
- normalized keywords
- translated query
- detected language

Recipe retrieval and ranking remain deterministic.

---

## Unit Tests

Unit tests cover the core backend behavior.

Covered components:

- `JsonRecipeLoader`
- `RecipeSearchService`
- `RecipeRankingService`

Tests verify:

- dataset parsing
- field mapping
- search orchestration
- dependency interaction
- ranking order
- zero score filtering
- top limit handling
- stronger match prioritization

---

## How to Run

Place the dataset file in:

```text
data/20170107-061401-recipeitems.json
```

Run locally:

```bash
dotnet restore
dotnet build
dotnet run --project src/RecipeSearch.Api
```

Swagger:

```text
http://localhost:5064/swagger
```

---

## Azure OpenAI Configuration

Configure secrets locally:

```bash
dotnet user-secrets set "AzureOpenAI:Endpoint" "<endpoint>"
dotnet user-secrets set "AzureOpenAI:ApiKey" "<api-key>"
dotnet user-secrets set "AzureOpenAI:DeploymentName" "<deployment-name>"
```

---

## Example Request

```json
{
    "ingredients": [],
    "query": "Jag vill laga något starkt med fisk och kokosmjölk",
    "language": "sv",
    "top": 5
}
```

Expected normalized interpretation:

```json
{
    "normalizedIngredients": ["fish", "coconut milk"],
    "normalizedKeywords": ["spicy"]
}
```

---

## Current Limitations

- in memory dataset
- no persistent storage
- no semantic vector search
- no synonym dictionary
- rule based ranking only

These tradeoffs are intentional to keep the system explainable and production minded for the scope of the assignment.
