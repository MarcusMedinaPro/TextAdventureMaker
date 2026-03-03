# What Did You Mean

_Slice tag: Slice 57 — Demonstration scenario for What Did You Mean._

## Demo Purpose
Show the slice behaviour in a compact, testable scenario focused on: semantic parser intent extraction and disambiguation.

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
