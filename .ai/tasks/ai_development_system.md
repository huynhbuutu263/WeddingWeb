## TASKS

**Task ID:** TSK-001
**Goal:** Scaffold the Clean Architecture solution and projects.
**Files:** `WeddingApp.sln`, `.csproj` files for `src` (Domain, Application, Infrastructure, Api) and `tests` (UnitTests, IntegrationTests).
**Implementation:** Run `dotnet new sln`. Create classlibs for each layer. Create Web API for the Api project. Create xunit projects for tests.
**Unit Tests:** None
**Acceptance Criteria:** Directory structure exists with `src/` and `tests/`.

**Task ID:** TSK-002
**Goal:** Map Solution Dependencies.
**Files:** `WeddingApp.sln`, all `.csproj` files.
**Implementation:** Use `dotnet sln add` to add all projects to the solution. Run `dotnet add reference` to connect projects: Api depends on App & Infra; Infra depends on App; App depends on Domain; Test projects depend on their targets.
**Unit Tests:** None
**Acceptance Criteria:** `dotnet build` executes successfully with completely isolated Domain and Application layers.

**Task ID:** TSK-003
**Goal:** Define Core Domain Entities base.
**Files:** `src/WeddingApp.Domain/Common/BaseEntity.cs`, `src/WeddingApp.Domain/Entities/User.cs`, `src/WeddingApp.Domain/Entities/Guest.cs`
**Implementation:** Create `BaseEntity` with `Id`, `CreatedAt`, `UpdatedAt`. Create `User` for admin management and `Guest` for card visitors.
**Unit Tests:** `tests/WeddingApp.Domain.UnitTests/Entities/UserTests.cs`
**Acceptance Criteria:** Entities are defined with proper encapsulation and no framework dependencies.

**Task ID:** TSK-004
**Goal:** Define Wedding Card & Template Entities.
**Files:** `src/WeddingApp.Domain/Entities/WeddingCard.cs`, `src/WeddingApp.Domain/Entities/Template.cs`
**Implementation:** Add `SlugUrl`, `Title`, `EventDate`, `TemplateId` to `WeddingCard`. Create `Template` with `Name` and `HtmlStructure`.
**Unit Tests:** `tests/WeddingApp.Domain.UnitTests/Entities/WeddingCardTests.cs` (ensure it cannot be initialized with empty string values).
**Acceptance Criteria:** Complex relations between Card and Template are defined purely via code classes.

**Task ID:** TSK-005
**Goal:** Configure Entity Framework Core DbContext.
**Files:** `src/WeddingApp.Infrastructure/Data/AppDbContext.cs`, Entity Configurations.
**Implementation:** `dotnet add package Microsoft.EntityFrameworkCore.SqlServer` to Infrastructure. Inherit `DbContext`. Set up `DbSet`s. Configure `WeddingCard.SlugUrl` as a unique database index to prevent duplicate sharing slugs.
**Unit Tests:** None.
**Acceptance Criteria:** DbContext successfully initializes and compiles.

**Task ID:** TSK-006
**Goal:** Initialize Database Migrations.
**Files:** Database schema.
**Implementation:** Run `dotnet ef migrations add Initial` and `dotnet ef database update` in the Infrastructure project (using Api as startup).
**Unit Tests:** None.
**Acceptance Criteria:** The physical database schema is created without errors.

**Task ID:** TSK-007
**Goal:** Set up Application layer Mediator & Validation pipeline.
**Files:** `src/WeddingApp.Application/DependencyInjection.cs`, `src/WeddingApp.Application/Common/Behaviors/ValidationBehavior.cs`
**Implementation:** Run `dotnet add package MediatR` and `dotnet add package FluentValidation.DependencyInjectionExtensions` to Application. Create a PipelineBehavior that automatically runs validators before handlers execute.
**Unit Tests:** `tests/WeddingApp.Application.UnitTests/Behaviors/ValidationBehaviorTests.cs`
**Acceptance Criteria:** Any command failing FluentValidation automatically throws a `ValidationException`.

**Task ID:** TSK-008
**Goal:** Global Exception Handling in API.
**Files:** `src/WeddingApp.Api/Middleware/ExceptionMiddleware.cs`
**Implementation:** Catch `ValidationException` (return 400 Bad Request with errors), `NotFoundException` (return 404), and map everything else to 500 Internal Server error using standard ProblemDetails.
**Unit Tests:** `tests/WeddingApp.Api.IntegrationTests/Middleware/ExceptionMiddlewareTests.cs`
**Acceptance Criteria:** API does not leak stack traces to the client and formats errors uniformly.

**Task ID:** TSK-009
**Goal:** Command to Seed or Create Wedding Templates.
**Files:** `src/WeddingApp.Application/Templates/Commands/CreateTemplateCommand.cs`, `tests/WeddingApp.Application.UnitTests/Templates/CreateTemplateCommandHandlerTests.cs`
**Implementation:** MediatR handler to add a new `Template` (e.g., "Classic Blue") to the database.
**Unit Tests:** Handler tests using mocked DbContext interface.
**Acceptance Criteria:** Templates can be inserted into the database via CQRS flow.

**Task ID:** TSK-010
**Goal:** Query to List all available Templates.
**Files:** `src/WeddingApp.Application/Templates/Queries/GetTemplatesQuery.cs`, `tests/WeddingApp.Application.UnitTests/Templates/GetTemplatesQueryHandlerTests.cs`
**Implementation:** Create query returning a list of `TemplateDto`. Use `AsNoTracking()` for fast reads.
**Unit Tests:** Handler test returning stubbed templates.
**Acceptance Criteria:** Clients can query all system templates to display via UI.

**Task ID:** TSK-011
**Goal:** Setup File Storage Interface for Image Uploads.
**Files:** `src/WeddingApp.Application/Common/Interfaces/IFileStorageService.cs`
**Implementation:** Create an abstract interface defining `Task<string> UploadImageAsync(Stream data, string filename)`.
**Unit Tests:** None.
**Acceptance Criteria:** Application defines infrastructure needs without implementing them.

**Task ID:** TSK-012
**Goal:** Implement File Storage Service.
**Files:** `src/WeddingApp.Infrastructure/Services/LocalFileStorageService.cs`, `tests/WeddingApp.Infrastructure.UnitTests/Services/LocalFileStorageServiceTests.cs`
**Implementation:** Implement `IFileStorageService` by writing files to a `wwwroot/uploads` folder and returning the relative public URL.
**Unit Tests:** Unit test file writing using temporary directories.
**Acceptance Criteria:** Images are saved physically to the server and return a valid access URL.

**Task ID:** TSK-013
**Goal:** Feature - Upload Image Command.
**Files:** `src/WeddingApp.Application/Images/Commands/UploadImageCommand.cs`, `tests/WeddingApp.Application.UnitTests/Images/UploadImageCommandHandlerTests.cs`
**Implementation:** Accept an `IFormFile` (or byte array stream), pass it to `IFileStorageService`, and return the URL. 
**Unit Tests:** Test command logic delegates perfectly to file storage interface.
**Acceptance Criteria:** Command properly routes data to storage interface and returns the URL.

**Task ID:** TSK-014
**Goal:** Web API Endpoint for Upload Image.
**Files:** `src/WeddingApp.Api/Controllers/ImagesController.cs`, `tests/WeddingApp.Api.IntegrationTests/Controllers/ImagesControllerTests.cs`
**Implementation:** Add `POST /api/images` mapping to MediatR `UploadImageCommand`.
**Unit Tests:** Integration test for POST route.
**Acceptance Criteria:** Post request with an image file returns a string URL via HTTP API.

**Task ID:** TSK-015
**Goal:** Feature - Create Wedding Card.
**Files:** `src/WeddingApp.Application/Cards/Commands/CreateCardCommand.cs`, `src/WeddingApp.Application/Cards/Commands/CreateCardValidator.cs`, `tests/WeddingApp.Application.UnitTests/Cards/CreateCardCommandHandlerTests.cs`
**Implementation:** Take user inputs (Title, Slug, Date, TemplateId). Validate Slug is alphanumeric. Ensure Template exists. Save to database.
**Unit Tests:** Test handler correctly saves data; test validator correctly rejects invalid slugs.
**Acceptance Criteria:** Valid card creation command results in database insertion and returns new ID.

**Task ID:** TSK-016
**Goal:** Web API Endpoint for Calling Create Card.
**Files:** `src/WeddingApp.Api/Controllers/CardsController.cs`, `tests/WeddingApp.Api.IntegrationTests/Controllers/CardsControllerTests.cs`
**Implementation:** Add `POST /api/cards` mapping to the MediatR `CreateCardCommand`.
**Unit Tests:** Integration test targeting that endpoint via `WebApplicationFactory`.
**Acceptance Criteria:** Endpoint returns 201 Created and the route to view the card.

**Task ID:** TSK-017
**Goal:** Feature - Upload and Link Images to Wedding Card.
**Files:** `src/WeddingApp.Application/Cards/Commands/AddGalleryImageCommand.cs`, `src/WeddingApp.Domain/Entities/CardImage.cs`
**Implementation:** Add a 1-to-many relationship between `WeddingCard` and a new `CardImage` entity. Handler takes an uploaded image URL and maps it to the card.
**Unit Tests:** Ensure image addition accurately modifies the Card entity state.
**Acceptance Criteria:** Cards can contain a list of associated image URLs.

**Task ID:** TSK-018
**Goal:** Feature - Share Card via Slug URL.
**Files:** `src/WeddingApp.Application/Cards/Queries/GetCardBySlugQuery.cs`, `tests/WeddingApp.Application.UnitTests/Cards/GetCardBySlugQueryHandlerTests.cs`
**Implementation:** Find card by `SlugUrl`. Eager load `Template` and linked `CardImages`.
**Unit Tests:** Test handler successfully retrieves data or throws `NotFoundException`.
**Acceptance Criteria:** Sending a known slug string returns the complete flattened payload to render the wedding card.

**Task ID:** TSK-019
**Goal:** Web API Endpoint for Viewing Shared Card.
**Files:** `src/WeddingApp.Api/Controllers/CardsController.cs`, `tests/WeddingApp.Api.IntegrationTests/Controllers/CardsControllerGetTests.cs`
**Implementation:** Add `GET /api/cards/{slug}` mapped to the slug query.
**Unit Tests:** Integration test returning 200 OK and card payload.
**Acceptance Criteria:** Public access string maps directly to HTTP GET.

**Task ID:** TSK-020
**Goal:** Setup JWT Authentication.
**Files:** `src/WeddingApp.Infrastructure/Authentication/JwtProvider.cs`, `src/WeddingApp.Api/Program.cs`
**Implementation:** Configure standard ASP.NET Core Bearer token authentication. Ensure secret key validation. Install required NuGet packages.
**Unit Tests:** None.
**Acceptance Criteria:** Requests with `[Authorize]` return 401 when no token is present.

**Task ID:** TSK-021
**Goal:** Feature - Admin Registration.
**Files:** `src/WeddingApp.Application/Admin/Commands/RegisterAdminCommand.cs`
**Implementation:** Create command to hash password, save administrative `User`.
**Unit Tests:** Ensure passwords are not stored in plaintext.
**Acceptance Criteria:** Admin account can be stored securely.

**Task ID:** TSK-022
**Goal:** Feature - Admin Login.
**Files:** `src/WeddingApp.Application/Admin/Commands/LoginAdminCommand.cs`
**Implementation:** Validate credentials against DB. Use `IJwtProvider` to generate and return token.
**Unit Tests:** Test invalid passwords reject the token request.
**Acceptance Criteria:** Valid credentials return a usable JWT string.

**Task ID:** TSK-023
**Goal:** Feature - Dashboard View Query.
**Files:** `src/WeddingApp.Application/Admin/Queries/GetDashboardStatsQuery.cs`, `tests/WeddingApp.Application.UnitTests/Admin/GetDashboardStatsQueryTests.cs`
**Implementation:** MediatR query returning totals: total templates, total cards created.
**Unit Tests:** Query test.
**Acceptance Criteria:** Query successfully returns statistics object.

**Task ID:** TSK-024
**Goal:** Web API Endpoint for Admin operations.
**Files:** `src/WeddingApp.Api/Controllers/AdminController.cs`
**Implementation:** Mapping Auth, Registration, and Dashboard Query to standard endpoints. Add `[Authorize]` where required.
**Unit Tests:** Route tests for `AdminController`.
**Acceptance Criteria:** Endpoints are correctly routed and secured with JWT.


## SKILLS

*(Skill files have been generated externally as standalone `.skill.md` files: architecture, coding, testing, debugging, task_execution)*


## WORKFLOW

- **How to read tasks:**
    1. Read exactly ONE task starting chronologically. Do not look ahead unless blocking dependencies occur.
    2. Understand exactly which domain layer the `Files:` property targets.
- **How to implement:**
    1. Start in the innermost ring (Domain/Base logic).
    2. Build out interfaces.
    3. Implement queries/commands in Application.
    4. Provide concrete classes in Infrastructure.
    5. Expose routes via API controllers.
- **How to run tests:**
    1. Before writing infrastructure implementations, write unit tests against MediatR handlers utilizing Moq (or similar) over the interfaces.
    2. Command execution: run `dotnet test`. Expect red initially.
- **How to debug:**
    1. Read terminal output completely. Identify standard trace lines.
    2. Identify layer boundary violations if DI errors occur.
    3. Change code iteratively. Re-test immediately via `dotnet build` or `dotnet test`.
    4. Only flag Task as "Complete" when tests pass and code strictly fulfills Acceptance Criteria.


## TESTING STRATEGY

- **Unit test rules:**
    - Must abide by Arrange, Act, Assert.
    - Focus heavily on Application Layer (Commands, Queries, Validators).
    - Interfaces mapping database (DbContext) or External services (File IO) MUST be mocked.
- **Integration test rules:**
    - Target the API layer using `WebApplicationFactory`.
    - Setup in-memory DbContext per test fixture.
    - Test standard user "Journeys" (Create Card -> Read Card).
- **How AI should verify correctness:**
    - The AI agent MUST rely on compiler limits and `dotnet test` output as ground truth.
    - AI must strictly cross-evaluate test expectations and the `Acceptance Criteria` fields before advancing sequentially. 
    - If compiling passes but logical bugs occur, AI is expected to use `ITelemetry` / logs or write aggressive negative assert tests to root out issues.
