## Slice 51: Directional Actions

**Mål:** Spelaren kan utföra actions mot angränsande rum utan att gå dit.

**Referens:** `docs/plans/imported/Directional_Actions_System.md`

### Kommandon
```
throw wallet north   → Kasta föremål in i nästa rum
shout east           → Ropa mot angränsande rum
push table north     → Blockera utgång med möbel
listen south         → Lyssna på ljud från angränsande rum
```

### Task 51.1: IDirectionalAction interface

```csharp
public interface IDirectionalAction
{
    Direction Direction { get; }
    IItem? UsedItem { get; }
    CommandResult Execute(CommandContext context);
}
```

### Task 51.2: ThrowCommand

```csharp
public class ThrowCommand(string itemName, Direction direction) : ICommand
{
    public CommandResult Execute(CommandContext context)
    {
        var item = context.State.Inventory.FindItem(itemName);
        if (item == null)
            return CommandResult.Fail($"You don't have a {itemName}.");

        if (!item.GetProperty<bool>("throwable", true))
            return CommandResult.Fail($"You can't throw the {item.Name}.");

        var location = context.State.CurrentLocation;
        if (!location.Exits.TryGetValue(direction, out var exit))
            return CommandResult.Fail($"There's nowhere to throw to the {direction}.");

        // Kolla om vägen är blockerad
        if (exit.Door != null && exit.Door.State == DoorState.Closed)
            return CommandResult.Ok($"The {item.Name} bounces off the closed door.");

        // Flytta item till target location
        context.State.Inventory.Remove(item);
        exit.Target.Items.Add(item);

        // Event för NPCs i target room
        context.State.RaiseEvent("item_thrown", new { Item = item, Direction = direction });

        return CommandResult.Ok($"You throw the {item.Name} {direction}. It lands in the {exit.Target.Name}.");
    }
}
```

### Task 51.3: ShoutCommand

```csharp
public class ShoutCommand(Direction? direction, string? message = null) : ICommand
{
    public CommandResult Execute(CommandContext context)
    {
        var location = context.State.CurrentLocation;
        var heardBy = new List<INpc>();

        if (direction.HasValue)
        {
            // Riktat rop
            if (location.Exits.TryGetValue(direction.Value, out var exit))
            {
                heardBy.AddRange(exit.Target.Npcs.Where(n => n.IsVisible));
            }
        }
        else
        {
            // Rop i alla riktningar
            foreach (var exit in location.Exits.Values)
            {
                heardBy.AddRange(exit.Target.Npcs.Where(n => n.IsVisible));
            }
        }

        if (heardBy.Any())
        {
            context.State.RaiseEvent("shout_heard", new { Npcs = heardBy, Message = message });
            return CommandResult.Ok($"Your shout echoes. {heardBy.Select(n => n.Name).CommaJoin()} heard you.");
        }

        return CommandResult.Ok("Your voice echoes, but no one seems to hear.");
    }
}
```

### Task 51.4: ListenCommand

```csharp
public class ListenCommand(Direction? direction = null) : ICommand
{
    public CommandResult Execute(CommandContext context)
    {
        var location = context.State.CurrentLocation;
        var sounds = new List<string>();

        IEnumerable<KeyValuePair<Direction, Exit>> exits = direction.HasValue
            ? location.Exits.Where(e => e.Key == direction.Value)
            : location.Exits;

        foreach (var (dir, exit) in exits)
        {
            // NPCs som pratar
            var talkingNpcs = exit.Target.Npcs.Where(n => n.GetProperty<bool>("talking", false));
            foreach (var npc in talkingNpcs)
                sounds.Add($"You hear voices from the {dir}.");

            // Ambient sounds
            var ambient = exit.Target.GetProperty<string>("ambient_sound");
            if (!string.IsNullOrEmpty(ambient))
                sounds.Add($"From the {dir}: {ambient}");
        }

        return sounds.Any()
            ? CommandResult.Ok(string.Join("\n", sounds))
            : CommandResult.Ok("You strain your ears but hear nothing unusual.");
    }
}
```

### Task 51.5: Parser-integration

```csharp
// I KeywordParserConfig:
config.AddKeyword("throw", (args, ctx) => ParseThrowCommand(args));
config.AddKeyword("shout", (args, ctx) => ParseShoutCommand(args));
config.AddKeyword("yell", (args, ctx) => ParseShoutCommand(args));
config.AddKeyword("listen", (args, ctx) => new ListenCommand(ParseDirection(args)));

private static ICommand ParseThrowCommand(string args)
{
    // "throw wallet north" → item=wallet, direction=north
    var parts = args.Split(' ');
    if (parts.Length >= 2 && DirectionHelper.TryParse(parts[^1], out var dir))
        return new ThrowCommand(string.Join(" ", parts[..^1]), dir);
    return new UnknownCommand($"throw {args}");
}
```

### Task 51.6: NPC-reaktioner på directional actions

```csharp
// NPCs reagerar på kastade föremål
npc.On("item_thrown", (ctx, args) =>
{
    if (args.Item.Name == "wallet")
    {
        npc.SetProperty("distracted", true);
        return "The robber chases after the wallet!";
    }
    return null;
});
```

### Task 51.7: Tester

```csharp
[Fact]
public void ThrowWalletNorth_MovesItemToTargetRoom()
{
    var game = CreateGame();
    game.State.Inventory.Add(new Item("wallet"));

    var result = game.Execute("throw wallet north");

    Assert.True(result.Success);
    Assert.DoesNotContain(game.State.Inventory.Items, i => i.Name == "wallet");
    Assert.Contains(game.State.Locations["north_room"].Items, i => i.Name == "wallet");
}
```

### Task 51.8: Sandbox — rånare-scenario

Demo där spelaren kan kasta plånbok för att distrahera rånare, eller ropa på hjälp.

---

## Completion Checklist
- [x] Core implementation for this slice is present in the engine.
- [x] Behaviour is covered by tests and/or deterministic validation paths.
- [x] Demo document: `51_The_Corridor_Levers.md`.
- [x] Marked complete in project slice status.
