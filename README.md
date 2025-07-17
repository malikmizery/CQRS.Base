
# CQRS Pattern Library

This library provides a simple and extensible foundation for implementing the Command Query Responsibility Segregation (CQRS) pattern in .NET applications.

## Features
- **Command and Query Handler Interfaces**: Define commands and queries with strong typing.
- **Result Types**: Standardized result objects for success, failure, and validation errors.
- **Automatic DI Registration**: Extension method to scan assemblies and register all command and query handlers as scoped services.

---


## Getting Started

### 1. Define Commands, Queries, and Handlers

**Command Example:**
```csharp
public class CreateUserCommand : ICommand<Guid>
{
    public string Name { get; set; }
}
```

**Command Handler Example:**
```csharp
public class CreateUserCommandHandler : ICommandHandler<CreateUserCommand, Guid>
{
    public Task<Result<Guid>> Handle(CreateUserCommand command, CancellationToken cancellationToken)
    {
        // Business logic here
        return Task.FromResult(Result<Guid>.Success(Guid.NewGuid()));
    }
}
```

**Query Example:**
```csharp
public class GetUserQuery : IQuery<UserDto>
{
    public Guid UserId { get; set; }
}
```

**Query Handler Example:**
```csharp
public class GetUserQueryHandler : IQueryHandler<GetUserQuery, UserDto>
{
    public Task<Result<UserDto>> Handle(GetUserQuery query, CancellationToken cancellationToken)
    {
        // Business logic here
        return Task.FromResult(Result<UserDto>.Success(new UserDto()));
    }
}
```

---

### 2. Register Handlers in DI

In your `Program.cs` or `Startup.cs`:

```csharp
using CqrsPattern;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Register all CQRS handlers from the current assembly
builder.Services.AddCQRSHandlersFromAssembly(Assembly.GetExecutingAssembly());

var app = builder.Build();
// ...
```

---

### 3. Use Handlers via Minimal API

```csharp
app.MapPost("/users", async (CreateUserCommand command, ICommandHandler<CreateUserCommand, Guid> handler, CancellationToken cancellationToken) =>
{
    var result = await handler.Handle(command, cancellationToken);
    return result.IsSuccess
        ? Results.Ok(result.Value)
        : Results.BadRequest(result.ErrorMessage);
});
```

---

## Summary
- Define your commands, queries, and handlers.
- Register all handlers with one line using the provided extension method.
- Use handlers via DI in your application.

This approach keeps your codebase clean, testable, and scalable using the CQRS pattern.
