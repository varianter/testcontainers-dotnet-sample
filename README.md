# TestContainers .NET Sample

This repository contains a sample project that demonstrates how to use [TestContainers](https://www.testcontainers.org/) with .NET.

These are my personal preferences but include how to use it in E2E/integration tests with a real database (PostgreSQL), and also using TestContainers for local development.

This includes how to enable re-use of containers to speed up the test execution and Api startup time.

The most interesting bits are under:

- [`src/Api/TestContainers`](src/Api/TestContainers) - Contains the TestContainers setup for the API
- [`tests/Api.Tests/Shared/ApiFixture.cs`](tests/Api.Tests/Shared/ApiFixture.cs) - Contains the test collection fixture that sets up the shared ApiFactory for all tests.
- [`tests/Api.Tests/Shared/ApiFactory.cs`](tests/Api.Tests/Shared/ApiFactory.cs) - Contains the factory that creates the in-memory API, and uses TestContainers for a shared database, which is reset between each test with `Respawn`.

## Slides

This sample project was demoed at a [Variantdag](https://handbook.variant.no/#Variantdag) at Variant, the slides are included in the repository under [`slides.pdf`](./slides.pdf)` (in Norwegian).
