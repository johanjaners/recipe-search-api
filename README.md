# Recipe Search API

Simple ASP.NET Core Web API for searching recipes using ingredients and free text queries.

Current release: `v0.1.0`  
Status: `MVP / Pre release`

---

## Project overview

This project provides a simple backend API for searching recipes from a JSON dataset.

The API supports:

- ingredient based search
- free text search
- mixed search
- request validation
- deterministic ranking
- Swagger UI testing

The goal of this MVP is to demonstrate clear backend architecture, service boundaries, validation, and explainable search logic.

---

## Architecture

Project structure:

```text
src/
 ├── RecipeSearch.Api
 ├── RecipeSearch.Application
 ├── RecipeSearch.Domain
 └── RecipeSearch.Infrastructure

tests/
 └── RecipeSearch.Tests
```

### Layers

### API
Handles:

- controllers
- request / response DTOs
- validation
- Swagger

### Application
Handles:

- search orchestration
- ranking logic
- query interpretation
- service interfaces

### Domain
Contains:

- Recipe entity
- search models
- business rules

### Infrastructure
Handles:

- JSON recipe loading
- repository implementation
- data normalization

---

## Search flow

Search endpoint:

```http
POST /api/Recipe/search
```

Request fields:

* `ingredients`: list of ingredients
* `query`: free text cooking intent
* `language`: ISO 639-1 language code (for example: `"en"`, `"sv"`, `"es"`)
* `top`: number of results to return

Flow:

```text
Request
→ validation
→ normalize ingredients and query
→ repository search
→ ranking
→ top N results
→ response
```

Search supports:

### 1. Ingredient search

Example:

```json
{
  "ingredients": ["chicken", "rice"],
  "query": "",
  "language": "en",
  "top": 5
}
```

### 2. Free text search

Example:

```json
{
  "ingredients": [],
  "query": "spicy chicken dinner",
  "language": "en",
  "top": 5
}
```

### 3. Mixed search

Example:

```json
{
  "ingredients": ["chicken"],
  "query": "spicy dinner",
  "language": "en",
  "top": 5
}
```

---

## Ranking logic

The current MVP uses deterministic ranking.

Results are ranked based on:

- ingredient exact matches
- recipe name keyword matches
- ingredient text keyword matches
- basic text normalization

This keeps search behavior predictable and easy to explain.

Example scoring logic:

```text
ingredient matches = high weight
name matches = medium weight
query keyword matches = medium weight
```

Top results are returned using the `top` parameter.

Validation:

```text
top must be between 1 and 50
```

---

## How to run

### Dataset

Place the provided dataset file in the `data/` folder before starting the API.

Expected path:

```text
data/20170107-061401-recipeitems.json
```

### Run locally

```bash
dotnet restore
dotnet build
dotnet run --project src/RecipeSearch.Api
```

Swagger UI:

```text
http://localhost:5064/swagger
```

---

## Swagger test examples

### Ingredient search

```json
{
  "ingredients": ["chicken", "rice"],
  "query": "",
  "language": "en",
  "top": 5
}
```

### Free text search

```json
{
  "ingredients": [],
  "query": "spicy chicken dinner",
  "language": "en",
  "top": 5
}
```

### Validation failure example

```json
{
  "ingredients": [],
  "query": "",
  "language": "en",
  "top": 5
}
```

Expected response:

```text
400 Bad Request
At least one ingredient or a query must be provided.
```

---

## Unit Tests

Unit tests cover the core application flow and deterministic search behavior.

This includes:

- JSON recipe loading and parsing from the source dataset
- search service orchestration between query interpretation, repository, and ranking
- ranking logic, result ordering, filtering, and top result limits

The test suite focuses on validating the most critical backend behavior.

---

## Current limitations

Current MVP limitations:

- in memory dataset only
- no database
- no semantic search
- no synonym dictionary
- Swedish support is currently basic token matching
- no AI query interpretation yet
- ranking is rule based only

This is intentional for MVP simplicity and explainability.

---

## Future improvements

Planned improvements:

- AI based query interpretation
- multilingual normalization
- synonym support
- semantic / vector search
- database persistence
- caching
- improved ranking weights
- more test coverage
- deployment pipeline

---

## Purpose

This project is built as a backend engineering demo to showcase:

- ASP.NET Core Web API
- clean service architecture
- validation and error handling
- deterministic backend logic
- clear API contracts
- production minded engineering decisions
