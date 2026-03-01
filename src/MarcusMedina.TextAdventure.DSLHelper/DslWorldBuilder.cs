using System.Text;
using MarcusMedina.TextAdventure.Dsl;
using MarcusMedina.TextAdventure.Engine;
using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Extensions;
using MarcusMedina.TextAdventure.Interfaces;
using MarcusMedina.TextAdventure.Models;

namespace MarcusMedina.TextAdventure.DSLHelper;

internal sealed class DslWorldBuilder
{
    private static readonly Direction[] DirectionPreference =
    [
        Direction.North,
        Direction.East,
        Direction.South,
        Direction.West,
        Direction.Up,
        Direction.Down,
        Direction.In,
        Direction.Out,
        Direction.NorthEast,
        Direction.NorthWest,
        Direction.SouthEast,
        Direction.SouthWest
    ];

    private static readonly IReadOnlyDictionary<string, Direction> DirectionAliases = new Dictionary<string, Direction>(StringComparer.OrdinalIgnoreCase)
    {
        ["north"] = Direction.North,
        ["n"] = Direction.North,
        ["south"] = Direction.South,
        ["s"] = Direction.South,
        ["east"] = Direction.East,
        ["e"] = Direction.East,
        ["west"] = Direction.West,
        ["w"] = Direction.West,
        ["up"] = Direction.Up,
        ["u"] = Direction.Up,
        ["down"] = Direction.Down,
        ["d"] = Direction.Down,
        ["in"] = Direction.In,
        ["out"] = Direction.Out,
        ["north_east"] = Direction.NorthEast,
        ["northeast"] = Direction.NorthEast,
        ["ne"] = Direction.NorthEast,
        ["north_west"] = Direction.NorthWest,
        ["northwest"] = Direction.NorthWest,
        ["nw"] = Direction.NorthWest,
        ["south_east"] = Direction.SouthEast,
        ["southeast"] = Direction.SouthEast,
        ["se"] = Direction.SouthEast,
        ["south_west"] = Direction.SouthWest,
        ["southwest"] = Direction.SouthWest,
        ["sw"] = Direction.SouthWest
    };

    private readonly AdventureDslExporter _exporter = new();
    private readonly Dictionary<string, Location> _locations = new(StringComparer.OrdinalIgnoreCase);

    public DslWorldBuilder()
    {
        Location start = new("room", "You are in a room.");
        _locations[start.Id] = start;
        State = new GameState(start, worldLocations: [start]);
    }

    public GameState State { get; }
    public string WorldTitle { get; set; } = "Untitled Adventure";
    public string Goal { get; set; } = "Build a world and survive what waits inside it.";
    public Location CurrentLocation => (Location)State.CurrentLocation;

    public IReadOnlyList<string> GetCurrentRoomLines()
    {
        Location current = CurrentLocation;
        string roomName = string.IsNullOrWhiteSpace(current.Name) ? IdToDisplayName(current.Id) : current.Name.ToProperCase();
        string description = string.IsNullOrWhiteSpace(current.GetDescription())
            ? "An unfinished room without a proper description."
            : current.GetDescription();

        List<string> items = current.GetRoomItems();
        List<string> npcs = current.GetRoomNpcs();
        List<string> exits = current.GetRoomExits();

        return
        [
            string.Empty,
            $"Room: {roomName}",
            description,
            items.Count == 0 ? "You can see nothing." : $"Items: {items.CommaJoin()}",
            npcs.Count == 0 ? "People: none." : $"People: {npcs.CommaJoin()}",
            exits.Count == 0 ? "Exits: none." : $"Exits: {exits.CommaJoin()}"
        ];
    }

    public IReadOnlyList<string> ListRooms() =>
        [.. _locations.Keys
            .OrderBy(static key => key, StringComparer.OrdinalIgnoreCase)
            .Select(id => id.TextCompare(CurrentLocation.Id) ? $"{id} (current)" : id)];

    public IReadOnlyList<string> ListItems(string? roomIdRaw = null)
    {
        if (string.IsNullOrWhiteSpace(roomIdRaw))
        {
            List<string> lines =
            [
                .. _locations.Values
                    .OrderBy(static room => room.Id, StringComparer.OrdinalIgnoreCase)
                    .SelectMany(room => room.Items.Select(item => $"{item.Id} ({item.Name}) in {room.Id}"))
            ];

            return lines.Count == 0 ? ["No items found."] : lines;
        }

        string roomId = ResolveRoomId(roomIdRaw);
        if (!_locations.TryGetValue(roomId, out Location? room))
            return [$"Room '{roomId}' does not exist."];

        List<string> roomItems = [.. room.Items.Select(item => $"{item.Id} ({item.Name})")];
        return roomItems.Count == 0 ? [$"No items in room '{room.Id}'."] : roomItems;
    }

    public IReadOnlyList<string> ListNpcs(string? roomIdRaw = null)
    {
        if (string.IsNullOrWhiteSpace(roomIdRaw))
        {
            List<string> lines =
            [
                .. _locations.Values
                    .OrderBy(static room => room.Id, StringComparer.OrdinalIgnoreCase)
                    .SelectMany(room => room.Npcs.Select(npc => $"{npc.Id} ({npc.Name}) in {room.Id}"))
            ];

            return lines.Count == 0 ? ["No NPCs found."] : lines;
        }

        string roomId = ResolveRoomId(roomIdRaw);
        if (!_locations.TryGetValue(roomId, out Location? room))
            return [$"Room '{roomId}' does not exist."];

        List<string> roomNpcs = [.. room.Npcs.Select(npc => $"{npc.Id} ({npc.Name})")];
        return roomNpcs.Count == 0 ? [$"No NPCs in room '{room.Id}'."] : roomNpcs;
    }

    public IReadOnlyList<string> ListDoors()
    {
        HashSet<string> seen = new(StringComparer.OrdinalIgnoreCase);
        List<string> lines = [];

        foreach (Location room in _locations.Values.OrderBy(static x => x.Id, StringComparer.OrdinalIgnoreCase))
        {
            foreach ((Direction direction, Exit exit) in room.Exits.OrderBy(static x => x.Key))
            {
                if (exit.Door is not Door door || !seen.Add(door.Id))
                    continue;

                lines.Add($"{door.Id} ({door.Name}): {room.Id} -> {exit.Target.Id} [{direction.ToString().Lower()}]");
            }
        }

        return lines.Count == 0 ? ["No doors found."] : lines;
    }

    public string BuildPromptContext()
    {
        StringBuilder context = new();
        _ = context.AppendLine($"Current room: {CurrentLocation.Id}");
        _ = context.AppendLine($"World title: {WorldTitle}");
        _ = context.AppendLine($"Goal: {Goal}");
        _ = context.AppendLine("Known rooms:");

        foreach (Location room in _locations.Values.OrderBy(static x => x.Id, StringComparer.OrdinalIgnoreCase))
        {
            string exits = room.Exits.Count == 0
                ? "none"
                : string.Join(", ", room.Exits.Select(x => $"{x.Key.ToString().Lower()}->{x.Value.Target.Id}"));
            string items = room.Items.Count == 0
                ? "none"
                : room.Items.Select(static x => x.Id).CommaJoin();
            string npcs = room.Npcs.Count == 0
                ? "none"
                : room.Npcs.Select(static x => x.Id).CommaJoin();
            _ = context.AppendLine($"- {room.Id}; desc={room.GetDescription()}; exits={exits}; items={items}; npcs={npcs}");
        }

        return context.ToString();
    }

    public string CreateRoom(string? roomIdRaw, string? description)
    {
        string roomId = roomIdRaw.ToId();
        if (string.IsNullOrWhiteSpace(roomId))
            return "Cannot create a room without an id.";

        if (_locations.TryGetValue(roomId, out Location? existing))
        {
            if (!string.IsNullOrWhiteSpace(description))
                existing.Description(EnsureSentence(description, existing.GetDescription()));
            return $"Room '{roomId}' already exists.";
        }

        string fallbackDescription = $"A room called {roomId.Replace('_', ' ')}.";
        Location room = new(roomId, EnsureSentence(description, fallbackDescription))
        {
            Name = IdToDisplayName(roomId)
        };
        _locations[roomId] = room;
        State.RegisterLocations([room]);
        return $"Created room '{roomId}'.";
    }

    public string AddDoorStrict(
        string? fromRoomIdRaw,
        string? toRoomIdRaw,
        string? directionRaw,
        string? doorIdRaw,
        string? doorName,
        string? doorDescription)
    {
        string fromRoomId = fromRoomIdRaw.ToId();
        string toRoomId = toRoomIdRaw.ToId();
        if (string.IsNullOrWhiteSpace(fromRoomId) || string.IsNullOrWhiteSpace(toRoomId))
            return "Door creation needs both source and target room ids.";

        if (!_locations.TryGetValue(fromRoomId, out Location? fromRoom))
            return $"Source room '{fromRoomId}' does not exist. Create it first.";
        if (!_locations.TryGetValue(toRoomId, out Location? toRoom))
            return $"Target room '{toRoomId}' does not exist. Create it first.";

        Direction? direction;
        Direction? requestedDirection = ParseDirection(directionRaw);
        if (requestedDirection is Direction explicitDirection)
        {
            if (fromRoom.Exits.ContainsKey(explicitDirection))
                return $"Direction '{explicitDirection.ToString().Lower()}' is already used from room '{fromRoom.Id}'.";

            direction = explicitDirection;
        }
        else
        {
            direction = GetAvailableDirection(fromRoom);
            if (direction is null)
                return $"No free exits left in room '{fromRoom.Id}'.";
        }

        string doorId = string.IsNullOrWhiteSpace(doorIdRaw)
            ? $"door_{fromRoom.Id}_{toRoom.Id}_{direction.Value.ToString().Lower()}"
            : doorIdRaw.ToId();
        if (string.IsNullOrWhiteSpace(doorId))
            return "Door id is required.";
        if (FindDoor(doorId).Door is not null)
            return $"Door '{doorId}' already exists.";

        string doorDisplayName = string.IsNullOrWhiteSpace(doorName)
            ? $"{IdToDisplayName(fromRoom.Id)} to {IdToDisplayName(toRoom.Id)} Door"
            : doorName.Trim();

        Door door = new(doorId, doorDisplayName);
        if (!string.IsNullOrWhiteSpace(doorDescription))
            _ = door.Description(EnsureSentence(doorDescription, "A plain wooden door."));

        _ = fromRoom.AddExit(direction.Value, toRoom, door);
        return $"Added door '{door.Id}' from {fromRoom.Id} to {toRoom.Id} via {direction.Value.ToString().Lower()}.";
    }

    public string AddItem(string? roomIdRaw, string? itemIdRaw, string? itemName, string? description)
    {
        string roomId = roomIdRaw.ToId();
        if (string.IsNullOrWhiteSpace(roomId))
            return "Item creation needs a room id.";
        if (!_locations.TryGetValue(roomId, out Location? room))
            return $"Room '{roomId}' does not exist. Create it first.";

        string itemId = string.IsNullOrWhiteSpace(itemIdRaw) ? itemName.ToId() : itemIdRaw.ToId();
        if (string.IsNullOrWhiteSpace(itemId))
            return "Item creation needs an item id or name.";
        if (FindItem(itemId).Item is not null)
            return $"Item '{itemId}' already exists.";

        string finalName = string.IsNullOrWhiteSpace(itemName) ? IdToDisplayName(itemId) : itemName.Trim();
        string finalDescription = string.IsNullOrWhiteSpace(description)
            ? $"A {finalName.ToLowerInvariant()}."
            : EnsureSentence(description, string.Empty);

        room.AddItem(new Item(itemId, finalName, finalDescription));
        return $"Added item '{finalName}' to room '{roomId}'.";
    }

    public string AddNpc(string? roomIdRaw, string? npcIdRaw, string? npcName, string? description)
    {
        string roomId = roomIdRaw.ToId();
        if (string.IsNullOrWhiteSpace(roomId))
            return "NPC creation needs a room id.";
        if (!_locations.TryGetValue(roomId, out Location? room))
            return $"Room '{roomId}' does not exist. Create it first.";

        string npcId = string.IsNullOrWhiteSpace(npcIdRaw) ? npcName.ToId() : npcIdRaw.ToId();
        if (string.IsNullOrWhiteSpace(npcId))
            return "NPC creation needs an npc id or name.";
        if (FindNpc(npcId).Npc is not null)
            return $"NPC '{npcId}' already exists.";

        string finalName = string.IsNullOrWhiteSpace(npcName) ? IdToDisplayName(npcId) : npcName.Trim();
        Npc npc = new(npcId, finalName);
        if (!string.IsNullOrWhiteSpace(description))
            _ = npc.Description(EnsureSentence(description, string.Empty));
        room.AddNpc(npc);
        return $"Added NPC '{finalName}' to room '{roomId}'.";
    }

    public string DescribeRoom(string? roomIdRaw, string? description)
    {
        string roomId = ResolveRoomId(roomIdRaw);
        if (!_locations.TryGetValue(roomId, out Location? room))
            return $"Room '{roomId}' does not exist.";
        if (string.IsNullOrWhiteSpace(description))
            return "Missing room description text.";

        room.Description(EnsureSentence(description, room.GetDescription()));
        return $"Updated description for room '{room.Id}'.";
    }

    public string DescribeItem(string? itemIdRaw, string? description)
    {
        string itemId = itemIdRaw.ToId();
        if (string.IsNullOrWhiteSpace(itemId))
            return "Missing item id.";
        if (string.IsNullOrWhiteSpace(description))
            return "Missing item description text.";

        (Item? item, Location? room) = FindItem(itemId);
        if (item is null || room is null)
            return $"No item '{itemId}' exists in any room.";

        _ = item.SetDescription(EnsureSentence(description, item.GetDescription()));
        return $"Updated description for item '{item.Id}' in room '{room.Id}'.";
    }

    public string DescribeNpc(string? npcIdRaw, string? description)
    {
        string npcId = npcIdRaw.ToId();
        if (string.IsNullOrWhiteSpace(npcId))
            return "Missing NPC id.";
        if (string.IsNullOrWhiteSpace(description))
            return "Missing NPC description text.";

        (Npc? npc, Location? room) = FindNpc(npcId);
        if (npc is null || room is null)
            return $"No NPC '{npcId}' exists in any room.";

        _ = npc.Description(EnsureSentence(description, npc.GetDescription()));
        return $"Updated description for NPC '{npc.Id}' in room '{room.Id}'.";
    }

    public string DescribeDoor(string? doorIdRaw, string? description)
    {
        string doorId = doorIdRaw.ToId();
        if (string.IsNullOrWhiteSpace(doorId))
            return "Missing door id.";
        if (string.IsNullOrWhiteSpace(description))
            return "Missing door description text.";

        (Door? door, _, _, _) = FindDoor(doorId);
        if (door is null)
            return $"No door '{doorId}' exists.";

        _ = door.Description(EnsureSentence(description, door.GetDescription()));
        return $"Updated description for door '{door.Id}'.";
    }

    public string DeleteRoom(string? roomIdRaw)
    {
        string roomId = ResolveRoomId(roomIdRaw);
        if (!_locations.TryGetValue(roomId, out Location? room))
            return $"Room '{roomId}' does not exist.";
        if (_locations.Count <= 1)
            return "You cannot delete the final room.";

        foreach (Location other in _locations.Values.Where(candidate => !candidate.Id.TextCompare(roomId)))
        {
            Direction[] exitsToRemove =
            [
                .. other.Exits
                    .Where(x => x.Value.Target.Id.TextCompare(roomId))
                    .Select(x => x.Key)
            ];

            foreach (Direction direction in exitsToRemove)
                _ = other.RemoveExit(direction);
        }

        _ = _locations.Remove(roomId);
        if (State.CurrentLocation.Id.TextCompare(roomId))
        {
            Location fallback = _locations.Values.OrderBy(static x => x.Id, StringComparer.OrdinalIgnoreCase).First();
            State.Teleport(fallback);
        }

        return $"Deleted room '{roomId}'.";
    }

    public string DeleteItem(string? itemIdRaw)
    {
        string itemId = itemIdRaw.ToId();
        if (string.IsNullOrWhiteSpace(itemId))
            return "Missing item id.";

        int removed = 0;
        foreach (Location room in _locations.Values)
        {
            IItem[] matches =
            [
                .. room.Items.Where(item => item.Id.TextCompare(itemId))
            ];

            foreach (IItem item in matches)
                removed += room.RemoveItem(item) ? 1 : 0;
        }

        return removed == 0
            ? $"No item '{itemId}' exists."
            : $"Deleted {removed} item(s) with id '{itemId}'.";
    }

    public string DeleteNpc(string? npcIdRaw)
    {
        string npcId = npcIdRaw.ToId();
        if (string.IsNullOrWhiteSpace(npcId))
            return "Missing NPC id.";

        int removed = 0;
        foreach (Location room in _locations.Values)
        {
            INpc[] matches =
            [
                .. room.Npcs.Where(npc => npc.Id.TextCompare(npcId))
            ];

            foreach (INpc npc in matches)
                removed += room.RemoveNpc(npc) ? 1 : 0;
        }

        return removed == 0
            ? $"No NPC '{npcId}' exists."
            : $"Deleted {removed} NPC(s) with id '{npcId}'.";
    }

    public string DeleteDoor(string? doorIdRaw)
    {
        string doorId = doorIdRaw.ToId();
        if (string.IsNullOrWhiteSpace(doorId))
            return "Missing door id.";

        int removedExits = 0;
        foreach (Location room in _locations.Values)
        {
            Direction[] exitsToRemove =
            [
                .. room.Exits
                    .Where(x => x.Value.Door?.Id.TextCompare(doorId) == true)
                    .Select(x => x.Key)
            ];

            foreach (Direction direction in exitsToRemove)
                removedExits += room.RemoveExit(direction) ? 1 : 0;
        }

        return removedExits == 0
            ? $"No door '{doorId}' exists."
            : $"Deleted door '{doorId}' ({removedExits} exit links removed).";
    }

    public string MoveTo(string? roomIdRaw)
    {
        string roomId = roomIdRaw.ToId();
        if (string.IsNullOrWhiteSpace(roomId))
            return "Please provide a room id to move to.";
        if (!_locations.TryGetValue(roomId, out Location? room))
            return $"No such room: {roomId}.";

        State.Teleport(room);
        return $"You move to {room.Id}.";
    }

    public string ExportDsl() => _exporter.Export(BuildSnapshotState(), WorldTitle, Goal);

    public string Save(string path)
    {
        string trimmed = path.Trim();
        if (!trimmed.EndsWith(".adventure", StringComparison.OrdinalIgnoreCase))
            trimmed += ".adventure";

        string fullPath = Path.GetFullPath(trimmed);
        string? directory = Path.GetDirectoryName(fullPath);
        if (!string.IsNullOrWhiteSpace(directory))
            Directory.CreateDirectory(directory);

        _exporter.ExportToFile(BuildSnapshotState(), fullPath, WorldTitle, Goal);
        return fullPath;
    }

    private GameState BuildSnapshotState()
    {
        Location fallback = _locations.Values.OrderBy(static x => x.Id, StringComparer.OrdinalIgnoreCase).First();
        Location current = _locations.TryGetValue(State.CurrentLocation.Id, out Location? tracked)
            ? tracked
            : fallback;
        return new GameState(current, worldLocations: _locations.Values);
    }

    private string ResolveRoomId(string? roomIdRaw)
    {
        if (string.IsNullOrWhiteSpace(roomIdRaw))
            return CurrentLocation.Id;

        string trimmed = roomIdRaw.Trim();
        return trimmed.TextCompare("this") || trimmed.TextCompare("current") || trimmed.TextCompare("here")
            ? CurrentLocation.Id
            : trimmed.ToId();
    }

    private static Direction? ParseDirection(string? directionRaw)
    {
        if (string.IsNullOrWhiteSpace(directionRaw))
            return null;

        string key = directionRaw.ToId();
        return DirectionAliases.TryGetValue(key, out Direction direction) ? direction : null;
    }

    private static Direction? GetAvailableDirection(Location source) =>
        DirectionPreference.FirstOrDefault(direction => !source.Exits.ContainsKey(direction));

    private (Item? Item, Location? Room) FindItem(string itemId)
    {
        foreach (Location room in _locations.Values)
        {
            if (room.Items.FirstOrDefault(candidate => candidate.Id.TextCompare(itemId)) is not Item item)
                continue;

            return (item, room);
        }

        return (null, null);
    }

    private (Npc? Npc, Location? Room) FindNpc(string npcId)
    {
        foreach (Location room in _locations.Values)
        {
            if (room.Npcs.FirstOrDefault(candidate => candidate.Id.TextCompare(npcId)) is not Npc npc)
                continue;

            return (npc, room);
        }

        return (null, null);
    }

    private (Door? Door, Location? From, Direction? Direction, Location? To) FindDoor(string doorId)
    {
        foreach (Location room in _locations.Values)
        {
            foreach ((Direction direction, Exit exit) in room.Exits)
            {
                if (exit.Door is not Door door || !door.Id.TextCompare(doorId))
                    continue;

                Location? target = _locations.TryGetValue(exit.Target.Id, out Location? tracked) ? tracked : null;
                return (door, room, direction, target);
            }
        }

        return (null, null, null, null);
    }

    private static string EnsureSentence(string? text, string fallback)
    {
        string cleaned = string.IsNullOrWhiteSpace(text) ? fallback : text.Trim();
        if (string.IsNullOrWhiteSpace(cleaned))
            cleaned = fallback;

        if (cleaned.EndsWith('.')
            || cleaned.EndsWith('!')
            || cleaned.EndsWith('?'))
        {
            return cleaned;
        }

        return $"{cleaned}.";
    }

    private static string IdToDisplayName(string id) =>
        id.Replace('_', ' ').Replace('-', ' ').ToProperCase();
}
