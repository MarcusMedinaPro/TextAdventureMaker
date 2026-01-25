# Slice Stories Map

This document maps slices to demo stories and explains why each story fits the engine feature. It also double-checks existing demos (Slice 1–13) against their intended goals.

## Legend
- **Status:** OK (demo already demonstrates the feature) / Needs tweak (small adjustment recommended)
- **Story file:** points to the current example in `docs/examples/` when it exists

---

## Slices 1–13 (existing demos)

1) **Slice 1 — Location & Navigation**
- **Story:** Morning Ritual
- **Story file:** `docs/examples/01_Morning_Ritual.md`
- **Why it fits:** clean movement between rooms with `go` and `look`
- **Status:** OK

2) **Slice 2 — Doors & Keys (Stateful exits)**
- **Story:** The Locked Drawer
- **Story file:** `docs/examples/02_The_Locked_Drawer.md`
- **Why it fits:** locked drawer modeled as a door with key requirement
- **Status:** OK

3) **Slice 3 — Command Pattern + Parser**
- **Story:** Light in the Basement
- **Story file:** `docs/examples/03_Light_in_the_Basement.md`
- **Why it fits:** parser drives command execution; world state reacts to item pickup
- **Status:** OK (optional tweak: add more synonyms like "switch on", "turn on")

4) **Slice 4 — Items & Inventory (Core)**
- **Story:** The Last Train Home
- **Story file:** `docs/examples/04_The_Last_Train_Home.md`
- **Why it fits:** pick up a ticket and branch the outcome
- **Status:** OK

5) **Slice 5 — NPCs + Dialogue + Movement**
- **Story:** The Silent Classroom
- **Story file:** `docs/examples/05_The_Silent_Classroom.md`
- **Why it fits:** simple NPC dialog tree in a single location
- **Status:** OK

6) **Slice 6 — Event System (Observer)**
- **Story:** The Key Under the Stone
- **Story file:** `docs/examples/06_The_Key_Under_the_Stone.md`
- **Why it fits:** pickup triggers event, spawns key
- **Status:** OK

7) **Slice 7 — Combat System (Strategy)**
- **Story:** Rain on the Roof
- **Story file:** `docs/examples/07_Rain_on_the_Roof.md`
- **Why it fits:** uses combat flow as a "struggle" abstraction
- **Status:** OK

8) **Slice 8 — Quest System**
- **Story:** The Forgotten Password
- **Story file:** `docs/examples/08_The_Forgotten_Password.md`
- **Why it fits:** quest completion gates the login action
- **Status:** OK

9) **Slice 9 — World State & Consequence**
- **Story:** Predator (Pre‑Date‑Or)
- **Story file:** `docs/examples/09_Predator_Pre-Date-Or.md`
- **Why it fits:** flags reflect choices and change outcomes
- **Status:** OK

10) **Slice 10 — Save/Load (Memento)**
- **Story:** The Warm Library
- **Story file:** `docs/examples/10_The_Warm_Library.md`
- **Why it fits:** safe, small world that highlights save/restore
- **Status:** OK

11) **Slice 11 — Language Provider**
- **Story:** Before the Meeting
- **Story file:** `docs/examples/11_Before_the_Meeting.md`
- **Why it fits:** swaps UI messages via language file
- **Status:** OK

12) **Slice 12 — DSL Parser (.adventure)**
- **Story:** Clockwork Dock
- **Story file:** `docs/examples/12_Clockwork_Dock.md`
- **Why it fits:** whole world loaded from DSL
- **Status:** OK

13) **Slice 13 — GameBuilder + Game Loop (NPC ticks)**
- **Story:** Midnight Studio
- **Story file:** `docs/examples/13_Midnight_Studio.md`
- **Why it fits:** fluent setup + NPC movement tick
- **Status:** OK

---

## Slices 14–40 (new story list)

14) **Slice 14 — The Key Under the Stone**
- **Story:** The Key Under the Stone
- **Why it fits:** compact trigger/reveal loop for a single mechanic

15) **Slice 15 — Rain on the Roof**
- **Story:** Rain on the Roof
- **Why it fits:** steady, low-stakes loop ideal for environment effects

16) **Slice 16 — The Forgotten Password**
- **Story:** The Forgotten Password
- **Why it fits:** clear objective + gate checks

17) **Slice 17 — The Warm Library**
- **Story:** The Warm Library
- **Why it fits:** calm setting for progression and persistence

18) **Slice 18 — Before the Meeting**
- **Story:** Before the Meeting
- **Why it fits:** short, reflective scene good for tone/locale

19) **Slice 19 — First Date at the Café**
- **Story:** First Date at the Café
- **Why it fits:** dialog choices with light consequence

20) **Slice 20 — Hints & Properties**
- **Story:** Hints & Properties
- **Why it fits:** metadata + dynamic hints on items, keys, doors, NPCs

21) **Slice 21 — The Bank Errand**
- **Story:** The Bank Errand
- **Why it fits:** small transactional flow with state checks

22) **Slice 22 — Street Robbery**
- **Story:** Street Robbery
- **Why it fits:** simple threat flow with branching outcomes

23) **Slice 23 — Job Interview**
- **Story:** Job Interview
- **Why it fits:** structured dialog steps with success/fail states

24) **Slice 24 — Missed Train**
- **Story:** Missed Train
- **Why it fits:** path choice + alternate route options

25) **Slice 25 — The Hospital Visit**
- **Story:** The Hospital Visit
- **Why it fits:** staged access, check-in, and result flow

26) **Slice 26 — The Broken Car**
- **Story:** The Broken Car
- **Why it fits:** chain of dependencies (tools, phone, fix)

27) **Slice 27 — Apartment Viewing**
- **Story:** Apartment Viewing
- **Why it fits:** questions + reveal loop

28) **Slice 28 — The Bar Fight**
- **Story:** The Bar Fight
- **Why it fits:** escalation + de-escalation choice

29) **Slice 29 — The Night Walk Home**
- **Story:** The Night Walk Home
- **Why it fits:** tension + risk assessment beats

30) **Slice 30 — The Lost Dog**
- **Story:** The Lost Dog
- **Why it fits:** multi-location follow + rescue + payoff

31) **Slice 31 — The Locked Classroom**
- **Story:** The Locked Classroom
- **Why it fits:** navigation + alarm constraint

32) **Slice 32 — The Old Photograph**
- **Story:** The Old Photograph
- **Why it fits:** clue chain to meaning

33) **Slice 33 — The Elevator Stuck**
- **Story:** The Elevator Stuck
- **Why it fits:** limited space, stress, and communication

34) **Slice 34 — The Power Outage**
- **Story:** The Power Outage
- **Why it fits:** light/dark gating + sequential fixes

35) **Slice 35 — The Midnight Phone Call**
- **Story:** The Midnight Phone Call
- **Why it fits:** remote dialog + branching response

36) **Slice 36 — The Package at the Door**
- **Story:** The Package at the Door
- **Why it fits:** trust/choice with consequence

37) **Slice 37 — The Abandoned Playground**
- **Story:** The Abandoned Playground
- **Why it fits:** atmosphere and delayed reveals

38) **Slice 38 — The Late Bus**
- **Story:** The Late Bus
- **Why it fits:** waiting loop + alternative decision

39) **Slice 39 — The Forgotten Lunchbox**
- **Story:** The Forgotten Lunchbox
- **Why it fits:** memory loop + retrieval

40) **Slice 40 — The Storm Shelter**
- **Story:** The Storm Shelter
- **Why it fits:** group safety + resource pacing

41) **Slice 41 — The Final Light**
- **Story:** The Final Light
- **Why it fits:** calm closure + full-system integration
