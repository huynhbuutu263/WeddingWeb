# Task Execution Skill

Purpose: Force sequential, methodical AI task execution.
Scope: Terminal commands, task reading.

Rules:
- Execute ONE task at a time.
- Stop immediately if the current task's Acceptance Criteria fail.
- Do NOT edit files outside the current task's scope.

Workflow:
1. Read Next Task ID and Target Files.
2. Write tests.
3. Write implementation.
4. Run standard compiler and test commands (like `dotnet test`).
5. Report completion ONLY if tests pass and Acceptance Criteria are met.

Examples:
- Report: "TSK-003 Complete. `dotnet test` passed. Acceptance Criteria met. Awaiting user command."
