# CQRS Pattern Library

This library provides a simple and extensible foundation for implementing the Command Query Responsibility Segregation (CQRS) pattern in .NET applications.

## Features
- **Command and Query Handler Interfaces**: Define commands and queries with strong typing.
- **Result Types**: Standardized result objects for success, failure, and validation errors.
- **Automatic DI Registration**: Extension method to scan one or many assemblies and register all command and query handlers as scoped services.

---

## Installation

### NuGet (recommended)
```bash
dotnet add package CQRS.Base
```

### PackageReference
```xml
<ItemGroup>
  <PackageReference Include="CQRS.Base" Version="1.0.3" />
</ItemGroup>
```

---

## Core Abstractions

- Commands (no return): [`CQRS.Base.ICommand`](src/ICommand.cs)
- Commands (with return): [`CQRS.Base.ICommand<TResponse>`](src/ICommand.cs)
- Queries (with return): [`CQRS.Base.IQuery<TResponse>`](src/IQuery.cs)
- Command handlers: 
  - No return: [`CQRS.Base.ICommandHandler<TCommand>`](src/ICommandHandler.cs)
  - With return: [`CQRS.Base.ICommandHandler<TCommand,TResponse>`](src/ICommandHandler.cs)
- Query handlers: [`CQRS.Base.IQueryHandler<TQuery,TResponse>`](src/IQueryHandler.cs)
- Results: [`CQRS.Base.Result`](src/Result.cs), [`CQRS.Base.Result<TResponse>`](src/ResultTResponse.cs)
- DI registration (multi-assembly): [`CQRS.Base.ServiceCollectionExtensions.AddCQRSHandlersFromAssemblies`](src/ServiceCollectionExtensions.cs)

---

## Result Pattern

Use factory helpers from [`Result`](src/Result.cs):
```csharp
return Result.Success();
return Result.Failure("Something went wrong");
return Result.Failure("BusinessRule", "Credit limit exceeded");
return Result.NotFound("User not found");
return Result.BadRequest(validationErrorsDictionary);

return Result.Success<Guid>(id);
return Result.Failure<Guid>("Could not create user");
```

A handler returning data:
```csharp
public class GetUserQueryHandler : IQueryHandler<GetUserQuery, UserDto>
{
    public async Task<Result<UserDto>> Handle(GetUserQuery query, CancellationToken ct = default)
    {
        var user = await repo.FindAsync(query.UserId, ct);
        return user is null
            ? Result.NotFound<UserDto>("User not found")
            : Result.Success(user.ToDto());
    }
}
```

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
    public Task<Result<Guid>> Handle(CreateUserCommand command, CancellationToken cancellationToken = default)
    {
        // Business logic here
        return Task.FromResult(Result.Success(Guid.NewGuid()));
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
    public Task<Result<UserDto>> Handle(GetUserQuery query, CancellationToken cancellationToken = default)
    {
        // Business logic here
        return Task.FromResult(Result.Success(new UserDto()));
    }
}
```

---

### 2. Register Handlers in DI

In your `Program.cs` you can register handlers from a single assembly or multiple assemblies.

Single assembly (just wrap in an array):
```csharp
using CQRS.Base;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCQRSHandlersFromAssemblies(new[]{ Assembly.GetExecutingAssembly() });

var app = builder.Build();
```

Multiple assemblies:
```csharp
var assemblies = new[]
{
    Assembly.GetExecutingAssembly(),
    typeof(SomeFeatureMarker).Assembly // any other assembly containing handlers
};

builder.Services.AddCQRSHandlersFromAssemblies(assemblies);
```

This will scan every provided assembly for implementations of:
* ICommandHandler<TCommand>
* ICommandHandler<TCommand, TResponse>
* IQueryHandler<TQuery, TResponse>

and register them as scoped services.

---

### 3. Use Handlers via Minimal API

```csharp
app.MapPost("/users", async (
    CreateUserCommand command,
    ICommandHandler<CreateUserCommand, Guid> handler,
    CancellationToken ct) =>
{
    var result = await handler.Handle(command, ct);
    return result.IsSuccess
        ? Results.Created($"/users/{result.Value}", result.Value)
        : MapResult(result);
});
```

Centralized HTTP mapping:
```csharp
IResult MapResult<T>(Result<T> r) => r.IsSuccess switch
{
    true => Results.Ok(r.Value),
    _ => r.ErrorCode switch
    {
        "NotFound" => Results.NotFound(r.ErrorMessage),
        "ValidationError" => Results.BadRequest(new { r.ErrorMessage, r.Errors }),
        _ => Results.BadRequest(new { r.ErrorCode, r.ErrorMessage })
    }
};
```

---

## Console App Example

```csharp
using CQRS.Base;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

var services = new ServiceCollection()
    .AddCQRSHandlersFromAssembly(Assembly.GetExecutingAssembly())
    .BuildServiceProvider();

var handler = services.GetRequiredService<ICommandHandler<CreateUserCommand, Guid>>();
var result = await handler.Handle(new CreateUserCommand { Name = "Alice" });

Console.WriteLine(result.IsSuccess ? $"Created {result.Value}" : result.ErrorMessage);
```

---

## Testing Handlers

```csharp
[Fact]
public async Task CreateUser_Returns_Id()
{
    var handler = new CreateUserCommandHandler();
    var result = await handler.Handle(new CreateUserCommand { Name = "Test" });
    Assert.True(result.IsSuccess);
    Assert.NotEqual(Guid.Empty, result.Value);
}
```

No special test infrastructure required—handlers are plain classes.

---

## Patterns & Tips

- Use plain POCOs for commands/queries; keep them small.
- Put validation either before calling handlers or inside handlers returning `Result.BadRequest(errors)`.
- Prefer `ICommand` (no generic) for operations without return data; use `ICommand<T>` or `IQuery<T>` when data is required.
- You can layer a mediator or pipeline behaviors later without changing existing handlers.

---

## Summary
- Define your commands, queries, and handlers.
- Register all handlers with one line using the provided extension method.
- Use handlers via DI in your application.
- Standardize responses with `Result` / `Result<T>`.

This approach keeps the codebase clean, testable, and scalable using the CQRS pattern.

---

##
