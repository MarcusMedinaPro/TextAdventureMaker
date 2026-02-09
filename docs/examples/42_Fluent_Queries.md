# Fluent Queries

_Slice tag: Slice 42 — LINQ vs Story fluent extensions._

## Story beats (max ~10 steps)
1) Define a small inventory.
2) Query with LINQ-style extensions.
3) Query with Story-style extensions.

## Example (fluent queries)
```csharp
using System;
using System.Collections.Generic;
using MarcusMedina.TextAdventure.Extensions;
using MarcusMedina.TextAdventure.Models;
using MarcusMedina.TextAdventure.Linq;
using MarcusMedina.TextAdventure.Story;
using static MarcusMedina.TextAdventure.Extensions.ConsoleExtensions;

// Slice 42 — Fluent LINQ/Story Extensions
// Tests:
// - LINQ-style query extensions
// - Story-style narrative extensions

List<Item> items =
[
    new Item("torch", "torch", "A steady torch.") { Takeable = true },
    new Item("coin", "coin", "A silver coin.") { Takeable = true },
    new Item("statue", "statue", "A heavy statue.") { Takeable = false }
];

SetupC64("Fluent Queries - Text Adventure Sandbox");

var takeable = items.Where(item => item.Takeable).Select(item => item.Name).ToList();
WriteLineC64($"LINQ: takeable items = {takeable.CommaJoin()}");

var storyTakeable = items.ThatMatch(item => item.Takeable).TheirNames().Gathered();
WriteLineC64($"Story: takeable items = {storyTakeable.CommaJoin()}");

var hasCoin = items.ThatMatch(item => item.Id.TextCompare("coin")).ThereAreAny();
WriteLineC64(hasCoin.Then("Story: there is a coin.").Else("Story: no coin found."));
```
