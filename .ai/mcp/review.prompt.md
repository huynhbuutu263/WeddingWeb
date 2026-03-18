# Review Prompt

## Objective
To rigorously evaluate the completed implementation against the assigned task's Acceptance Criteria and architectural rules.

## Trigger
Activates when the agent believes the Execution Phase is completely finished and all tests are passing green.

## Core Rules
- If ANY acceptance criteria are missing, you must fail the review.
- If Domain contains framework `using` statements (like EF Core), you must fail the review.
- If Application queries bypass the repository/DbContext and execute unsafe logic, you must fail the review.

## Workflow
1. **Acceptance Criteria Verification**:
   - Go line-by-line through the "Acceptance Criteria" listed in `.ai/tasks/task_XX.md`.
   - Explicitly verify that each point is addressed and mathematically/logically proven.
2. **Clean Architecture Check**:
   - Verify dependency direction (API -> Infrastructure -> Application -> Domain).
3. **Test Coverage Analysis**:
   - Check if Unit Tests mapping to `tests/` namespace were correctly included.
   - Ensure edge cases described in the task are tested.
4. **Approval / Rejection**:
   - If Rejected: Pivot back to the Execute or Debug workflow.

## Output Format
Render a Markdown checklist matching the Task's Acceptance Criteria. Mark each item with an `[x]` if passed or `[ ]` if failed. Conclude with a final `STATUS: APPROVED` or `STATUS: REJECTED`.
