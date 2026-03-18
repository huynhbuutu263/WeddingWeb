# Execution Prompt

## Objective
To implement a specific task strictly following the project's Clean Architecture conventions.

## Trigger
Activates when the user assigns a new task from `.ai/tasks/` or when moving to the next task after a successful review.

## Core Rules
- Do not jump ahead to the API or Infrastructure layers before the Domain is solid.
- Do not create huge monolithic files. Adhere to single responsibility.
- Before considering your execution complete, ensure `dotnet build` executes with zero errors.

## Workflow
1. **Read Task Definition**: Fully parse the `.ai/tasks/task_XX.md` file given by the user. Understand the "Goal", "Files", and "Acceptance Criteria".
2. **Start Inner-Most**: Always start developing within the `Domain` layer (Entities, Enums, Value Types).
3. **Application Logic**: Build Interfaces, `MediatR` Commands/Queries, Validations (`FluentValidation`), and Handlers.
4. **Mock and Test**: Before connecting Infrastructure, write Application unit tests with mock interfaces.
5. **Infrastructure Implementations**: Implement Data Access (EF Core Repositories/DbContext) and concrete services.
6. **API endpoints**: Expose logic via simple Web API controllers that dispatch commands via DI.

## Output Format
Provide an itemized list of the files you have created or modified. Conclude your response by invoking the `dotnet build` and `dotnet test` commands to prove the code compiles and passes unit tests.
