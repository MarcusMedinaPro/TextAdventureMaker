## Slice 50: Directional Looking

**Mål:** Spelaren kan titta i en riktning och se in i angränsande rum genom öppna dörrar/passager.

**Referens:** `docs/plans/imported/Directional_Looking_System.md`

### Kommandon
```
look north    → Kika in i rummet norrut
look door     → Undersök dörren/passagen
peek west     → Försiktig titt västerut
```

### Task 50.1: Utöka LookCommand med direction-stöd

```csharp
// I LookCommand.ExecuteTarget, lägg till direction-check:
if (DirectionHelper.TryParse(target, out var direction))
{
    return LookInDirection(context, direction);
}
```

### Task 50.2: LookInDirection-metod

```csharp
private static CommandResult LookInDirection(CommandContext context, Direction direction)
{
    var location = context.State.CurrentLocation;

    if (!location.Exits.TryGetValue(direction, out var exit))
        return CommandResult.Fail($"There is no exit to the {direction}.");

    // Stängd dörr blockerar sikt
    if (exit.Door != null && exit.Door.State != DoorState.Open)
        return CommandResult.Ok($"A {exit.Door.State.ToString().ToLower()} {exit.Door.Name} blocks your view.");

    // Kan se in i nästa rum
    var targetLocation = exit.Target;
    var glimpse = GetRoomGlimpse(targetLocation);
    return CommandResult.Ok($"Looking {direction}, you see:\n{glimpse}");
}
```

### Task 50.3: GetRoomGlimpse - kort beskrivning av angränsande rum

```csharp
private static string GetRoomGlimpse(ILocation location)
{
    var sb = new StringBuilder();
    sb.AppendLine(location.ShortDescription ?? location.Name);

    // Synliga objekt (stora/uppenbara)
    var visibleItems = location.Items
        .Where(i => !i.HiddenFromItemList && i.GetProperty<bool>("prominent", false))
        .Take(3);

    if (visibleItems.Any())
        sb.AppendLine($"You can make out: {visibleItems.Select(i => i.Name).CommaJoin()}.");

    // Synliga NPCs
    var npcs = location.Npcs.Where(n => n.IsVisible).Take(2);
    if (npcs.Any())
        sb.AppendLine($"You see: {npcs.Select(n => n.Name).CommaJoin()}.");

    return sb.ToString().Trim();
}
```

### Task 50.4: Visibility-faktorer (framtida utökning)

```csharp
public interface IVisibilityModifier
{
    float Factor { get; }           // 0.0-1.0
    string Description { get; }     // "through frosted glass"
}

// Exit-properties:
exit.SetProperty("transparent", true);      // Glas, galler
exit.SetProperty("visibility", 0.3f);       // Frostat glas
```

### Task 50.5: Parser-stöd för "peek"

```csharp
// I KeywordParserConfig:
config.AddKeyword("peek", (args, ctx) => new LookCommand(args));
```

### Task 50.6: Tester

```csharp
[Fact]
public void LookNorth_WithOpenDoor_ShowsTargetRoom()
{
    var game = CreateGame();
    var result = game.Execute("look north");

    Assert.True(result.Success);
    Assert.Contains("Living Room", result.Message);
}

[Fact]
public void LookNorth_WithClosedDoor_ShowsDoorBlocking()
{
    var game = CreateGame();
    game.State.CurrentLocation.Exits[Direction.North].Door.State = DoorState.Closed;

    var result = game.Execute("look north");

    Assert.Contains("closed", result.Message.ToLower());
}
```

### Task 50.7: Sandbox — hus med fönster och dörrar

Skapa demo där spelaren kan kika genom öppningar innan de går in.

---

## Completion Checklist
- [x] Core implementation for this slice is present in the engine.
- [x] Behaviour is covered by tests and/or deterministic validation paths.
- [x] Demo document: `50_The_Laundry_Room_Warning.md`.
- [x] Marked complete in project slice status.
