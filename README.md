# TaskFlow API

A task management REST API built to demonstrate production-grade ASP.NET Core
architecture patterns. Intended as a portfolio project showcasing clean
architecture, design patterns, and senior-level .NET practices.

---

## Architecture

The solution uses Clean Architecture with four layers. Dependencies flow
inward — outer layers know about inner layers, never the reverse.
```
TaskFlow.Api           HTTP concerns only — controllers, middleware, DI wiring
TaskFlow.Application   Use cases — services, DTOs, validators, interfaces
TaskFlow.Domain        Business rules — entities, exceptions, repo interfaces
TaskFlow.Infrastructure External concerns — EF Core, JWT, decorators, handlers
```

The dependency rule is enforced by project references. Domain has zero NuGet
references. If a developer accidentally tries to use EF Core in Domain,
the build fails.

---

## Design patterns

| Pattern | Where | Why |
|---|---|---|
| Repository | Infrastructure | Decouples domain from EF Core |
| Decorator | Infrastructure | Adds logging and caching without modifying repos |
| Strategy | Notifications | One handler per notification type, resolved at runtime |
| Result&lt;T&gt; | Application | Makes failure paths explicit in method signatures |
| Factory (scope) | BackgroundService | Creates fresh DI scopes per notification |

---

## Key decisions

**`Result<T>` over exceptions for domain errors** — expected failures
(not found, forbidden, conflict) are return values, not exceptions.
Exceptions are reserved for truly unexpected failures. This makes
failure paths visible in method signatures and avoids the performance
cost of stack unwinding for expected conditions.

**Generic decorators over per-entity implementations** — `LoggingRepository<T>`
and `CachingRepository<T>` are written once and applied to every entity.
Adding a new entity requires zero new infrastructure code.

**`Channel<T>` for background jobs** — built into .NET, no external
dependencies. Push-based consumption means zero CPU when idle, unlike
polling approaches. Bounded capacity provides natural backpressure.

**`MapInboundClaims = false`** — prevents the JWT middleware from
remapping standard claim names to long WS-Federation URIs. Claim names
in the token match exactly what `CurrentUserService` reads.

**Scoped `DbContext` per notification** — `NotificationProcessor` is a
singleton `BackgroundService`. It creates a fresh DI scope per notification
to get a fresh `DbContext`, avoiding the captive dependency problem.

---

## Getting started

### Docker (recommended)

```bash
# 1. Copy/rename docker-compose.example.yml -> docker-compose.yml
# 2. Start Docker
docker-compose up --build
```

The API will be available at `http://localhost:8080`.
Swagger UI is disabled in Production — use Postman or curl.

### Manual

**Prerequisites:** .NET 10 SDK, Docker (for SQL Server)

```bash
# 1. Copy/rename docker-compose.example.yml -> docker-compose.yml
# 2. Start SQL Server
docker-compose up sqlserver -d

# 3. Apply migrations
dotnet ef database update \
  --project TaskFlow.Infrastructure \
  --startup-project TaskFlow.Api

# 4. Copy/rename appsettings.Example.json -> appsettings.Development.json
# 5. Run the API
dotnet run --project TaskFlow.Api
```

Swagger UI: `http://localhost:5073/swagger`

---

## Running tests
```bash
dotnet test
```

Integration tests use SQLite in-memory — no SQL Server required.

---

## Endpoints

| Method | Path | Auth | Description |
|---|---|---|---|
| POST | /api/v1/auth/register | No | Register a new account |
| POST | /api/v1/auth/login | No | Login and receive JWT |
| GET | /api/v1/projects | Yes | List workspace projects |
| POST | /api/v1/projects | Yes | Create a project |
| GET | /api/v1/projects/{id} | Yes | Get project by id |
| PATCH | /api/v1/projects/{id}/archive | Yes | Archive a project |
| GET | /api/v1/projects/{id}/tasks | Yes | Get paged tasks |
| POST | /api/v1/projects/{id}/tasks | Yes | Create a task |
| GET | /api/v1/projects/{id}/tasks/{taskId} | Yes | Get task by id |
| PATCH | /api/v1/projects/{id}/tasks/{taskId} | Yes | Update a task |
| PATCH | /api/v1/projects/{id}/tasks/{taskId}/assign | Yes | Assign a task |
| DELETE | /api/v1/projects/{id}/tasks/{taskId} | Yes | Delete a task |
| GET | /health | No | Full health check |
| GET | /health/live | No | Liveness check |
| GET | /health/ready | No | Readiness check (DB) |

---

## Project structure
```
TaskFlow/
├── TaskFlow.Api/              HTTP layer
│   ├── Controllers/
│   ├── HealthChecks/
│   ├── Infrastructure/        Swagger configuration
│   ├── Middleware/
│   └── Extensions/
├── TaskFlow.Application/      Use cases
│   ├── BackgroundJobs/        Channel, processor, handlers
│   ├── Common/                Result<T>, PagedResponse<T>
│   ├── DTOs/
│   ├── Interfaces/
│   ├── Services/
│   └── Validators/
├── TaskFlow.Domain/           Business rules
│   ├── Entities/
│   ├── Exceptions/
│   └── Repositories/
├── TaskFlow.Infrastructure/   External concerns
│   ├── Auth/
│   ├── Decorators/
│   ├── Notifications/
│   └── Persistence/
└── TaskFlow.Tests/            Integration tests
```