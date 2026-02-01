# 🎮 BattleTanks Backend

Backend API for the BattleTanks game, built with **.NET 10** following **Clean Architecture** principles.

## Technologies

- **.NET 10**
- **PostgreSQL** - User data persistence
- **Firebase Authentication** - User authentication via ID tokens
- **Entity Framework Core** - ORM for database operations

## Configuration

### 1. Firebase Setup

Copy the base configuration file and add your credentials:

```bash
cp src/BattleTanks.Api/BattleTanks.Api/appsettings.Development.json.example src/BattleTanks.Api/BattleTanks.Api/appsettings.json
```

#### Obtain Firebase Credentials

1. Obtain your `firebase-service-account.json` from the [Firebase Console](https://console.firebase.google.com/).
2. Save it in `src/BattleTanks.Api/BattleTanks.Api/`.
3. Update the `ProjectId` in your `appsettings.json`.

### 2. Database (Entity Framework)

Create and apply the database migration:

```bash
# Restore dependencies
dotnet restore

# Apply migrations to the local database
dotnet ef database update --project src/BattleTanks.Infrastructure --startup-project src/BattleTanks.Api/BattleTanks.Api
```

## Running the Application

You can start the development server via the .NET CLI:

```bash
dotnet run --project src/BattleTanks.Api/BattleTanks.Api
```

Once started, access the interactive documentation:

- **Swagger UI:** [http://localhost:5000/swagger](http://localhost:5000/swagger)

## Project Architecture

The system follows **Clean Architecture** with 4 layers organized by features:

### Layer Structure

Each layer is organized by features (Auth, Users):

```
BattleTanks.Domain/
├── Entities/
│   └── Users/
└── Interfaces/
    ├── Auth/
    └── Users/

BattleTanks.Application/
├── DTOs/
│   ├── Auth/
│   └── Users/
├── Interfaces/
└── Services/
    ├── Auth/
    └── Users/
```

### Responsibilities

- **Domain/** - Entities, Exceptions, and Repository Contracts
- **Application/** - Use Cases, DTOs, and Business Logic
- **Infrastructure/** - Persistence (EF Core), Firebase SDK, and External Services
- **Api/** - API Controllers and Dependency Injection

## Folder Organization

```text
src/
├── BattleTanks.Api/
│   └── BattleTanks.Api/        # API Host (Controllers, DI, Program.cs)
├── BattleTanks.Application/     # Services + DTOs (by feature)
├── BattleTanks.Domain/          # Entities + Interfaces (by feature)
└── BattleTanks.Infrastructure/  # DbContext + Repositories
    └── Persistence/
        ├── BattleTanksDbContext.cs
        ├── Configurations/
        │   └── Users/
        └── Repositories/
            ├── Auth/
            └── Users/
```
