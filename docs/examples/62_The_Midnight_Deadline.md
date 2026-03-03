# The Midnight Deadline

_Slice tag: Slice 62 — Demonstration scenario for The Midnight Deadline._

## Demo Purpose
Show the slice behaviour in a compact, testable scenario focused on: countdown/deadline events and fail-state handling.

## Scenario Outline
1. Start in a controlled room setup with clear objective text.
2. Trigger the primary mechanic introduced in this slice.
3. Trigger one edge case/failure case.
4. Confirm state changes and output text are consistent.

## What To Verify
- Primary path works as expected.
- Failure/guard path produces correct feedback.
- State/log/debug output reflects the transition.

## Notes
- Keep language and output in British English.
- Prefer deterministic setup values for repeatable checks.
