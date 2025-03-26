# Feature Management Platform (FMP)

A cross-platform, extensible Feature Flag Management Platform built with .NET Core. The platform provides a comprehensive API for managing feature flags in your application.

## Features

- RESTful API for managing feature flags
- In-memory data store for development with sample feature flags
- Extensible architecture to swap in different data stores for production use
- Support for environment-specific flag configurations (Development, Staging, Production)
- User/group targeting capabilities
- Multi-platform support via .NET Core
- Docker support for containerized deployment
- Comprehensive test suite with unit and integration tests

## Getting Started

### Prerequisites

- [.NET 6.0 SDK](https://dotnet.microsoft.com/download/dotnet/6.0) or later
- [Docker](https://www.docker.com/products/docker-desktop) (optional, for container deployment)

### Running Locally

1. Clone the repository
2. Navigate to the solution directory
3. Build the solution:

```bash
dotnet build
```

4. Run the API:

```bash
cd src/FMP.Api
dotnet run
```

5. Access the Swagger UI at `https://localhost:5001/swagger` or `http://localhost:5000/swagger`

### Running with Docker

1. Build the Docker image:

```bash
docker build -t fmp-api .
```

2. Run the container:

```bash
docker run -p 5000:80 -p 5001:443 fmp-api
```

3. Or using Docker Compose:

```bash
docker-compose up
```

## Project Structure

- `src/FMP.Api` - Web API project with controllers and API endpoints
- `src/FMP.Core` - Core business logic, models, and interfaces
- `tests/FMP.Tests` - Unit and integration tests

## API Endpoints

The API provides the following endpoints:

- `GET /api/featureflags` - Get all feature flags
- `GET /api/featureflags/{id}` - Get a feature flag by ID
- `GET /api/featureflags/key/{key}` - Get a feature flag by key
- `GET /api/featureflags/status/{key}` - Check if a feature flag is enabled for a specific environment
- `POST /api/featureflags` - Create a new feature flag
- `PUT /api/featureflags/{id}` - Update an existing feature flag
- `DELETE /api/featureflags/{id}` - Delete a feature flag
- `GET /api/featureflags/tags?tags=tag1,tag2` - Get feature flags with specific tags

## Architecture

The FMP follows a clean architecture with separation of concerns:

- **Models** - Domain entities representing feature flags, environments, and targeting rules
- **Repositories** - Data access abstraction with in-memory implementation for development
- **Services** - Business logic for managing feature flags
- **Controllers** - API endpoints exposing feature flag management capabilities

## Extending for Production

To extend the platform for production use:

1. Implement a new repository (e.g., SQL, MongoDB, Redis) by implementing the `IFeatureFlagRepository` interface
2. Update the dependency injection in `Program.cs` to use your new repository implementation

## Running Tests

To run the tests:

```bash
dotnet test
```

## License

MIT