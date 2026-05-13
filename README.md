# UEFA Ranking Application
### Countries and Teams with 2025 -2026 statistics
#### Europeans football cups: Champions League, Europa and Conference League

FootballRankings tracks rankings based on European UEFA club matches across the 3 main competitions. This project is for non-commercial use only!

## Features
- UEFA country rankings
- Team statistics for 2025-2026
- Champions League / Europa League / Conference League tracking
- REST API architecture
- Layered clean architecture approach

## Architecture
This .NET project follows a layered architecture pattern:

- **Core**
  - Domain Models Contracts
  - Repositories Interfaces

- **Infrastructure**
  - Database Access Repositories
  - Query Parameters for API
  - Helpder and External Services

- **API**
  - REST API endpoints with Swagger format
  - ASP.NET Core MVC Controllers
  - Dependency Injection Configuration / Middleware

- **UI**
  - Frontend Blazor Application with Razor Pages
  - Clients controllers
  - DTOs models

- **Tests**
  - Backend and API tests
  - Unit and Integration types
 
 ## Project Structure
```text
API/
Core/
Infrastructure/
Tests/
UI/
