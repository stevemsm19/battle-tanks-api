# BattleTanks Backend

Backend API for the BattleTanks game, built with **.NET 10** following **Clean Architecture** principles.

## Technologies

- **.NET 10**
- **PostgreSQL** - User data persistence
- **Redis** - Caching and leaderboard management
- **Firebase Authentication** - User authentication via ID tokens
- **Entity Framework Core** - ORM for database operations
- **SignalR** - Real-time WebSocket communication
- **Swagger** - Interactive API documentation

## Getting Started

Follow these steps to set up and run the project locally.

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [Docker & Docker Compose](https://docs.docker.com/get-docker/)
- [Make](https://www.gnu.org/software/make/) (optional, for shortcut commands)

### 1. Clone the Repository

```bash
git clone <repository-url>
cd BattleTanks-Backend
```

### 2. Configure Environment Variables

Copy the example environment file and set your credentials:

```bash
cp .env.example .env
```

Edit the `.env` file with your values:

```env
POSTGRES_USER=my_db_user_here
POSTGRES_PASSWORD=my_secure_db_password
POSTGRES_DB=battle_tanks_db
REDIS_PASSWORD=my_secure_redis_password
```

### 3. Start Docker Services (PostgreSQL & Redis)

Run Docker Compose to spin up the PostgreSQL and Redis containers:

```bash
docker compose up -d
```

This will start the following services:

| Service       | Container                   | Port |
| ------------- | --------------------------- | ---- |
| PostgreSQL 18 | `postgres-battle-tanks`     | 5432 |
| Redis Stack   | `redis-battle-tanks`        | 6379 |
| RedisInsight  | `redisinsight-battle-tanks` | 5540 |

> **Note:** Make sure ports 5432, 6379, and 5540 are available on your machine.

### 4. Firebase Setup

1. Obtain your `firebase-service-account.json` from the [Firebase Console](https://console.firebase.google.com/).
2. Save it in `src/BattleTanks.Api/BattleTanks.Api/`.
3. Copy the appsettings example file:

```bash
cp src/BattleTanks.Api/BattleTanks.Api/appsettings.Development.json.example src/BattleTanks.Api/BattleTanks.Api/appsettings.json
```

4. Update the `ProjectId` in your `appsettings.json`.

### 5. Apply Database Migrations

Restore dependencies and apply the migrations to create the database schema:

```bash
# Restore dependencies
dotnet restore

# Apply migrations
make db-update
```

Or without Make:

```bash
dotnet ef database update --project src/BattleTanks.Infrastructure --startup-project src/BattleTanks.Api
```

### 6. Run the Application

```bash
make run
```

Or without Make:

```bash
dotnet run --project src/BattleTanks.Api
```

Once started, the application will be available at:

- **API Base URL:** [http://localhost:5000/](http://localhost:5000/)
- **Swagger UI (Endpoints):** [http://localhost:5000/swagger](http://localhost:5000/swagger)

## Makefile Commands

| Command                      | Description                        |
| ---------------------------- | ---------------------------------- |
| `make run`                   | Run the application                |
| `make migration name=<Name>` | Create a new EF Core migration     |
| `make db-update`             | Apply pending migrations to the DB |
| `make reset-all`             | Clean, rebuild, and update the DB  |

## Project Architecture

The system follows **Clean Architecture** with 4 layers organized by features:

### Layer Structure

Each layer is organized by features (Auth, Users, Game):

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
- **Infrastructure/** - Persistence (EF Core), Firebase SDK, Redis, SignalR Hubs, and External Services
- **Api/** - API Controllers and Dependency Injection

## Folder Organization

```text
src/
├── BattleTanks.Api/          # API Host (Controllers, DI, Program.cs)
├── BattleTanks.Application/  # Services + DTOs (by feature)
├── BattleTanks.Domain/       # Entities + Interfaces (by feature)
└── BattleTanks.Infrastructure/
    ├── Persistence/           # DbContext, Configurations, Repositories
    ├── Hubs/                  # SignalR Hubs (GameHub)
    └── Services/              # Redis, Firebase, External Services
```
