# Vertical Slices Demo
Vertical slices demo application with 
- a simple DDD-style domain
- CRUD operations organized in feature folders
- integration tests 
- and more...

## How to run

Make sure Docker Desktop is running for the container based integration tests to work.

## Technologies
- ✔️ **[`.NET 9`](https://dotnet.microsoft.com/download)** - .NET Framework and .NET Core, including ASP.NET and ASP.NET Core
- ✔️ **[`Swagger & Swagger UI`](https://github.com/domaindrivendev/Swashbuckle.AspNetCore)** - Swagger tools for documenting API's built on ASP.NET Core
- ✔️ **[`Swashbuckle.AspNetCore.Cli`](https://github.com/domaindrivendev/Swashbuckle.AspNetCore)** - Provides a command line interface for retrieving Swagger directly from a startup assembly, and writing to file
- ✔️ **[`MediatR`](https://github.com/jbogard/MediatR)** - Simple, unambitious mediator implementation in .NET.
- ✔️ **[`FluentValidation`](https://github.com/FluentValidation/FluentValidation)** - Popular .NET validation library for building strongly-typed validation rules
- ✔️ **[`Serilog`](https://github.com/serilog/serilog)** - Simple .NET logging with fully-structured events
- ✔️ **[`xUnit.net`](https://github.com/xunit/xunit)** - A free, open source, community-focused unit testing tool for the .NET Framework.
- ✔️ **[`Respawn`](https://github.com/jbogard/Respawn)** - Respawn is a small utility to help in resetting test databases to a clean state.
- ✔️ **[`Testcontainers`](https://github.com/testcontainers/testcontainers-dotnet)** - Testcontainers for .NET is a library to support tests with throwaway instances of Docker containers.
- ✔️ **[`Awesome Assertions`](https://awesomeassertions.org/)** - A very extensive set of extension methods that allow you to more naturally specify the expected outcome of a TDD or BDD-style unit tests.

## Articles / Videos
- [The complete guide to unit testing structure best practices](https://www.youtube.com/watch?v=adaQ52DMitE)
- [5 Amazing Use Cases for MediatR Pipelines - Cross-Cutting Concerns](https://www.youtube.com/watch?v=Iql4yjHYRiA)
- [Validation Behavior | MediatR + FluentValidation | CLEAN ARCHITECTURE & DDD Tutorial | Part 8](https://www.youtube.com/watch?v=FXP3PQ03fa0)
- [Use These 4 Best Practices For Your .NET Project Setup](https://www.youtube.com/watch?v=B9ZUJN1Juhk)
- [ASP.NET 6 REST API Following CLEAN ARCHITECTURE & DDD Tutorial - by Amichai Mantinband (full series of videos)](https://www.youtube.com/watch?v=fhM0V2N1GpY&list=PLzYkqgWkHPKBcDIP5gzLfASkQyTdy0t4k)
- [Domain-Driven Design - by Amichai Mantinband (series of videos)](https://www.youtube.com/watch?v=8Z5IAkWcnIw&list=PLzYkqgWkHPKDpXETRRsFv2F9ht6XdAF3v)

# Integration Testing

## Integration Testing with Respawn & TestContainers
[`Testcontainers`](https://github.com/testcontainers/testcontainers-dotnet) - Testcontainers for .NET is a library to support tests with throwaway instances of Docker containers.

[`Respawn`](https://github.com/jbogard/Respawn) - Respawn is a small utility to help in resetting test databases to a clean state.

[Testcontainers Reuse](https://dotnet.testcontainers.org/api/resource_reuse/) is an experimental feature designed to simplify and enhance the development experience. Instead of disposing resources after the tests are finished, enabling reuse will retain the resources and reuse them in the next test run.

## Working with an auto-regenerating Api client
[Auto-Regenerating API Client for Your Open API Project](https://techcommunity.microsoft.com/t5/healthcare-and-life-sciences/auto-regenerating-api-client-for-your-open-api-project/ba-p/3302390)

## Output logs to Xunit
[serilog-sinks-xunit](https://github.com/trbenning/serilog-sinks-xunit) - The xunit test output sink for Serilog
