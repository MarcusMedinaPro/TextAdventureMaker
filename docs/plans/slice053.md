## Slice 53: Advanced Map Generator

**Mål:** Procedural kartgenerering och ASCII-visualisering av spelvärlden.

**Referens:** `docs/plans/imported/Map_Generator_System.md`

### Task 53.1: IMapGenerator interface

```csharp
public interface IMapGenerator
{
    string GenerateAsciiMap(IGameState state, MapOptions? options = null);
    MapData GenerateMapData(IGameState state);
}

public record MapOptions(
    bool ShowUnvisited = false,
    bool ShowItems = false,
    bool ShowNpcs = false,
    int MaxWidth = 80,
    int MaxHeight = 24
);

public record MapData(
    Dictionary<ILocation, Point> Positions,
    List<MapConnection> Connections
);

public record Point(int X, int Y);
public record MapConnection(ILocation From, ILocation To, Direction Direction);
```

### Task 53.2: Graph-baserad layout-algoritm

```csharp
public class MapLayoutCalculator
{
    public Dictionary<ILocation, Point> Calculate(IEnumerable<ILocation> locations)
    {
        var positions = new Dictionary<ILocation, Point>();
        var visited = new HashSet<ILocation>();
        var queue = new Queue<(ILocation loc, Point pos)>();

        var start = locations.First();
        queue.Enqueue((start, new Point(0, 0)));

        while (queue.Count > 0)
        {
            var (loc, pos) = queue.Dequeue();
            if (visited.Contains(loc)) continue;

            visited.Add(loc);
            positions[loc] = pos;

            foreach (var (dir, exit) in loc.Exits)
            {
                if (!visited.Contains(exit.Target))
                {
                    var newPos = GetAdjacentPosition(pos, dir);
                    queue.Enqueue((exit.Target, newPos));
                }
            }
        }

        return NormalizePositions(positions);
    }

    private static Point GetAdjacentPosition(Point pos, Direction dir) => dir switch
    {
        Direction.North => pos with { Y = pos.Y - 1 },
        Direction.South => pos with { Y = pos.Y + 1 },
        Direction.East => pos with { X = pos.X + 1 },
        Direction.West => pos with { X = pos.X - 1 },
        Direction.Up => pos with { Y = pos.Y - 1 },
        Direction.Down => pos with { Y = pos.Y + 1 },
        _ => pos
    };
}
```

### Task 53.3: ASCII Map Renderer

```csharp
public class AsciiMapRenderer
{
    private const char RoomChar = '█';
    private const char CurrentRoomChar = '@';
    private const char UnvisitedChar = '?';
    private const char HorizontalPath = '─';
    private const char VerticalPath = '│';
    private const char DoorChar = '▒';

    public string Render(MapData data, IGameState state, MapOptions options)
    {
        var grid = CreateGrid(data, options);

        foreach (var (loc, pos) in data.Positions)
        {
            var displayPos = ToGridPosition(pos, options);

            char roomChar = loc == state.CurrentLocation ? CurrentRoomChar
                          : !state.VisitedLocations.Contains(loc) && !options.ShowUnvisited ? UnvisitedChar
                          : RoomChar;

            grid[displayPos.Y, displayPos.X] = roomChar;

            // Rita anslutningar
            foreach (var (dir, exit) in loc.Exits)
            {
                var pathChar = GetPathChar(dir, exit.Door);
                var pathPos = GetPathPosition(displayPos, dir);
                if (IsInBounds(pathPos, grid))
                    grid[pathPos.Y, pathPos.X] = pathChar;
            }
        }

        return GridToString(grid);
    }

    private char GetPathChar(Direction dir, IDoor? door)
    {
        var baseChar = dir is Direction.North or Direction.South ? VerticalPath : HorizontalPath;
        return door != null && door.State != DoorState.Open ? DoorChar : baseChar;
    }
}
```

### Task 53.4: MapCommand

```csharp
public class MapCommand : ICommand
{
    public CommandResult Execute(CommandContext context)
    {
        var generator = new MapGenerator();
        var map = generator.GenerateAsciiMap(context.State, new MapOptions(
            ShowUnvisited: false,
            ShowItems: false
        ));

        return CommandResult.Ok(map);
    }
}
```

### Task 53.5: Procedural World Generation

```csharp
public class ProceduralWorldGenerator
{
    public World Generate(WorldGenerationOptions options)
    {
        var world = new World(options.Name);
        var rooms = new List<ILocation>();

        // Generera rum baserat på template
        for (int i = 0; i < options.RoomCount; i++)
        {
            var room = GenerateRoom(options.Theme, i);
            rooms.Add(room);
            world.AddLocation(room);
        }

        // Koppla ihop rum
        ConnectRooms(rooms, options.Connectivity);

        // Placera items och NPCs
        PopulateWorld(world, options);

        return world;
    }

    private void ConnectRooms(List<ILocation> rooms, float connectivity)
    {
        // Skapa minimum spanning tree först
        var mst = CreateMinimumSpanningTree(rooms);

        // Lägg till extra kopplingar baserat på connectivity
        var extraConnections = (int)((rooms.Count - 1) * connectivity);
        AddRandomConnections(rooms, extraConnections);
    }
}

public record WorldGenerationOptions(
    string Name,
    int RoomCount = 10,
    float Connectivity = 0.3f,
    string Theme = "dungeon",
    int ItemDensity = 2,
    int NpcDensity = 1
);
```

### Task 53.6: Room Templates

```csharp
public static class RoomTemplates
{
    public static readonly Dictionary<string, RoomTemplate[]> Themes = new()
    {
        ["dungeon"] = [
            new("Dark Corridor", "A narrow stone corridor stretches ahead."),
            new("Guard Room", "An abandoned guard post with rusty weapons."),
            new("Cell", "A damp prison cell with iron bars."),
            new("Treasure Room", "Gold glints in the torchlight."),
        ],
        ["mansion"] = [
            new("Foyer", "An elegant entrance hall with marble floors."),
            new("Library", "Dusty books line the walls."),
            new("Dining Room", "A long table set for a feast that never came."),
            new("Bedroom", "A four-poster bed dominates the room."),
        ],
        ["forest"] = [
            new("Clearing", "Sunlight filters through the canopy."),
            new("Dense Woods", "Trees press close on all sides."),
            new("Stream", "A babbling brook crosses your path."),
            new("Cave Entrance", "A dark opening in the hillside."),
        ]
    };
}

public record RoomTemplate(string Name, string Description);
```

### Task 53.7: Tester

```csharp
[Fact]
public void MapGenerator_CreatesValidAsciiMap()
{
    var game = CreateGame();
    var generator = new MapGenerator();

    var map = generator.GenerateAsciiMap(game.State);

    Assert.Contains("@", map);  // Current location
    Assert.True(map.Split('\n').Length > 1);
}

[Fact]
public void ProceduralGenerator_CreatesConnectedWorld()
{
    var generator = new ProceduralWorldGenerator();
    var world = generator.Generate(new WorldGenerationOptions("Test", RoomCount: 5));

    Assert.Equal(5, world.Locations.Count);
    Assert.True(world.Locations.All(l => l.Exits.Any()));
}
```

### Task 53.8: Sandbox — procedurellt genererad dungeon

Demo med slumpmässigt genererad labyrint och ASCII-karta.

---
