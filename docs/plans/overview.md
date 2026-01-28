# Text Adventure Engine - Implementation Plan

> **For Claude:** REQUIRED SUB-SKILL: Use superpowers:executing-plans to implement this plan task-by-task.

**Goal:** Build a fluent, SOLID text adventure engine as two NuGet packages (core + AI) with extensible DSL.

**Architecture:** Core engine uses design patterns (State, Command, Observer, Memento, Strategy, Factory, Decorator, Composite, Chain of Responsibility, Visitor, Facade, Flyweight, Proxy, Prototype, Template). All systems behind interfaces, registered via fluent GameBuilder. AI package adds Ollama-based command parsing via ICommandParser.

**Tech Stack:** C# (latest), .NET 10, xUnit, System.Text.Json

---

## Core Design Decisions

### TDD Approach: Green/Blue

Skip red phase. Write test → write code → test passes → refactor.

### Slice Workflow Rule (mandatory)

test → kod → test → refactor → test → commit → update sandbox → låt Marcus testa sandbox → fixa vid behov, korrigera tester, commit igen.

### Slice Workflow Rule (tooling + examples)

# Agent Instructions

Keep this workflow for every slice and do not skip steps:

1 - test
2 - implementera
3 - kopiera över koden från docs/examples för slicen till sandbox Program.cs
3.1 - se till att exemplet använder det som finns i slicen (inga extra dependencies etc) och allt som kan vara med i slicen från tidigare slices.
3.2 - Använd alla extensions som finns i projektet.
3.3 - Använd existerande kod från tidigare slices.
3.4 - Se till att koden är ren och snygg.
4.5 - Använd pattern matching i ifsatser för att göra koden mer fluent.
4.6 - använd metoder för att dela upp koden i mindre delar där det är möjligt.
4.7 - skriv snygg läsbar kod.
4.8 - börja alltid med intro (mål) och visa start-rummet.
4.9 - gör alltid auto-look efter lyckad go/move.
4.10 - använd extensions där det går; om kod upprepas för mycket, föreslå en extension.
4.11 - håll koden liten: använd =>-metoder, ternary, switch och pattern matching där det går.
4.12 - skriv alltid på brittisk engelska (även i dokumentation).
4.13 - nämn inte takeable/destroyable interaktiva saker i rumsbeskrivningar (om de inte är icke-takeable och icke-destroyable).
4 - Marcus testar
5 - anpassa om något inte är ok
6 - kopiera koden från sandbox Program.cs till docs/examples
7 - commit

### Fluent Consistency Rule

Om ett objekt får en property/metod/extension, ge motsvarande funktionalitet till närliggande objekt (t.ex. Item/Key/Door) så API:t förblir konsekvent och lättläst.

### Language Generic

Varje kommando ska kunna ha språk-specifika nyckelord (t.ex. "go" på engelska, "gå" på svenska). Detta möjliggörs genom att ICommandParser hanterar språk och mappar inmatning till ICommand-objekt.

Finns det bättre sätt att göra detta?
Kom med förslag.

### Language System

- Single language loaded at runtime via file (e.g., `gamelang.en.txt`, `gamelang.sv.txt`)
- Default: English
- All game text comes from loaded language file

### Bi-directional Exits

Creating `hall.AddExit(Direction.North, bedroom)` automatically creates `bedroom.AddExit(Direction.South, hall)`.
Can be disabled per-exit for one-way passages.

### Doors

- Exits can have doors (IDoor)
- Doors can be: Open, Closed, Locked, Destroyed
- Locked doors require IKey
- Doors trigger events: OnOpen, OnClose, OnLock, OnUnlock, OnDestroy

### Items/Objects

- `Takeable: bool` — can player pick it up?
- `Weight: float` — optional, for inventory limits
- Containers: items can contain other items (Composite)
- Type constraints: `Glass : IContainer<IFluid>` — glass only holds fluids
- Combinations: `ice + fire → destroy both, create water`
- Events: OnTake, OnDrop, OnUse, OnOpen, OnClose, OnDestroy

### Inventory Limits

Configurable: by weight, by count, or unlimited.

### NPCs

- State: Friendly, Hostile, Dead, etc.
- Movement: None, Random, Patrol (pattern), Follow
- NPCs move between locations on game ticks

---

## Project Structure

```
C:\git\MarcusMedina\TextAdventure\
├── src\
│   ├── MarcusMedina.TextAdventure\           (core NuGet - engine + DSL)
│   └── MarcusMedina.TextAdventure.AI\        (AI NuGet - Ollama facade)
├── tests\
│   └── MarcusMedina.TextAdventure.Tests\     (xUnit tests)
├── sandbox\
│   └── TextAdventure.Sandbox\               (console app - testbädd)
├── docs\
│   └── plans\
└── TextAdventure.slnx
```

---
