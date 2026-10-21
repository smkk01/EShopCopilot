# EShop Copilot Instructions

## Build and run

- Build the full solution from the repository root with:
  ```powershell
  dotnet build .\src\EShop.sln
  ```
- Build only the API service with:
  ```powershell
  dotnet build .\src\EShop.ApiService\EShop.ApiService.csproj
  ```
- Run the distributed application through the Aspire CLI:
  ```powershell
  aspire run --project .\src\EShop.AppHost\EShop.AppHost.csproj
  ```
- `dotnet run --project .\src\EShop.AppHost\EShop.AppHost.csproj` is the equivalent fallback when the Aspire CLI is unavailable.
- There are currently no test projects, test runner configuration, or lint/format scripts. `EShop.ApiService.http` contains manual API request examples.

## Architecture

- `EShop.AppHost` is the Aspire orchestration entry point. It starts `apiservice` and `webfrontend`; the web project references and waits for the API service. Add new runnable projects and inter-service references here so Aspire can configure discovery and startup ordering.
- `EShop.ApiService` is a .NET Minimal API. Feature code is organized by `Models`, `Services`, and `Endpoints`. `Program.cs` registers services and maps endpoint extension methods.
- `EShop.Web` is a Blazor Server application using interactive server components. It is currently independent of API client calls.
- `EShop.ServiceDefaults` is shared infrastructure for every runnable service. Call `builder.AddServiceDefaults()` during setup and `app.MapDefaultEndpoints()` during mapping; do not replace or omit either when changing a runnable service. They retain service discovery, resilience, OpenTelemetry, and development health endpoints (`/health` and `/alive`).

## API conventions

- Implement each API feature with a model in `Models`, business logic in `Services`, and a `Map<Feature>Endpoints(this IEndpointRouteBuilder app)` extension in `Endpoints`.
- Register stateful in-memory services as singletons in `Program.cs`. `ProductService` protects its mutable list with a lock and returns cloned models so callers cannot mutate stored state.
- Group feature routes below `/api/<resource>` with `MapGroup`, add `.WithTags(...)`, and give routes used by `CreatedAtRoute` a stable `.WithName(...)`.
- Use `TypedResults` and explicit typed result return values (`Results<...>`) for endpoint handlers that can return different HTTP statuses. Return `NotFound` for missing resources, `CreatedAtRoute` after creation, and `NoContent` after successful update or deletion.
- Maintain request examples for all API operations in `EShop.ApiService.http` when changing an endpoint contract.

## .NET 10 style

- Keep every project targeting `net10.0`, with nullable reference types and implicit usings enabled.
- Prefer modern C# collection expressions for initialized collections, such as `List<Product> products = [ ... ];`, rather than `new List<Product> { ... }`.
- Keep endpoint declarations thin: delegate state changes and business rules to services, and place the corresponding route mapping in the feature's `MapGroup` extension method.
- Use route constraints such as `{id:int}` for typed identifiers, and retain named GET-by-ID routes when POST handlers use `CreatedAtRoute`.
