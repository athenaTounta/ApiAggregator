# ApiAggregator

A .NET 8 API aggregation service that fetches data from multiple external APIs simultaneously and returns a unified response.

## Architecture

- **API** — Controllers, middleware, global exception handling
- **Application** — Business logic, interfaces, decorators
- **Infrastructure** — External API clients, caching, resilience
- **Domain** — Models, DTOs, enums

## External APIs used

-  Weather | OpenWeatherMap 
-  News | NewsAPI | Top headlines |
-  GitHub | GitHub REST API | Trending .NET repositories |

## Features

- **Parallel API calls** — All three APIs are called simultaneously using `Task.WhenAll`
- **Error handling** — FluentResults error handling, no exceptions for expected failures
- **Fallback mechanism** — If some APIs fail, partial results are returned. If all APIs fail, stale cached data is returned
- **Resilience** — Microsoft.Extensions.Http.Resilience with default policy
- **Caching** — In-memory caching with decorator.
- **Filtering** — Filter by category (Weather, News, GitHub)
- **Sorting** — Sort by date or title, ascending or descending

## Endpoints

### GET /api/aggregation

Returns aggregated data from all external APIs.

**Query Parameters**
category 
sortBy
order 

**Example Requests**
```
GET /api/aggregation
GET /api/aggregation?category=github
GET /api/aggregation?category=news&sortBy=title&order=Asc
```

### GET /api/aggregation/statistics

*Coming soon*

## Setup

### Prerequisites

- .NET 8 SDK
- API keys for OpenWeatherMap and NewsAPI (GitHub does not require a key)

### Configuration

Add the following to `appsettings.Development.json`:
```json
{
  "ExternalApis": {
    "OpenWeather": {
      "ApiKey": "your_openweather_api_key",
      "BaseUrl": "https://api.openweathermap.org/data/2.5",
      "City": "Athens"
    },
    "News": {
      "ApiKey": "your_newsapi_key",
      "BaseUrl": "https://newsapi.org/v2"
    },
    "GitHub": {
      "BaseUrl": "https://api.github.com"
    }
  }
}
```

Navigate to `https://localhost:7015/swagger` to explore the API.

## Resilience

`AddStandardResilienceHandler`

## Caching

Responses are cached in memory using the decorator pattern

- Fresh cache — 5 minutes
- Stale cache (fallback) — 30 minutes

## Error Handling

- Expected failures (API down, rate limited, auth failed) → handled in each service, returned as partial results
- Unexpected failures → caught by `GlobalExceptionHandler`

## Tech Stack

- .NET 8 / ASP.NET Core
- FluentResults
- Microsoft.Extensions.Http.Resilience 
- Scrutor (decorator registration)
- xUnit / Moq / FluentAssertions