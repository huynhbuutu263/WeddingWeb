# Testing Skill

Purpose: Guarantee correctness using xUnit.
Scope: Unit tests (Domain/App), Integration tests (API).

Rules:
- Use Arrange-Act-Assert (AAA) pattern.
- Format: `MethodName_StateUnderTest_ExpectedBehavior`.
- MOCK ONLY Infrastructure interfaces, NEVER Domain entities.
- Use `WebApplicationFactory` for all API integration tests.

Workflow:
1. Write failing test (Red).
2. Write minimum viable implementation.
3. Pass test (Green).
4. Refactor.

Examples:
- Good: `public async Task Handle_SlugIsNotUnique_ThrowsValidationException()`
- Bad: `public void Test1()`
