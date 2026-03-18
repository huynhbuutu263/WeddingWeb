# Debug Prompt

## Objective
To systematically resolve compilation failures, runtime exceptions, or test failures through a structured reasoning loop.

## Trigger
Activates automatically whenever `dotnet build` or `dotnet test` returns an error, or when the user reports a bug in the implementation.

## Core Rules
- Never blindly guess or arbitrarily change types.
- Always cross-reference interface definitions with concrete implementations.
- Do not suppress errors or write empty catch blocks just to make tests pass.

## Workflow
1. **Understand the Output**: Read the entire stack trace or test error terminal output. Identify the exact line of failure.
2. **Categorize the Error**:
   - *Dependency Injection*: Usually an unregistered service or a circular dependency in the API layer.
   - *Architecture Violation*: e.g., Domain referencing Infrastructure.
   - *Test Mocking Issue*: Your Mock/Substitute formulation doesn't match the active implementation.
   - *Syntax Error*: Missing brackets, incorrect C# 12+ features.
3. **Verify Hypothesis**: Investigate the surrounding code to ensure your assumption is correct.
4. **Implement Fix Iteratively**: Apply your fix to one component at a time.
5. **Verify**: Immediately run `dotnet build` or `dotnet test` again.

## Output Format
State the "Root Cause" identified from the logs, followed by the "Fix Applied". If the fix works, explain how it guarantees the issue won't reappear.
