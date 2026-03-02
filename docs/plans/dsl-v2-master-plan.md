# DSL v2 Master Plan (Slices 073-093)

Purpose: one execution view for the full DSL v2 upgrade track, from parser foundations to release and post-GA governance.

Scope:
- Core (`src/MarcusMedina.TextAdventure`)
- DSLHelper (`src/MarcusMedina.TextAdventure.DSLHelper`)
- Docs/fixtures/CI

Out of scope:
- AI-generated content workflows (can be layered on top later)

## Phase 1: Foundation and Data Model

Goal: build safe parser/runtime foundations without breaking v1.

Slices:
1. `slice073.md` - entities, rich item schema, start-state
2. `slice074.md` - item reactions, consequences, recipes
3. `slice075.md` - interpolation + safe expression support

Exit criteria:
- v1 adventures still parse and run.
- v2 features are parseable and validated.
- start state can be authored in DSL.

## Phase 2: World Interaction and NPC Layer

Goal: move world and character logic from C# wiring into DSL.

Slices:
1. `slice076.md` - doors/exits, dynamic rooms, transforms
2. `slice077.md` - NPC base + acceptance ladders
3. `slice078.md` - NPC rules/triggers/dialog options

Exit criteria:
- room/door/NPC behaviour can be authored declaratively.
- acceptance thresholds and trigger logic run reliably.

## Phase 3: Progression and Automation

Goal: cover quests, events, schedules, and randomness.

Slices:
1. `slice079.md` - quest DSL + condition graph
2. `slice080.md` - event/schedule/random automation
3. `slice083.md` - story branches + chapter DSL

Exit criteria:
- progression systems are content-driven from DSL.
- event automation works with shared condition/effect runtime.

## Phase 4: File Architecture and Export/Import

Goal: support world/state split and stable round-trip flows.

Slices:
1. `slice081.md` - two-file architecture (world + state/save)
2. `slice085.md` - v2 exporter and round-trip fidelity
3. `slice087.md` - save snapshot completeness

Exit criteria:
- load order and two-file model are stable.
- save/export/import keeps gameplay-critical state.

## Phase 5: Tooling, Validation, and Migration

Goal: ship production-grade authoring and migration workflows.

Slices:
1. `slice082.md` - validator + DSLHelper CRUD baseline
2. `slice088.md` - advanced CRUD/refactor workflows
3. `slice089.md` - v1->v2 migration tooling
4. `slice090.md` - fixture corpus + CI quality gates

Exit criteria:
- content team can author/update/validate DSL without Core coding.
- migration path from v1 is automated and auditable.

## Phase 6: Parser Config, Hardening, and Release

Goal: operational robustness and release readiness.

Slices:
1. `slice084.md` - parser/command settings in DSL
2. `slice086.md` - condition/effect runtime hardening
3. `slice091.md` - pure DSL v2 demo adventure
4. `slice092.md` - release/versioning/deprecation
5. `slice093.md` - post-GA governance and v2.x backlog

Exit criteria:
- demo proves end-to-end viability.
- release gates and compatibility policy are clear.
- governance model exists for controlled schema evolution.

## Dependency Highlights

Hard dependencies:
1. `073` before `074`, `076`, `077`, `081`
2. `075` before `077`, `080`, `084`, `086`
3. `079` before `083`
4. `081` before `085` and `087`
5. `082` before `088`, `089`, `090`
6. `090` before `092`
7. `091` before `092`

Soft dependencies:
1. `084` can be parallelised after `075`.
2. `083` can start once `079` condition graph is stable.
3. `086` can start early once condition/effect AST exists.

## Suggested Delivery Batches

Batch A (Core DSL usability):
1. `073`
2. `074`
3. `076`
4. `077`

Batch B (Logic richness):
1. `075`
2. `078`
3. `079`
4. `080`

Batch C (State/save and export):
1. `081`
2. `085`
3. `087`

Batch D (Tooling + migration):
1. `082`
2. `088`
3. `089`
4. `090`

Batch E (Release track):
1. `083`
2. `084`
3. `086`
4. `091`
5. `092`
6. `093`

## Definition of Plan Complete

Plan is complete when:
1. All slices `073-093` have acceptance criteria and dependencies clear.
2. The index links all slices and this master plan.
3. A team can execute slices without needing extra architecture decisions.
