# Dual-AI Execution Workflow (Antigravity + Copilot)

This document defines the highly efficient cooperative workflow between **Antigravity** (Architect, Orchestrator, & Reviewer) and **GitHub Copilot** (Code Generator & Typist) on the `WeddingApp` project.

## Division of Labor
- **Antigravity (Me)**: Handles project context, reads tasks securely, scaffolds files, runs `dotnet build`/`dotnet test`, debugs complex architecture errors, and audits acceptance criteria.
- **GitHub Copilot**: Handles rapid inline code generation (C# language server awareness, LINQ queries, boilerplate handlers) within the files Antigravity sets up.
- **You (The Human)**: The bridge and executive decision-maker.

---

## State 1: Initialization & Scaffold (Antigravity)
**Trigger**: You say, *"Let's start Task 1"*
1. Antigravity loads `.ai/tasks/task_XX.md` and `.ai/mcp/system.prompt.md`.
2. Antigravity outlines the exact Clean Architecture boundary (Domain -> App -> Infra -> API).
3. Antigravity creates any missing files and sets up the interfaces or basic class structures.

## State 2: Rapid Generation (Copilot + You)
**Trigger**: Antigravity sets up the files.
1. You step into the source code files.
2. You use Copilot inline generation (`Ctrl+I` / `Cmd+I`) or standard autocomplete to rapidly flesh out the methods, MediatR handlers, and DbContext queries.
3. *Pro-Tip*: Give Copilot comments like `// Implement interface according to Clean Architecture rules` and let it autocomplete the boilerplate based on its language awareness.

## State 3: Verification & Debugging (Antigravity)
**Trigger**: You say, *"Code generated. Verify it."*
1. Antigravity runs `dotnet build` and your xUnit tests.
2. If errors occur, Antigravity reads the stack trace (using `.ai/mcp/debug.prompt.md`). It analyzes whether it's a DI missing mapping or syntax error, and immediately implements the fix or instructs you on what to tweak.

## State 4: Review & Audit (Antigravity)
**Trigger**: All tests go green.
1. Antigravity performs an independent audit against the `.ai/mcp/review.prompt.md` criteria to ensure the active task (`task_XX.md`) is completely fulfilled.
2. Antigravity generates a Markdown checklist validating the Acceptance Criteria.
3. Once approved by you, we commit and loop back to State 1.
