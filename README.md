# TaskFlow API

A task management REST API built to demonstrate production-grade
ASP.NET Core patterns.

## Architecture decisions

**Clean architecture** — domain layer has zero dependencies.
Application layer depends only on domain interfaces.
Infrastructure implements those interfaces. The dependency rule
is enforced by project references.

**Generic repository + decorator pattern** — a single
`EfRepository<T>` handles all entities. Logging and caching
are added via `LoggingRepository<T>` and `CachingRepository<T>`
decorators registered with Scrutor. Adding a new entity requires
zero changes to infrastructure.

**Result<T> over exceptions** — domain operations return
`Result<T>` instead of throwing for expected failures.
Exceptions are reserved for truly exceptional cases
(infrastructure down, bugs). This makes the failure surface
explicit in every method signature.

**Channel<T> for background jobs** — notifications are enqueued
to a bounded `Channel<INotification>` and processed by a
`BackgroundService`. The HTTP response is never delayed by
notification delivery.

## Design patterns used

| Pattern | Where | Why |
|---------|-------|-----|
| Repository | Infrastructure | Decouple domain from EF Core |
| Decorator | Infrastructure | Add logging/caching without modifying repos |
| Strategy | Notification handlers | Swap handler per notification type |
| Factory | DI registration | `IServiceScopeFactory` in BackgroundService |
| Result | Application | Explicit error handling without exceptions |

## Running locally

```bash
docker-compose up -d        # starts SQL Server
dotnet ef database update   # apply migrations
dotnet run --project TaskFlow.Api
```

## Key endpoints

POST   /api/v1/auth/register
POST   /api/v1/auth/login
GET    /api/v1/projects
POST   /api/v1/projects
GET    /api/v1/projects/{id}/tasks
POST   /api/v1/projects/{id}/tasks
PATCH  /api/v1/projects/{id}/tasks/{taskId}/complete
DELETE /api/v1/projects/{id}/tasks/{taskId}