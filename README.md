# Feature Management Platform (FMP)

A platform for managing feature flags across applications and environments with a user-friendly UI and robust API.

## Overview

The Feature Management Platform consists of three main components:

1. **API** - A .NET Core RESTful API for feature flag management
2. **UI** - A React-based frontend for feature flag administration
3. **DataStore** - A flexible data storage layer with in-memory and Cosmos DB implementations

## Project Structure

```
fmp_prototype_3/
├── api-new/            # .NET Core API project
├── datastore/          # Data storage implementation library
├── models/             # Shared data models
└── ui/                 # React-based frontend application
```

## Components

### Models (models/)

The shared domain model library containing the feature flag data structures and related models.

- **FeatureFlag** - Core entity representing a feature flag with:
  - Basic properties (id, key, name, description, enabled)
  - Targeting rules for specific user segments
  - Environment-specific configurations

### Data Store (datastore/)

A flexible data access library implementing the repository pattern that provides:

- **Repository Interface** - Defines the data contract for feature flag operations
- **In-Memory Implementation** - For local development with sample data
- **Cosmos DB Implementation** - For production use with:
  - Automatic database/container initialization
  - Resilient connection handling
  - Graceful error recovery

### API (api-new/)

A RESTful API providing endpoints for feature flag management:

- **GET /api/featureflags** - Retrieve all feature flags
- **GET /api/featureflags/{key}** - Get a specific feature flag
- **POST /api/featureflags** - Create a new feature flag
- **PUT /api/featureflags/{key}** - Update an existing feature flag
- **DELETE /api/featureflags/{key}** - Delete a feature flag

### UI (ui/)

A React-based frontend for feature flag management providing:

- Feature flag listing and administration
- Flag creation and editing
- Toggle controls for enabling/disabling features
- Environment-specific configuration management

## Getting Started

### Prerequisites

- .NET 9.0 SDK
- Node.js and npm (for UI development)
- Cosmos DB (production) or Cosmos DB Emulator (optional for development)

### Running the API

1. Build the solution:
   ```
   dotnet build fmp_prototype_3.sln
   ```

2. Run the API:
   ```
   cd api-new
   dotnet run
   ```

   The API will run at `https://localhost:5001` or `http://localhost:5000`

### Running the UI

1. Install dependencies:
   ```
   cd ui
   npm install
   ```

2. Start the development server:
   ```
   npm start
   ```

   The UI will run at `http://localhost:3000`

## Configuration

### API Configuration

The API supports different environment configurations in `appsettings.json` and `appsettings.Development.json`:

- **FeatureManagement:UseInMemoryRepository** - Set to `true` to use in-memory storage, `false` for Cosmos DB
- **CosmosDb:Endpoint** - The Cosmos DB endpoint URL
- **CosmosDb:Key** - The Cosmos DB access key
- **CosmosDb:DatabaseName** - The database name to use
- **CosmosDb:ContainerName** - The container name for feature flags

### Development Configuration

For local development, you can:

1. Use the in-memory repository (no additional setup)
2. Use the Cosmos DB Emulator (requires Docker or local installation)

## Architecture

The solution follows a clean architecture approach:

- **Domain Models** - Shared across the solution
- **Repository Pattern** - For data access abstraction
- **Dependency Injection** - For loose coupling of components
- **REST API** - Following standard RESTful practices
- **React Frontend** - Modern component-based UI

## Resilience Features

The platform includes resilience features:

- Graceful fallbacks when Cosmos DB is unavailable
- Automatic retry mechanisms
- Timeout management
- Error logging

## Cross-Origin Support

The API includes CORS configuration to allow cross-origin requests from the UI, making it easy to run the UI and API on different ports during development.

## Development Approach

This project was developed using an AI-assisted approach, leveraging GitHub Copilot to generate the majority of the codebase:

### AI-Assisted Development

- **Code Generation**: GitHub Copilot was used to generate almost all of the code for this project, including:
  - The full data store implementation with Cosmos DB integration
  - API controllers and routing
  - Repository pattern implementation
  - Configuration handling

- **Architecture Design**: The clean, modular architecture was designed through interactive sessions with GitHub Copilot, focusing on:
  - Separation of concerns
  - Proper abstraction layers
  - Resilient implementation patterns

- **Iterative Process**: Development followed an iterative approach where:
  1. Requirements were discussed with the AI
  2. Initial implementation was generated
  3. Issues were identified and resolved through further AI assistance
  4. Refinements were made based on testing feedback

### Benefits of AI-Assisted Development

- **Rapid Prototyping**: The initial implementation was completed rapidly
- **Best Practices**: The AI incorporated industry best practices for repository pattern, error handling, etc.
- **Knowledge Integration**: The implementation benefited from the AI's knowledge of Cosmos DB, ASP.NET Core, and modern architecture patterns

### Human Oversight

While GitHub Copilot generated most of the code, human oversight was maintained throughout the process to ensure:
- The implementation met business requirements
- The architecture remained clean and maintainable
- Security and performance considerations were properly addressed

## Testing

### Running API Tests

The API project includes unit tests to verify the functionality of repositories, controllers, and services. To run the tests:

```bash
# Run all tests in the solution
dotnet test fmp_prototype_3.sln

# Run tests for a specific project
dotnet test datastore.tests/datastore.tests.csproj
```

### Testing with the In-Memory Repository

When testing the API manually, you can use the in-memory repository which comes pre-loaded with sample data:

1. Ensure `"UseInMemoryRepository": true` is set in `appsettings.Development.json`
2. Run the API as described in the Getting Started section
3. Use Swagger UI at `https://localhost:5001/swagger` to test API endpoints

### Testing with Cosmos DB Emulator

To test with the Cosmos DB implementation:

1. Start the Cosmos DB Emulator:
   ```bash
   # Using Docker
   docker run --name cosmos-db-emulator -p 8081:8081 \
     -p 10251-10254:10251-10254 \
     microsoft/azure-cosmosdb-emulator
   ```

2. Update `appsettings.Development.json` to use Cosmos DB:
   ```json
   "FeatureManagement": {
     "UseInMemoryRepository": false
   }
   ```

3. Restart the API application

### UI Testing

To test the UI components:

1. Start the API (as described above)
2. Start the UI development server:
   ```bash
   cd ui
   npm start
   ```
3. Navigate to `http://localhost:3000` in your browser
4. Use the UI to create, edit, and delete feature flags

## License

[Specify your license here]