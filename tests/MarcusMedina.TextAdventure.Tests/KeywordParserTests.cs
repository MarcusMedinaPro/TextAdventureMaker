using MarcusMedina.TextAdventure.Commands;
using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Parsing;

namespace MarcusMedina.TextAdventure.Tests;

public class KeywordParserTests
{
    private static KeywordParserConfig CreateEnglishConfig()
    {
        return new KeywordParserConfig(
            quit: new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "quit", "exit", "q" },
            look: new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "look", "l", "ls" },
            inventory: new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "inventory", "inv", "i" },
            stats: new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "stats", "stat", "hp", "health" },
            open: new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "open" },
            unlock: new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "unlock" },
            take: new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "take", "get", "pickup", "pick" },
            drop: new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "drop" },
            use: new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "use", "eat", "bite" },
            combine: new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "combine", "mix" },
            pour: new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "pour" },
            go: new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "go", "move", "cd" },
            read: new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "read" },
            all: new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "all" },
            ignoreItemTokens: new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "up", "to" },
            combineSeparators: new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "and", "+" },
            pourPrepositions: new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "into", "in" },
            directionAliases: new Dictionary<string, Direction>(StringComparer.OrdinalIgnoreCase)
            {
                ["n"] = Direction.North,
                ["s"] = Direction.South,
                ["e"] = Direction.East,
                ["w"] = Direction.West,
                ["ne"] = Direction.NorthEast,
                ["nw"] = Direction.NorthWest,
                ["se"] = Direction.SouthEast,
                ["sw"] = Direction.SouthWest,
                ["u"] = Direction.Up,
                ["d"] = Direction.Down,
                ["in"] = Direction.In,
                ["out"] = Direction.Out
            },
            allowDirectionEnumNames: true);
    }

    [Theory]
    [InlineData("north", Direction.North)]
    [InlineData("go east", Direction.East)]
    [InlineData("move south", Direction.South)]
    [InlineData("n", Direction.North)]
    [InlineData("out", Direction.Out)]
    public void Parse_Directions_ReturnsGoCommand(string input, Direction expected)
    {
        var parser = new KeywordParser(CreateEnglishConfig());

        var command = parser.Parse(input);

        var go = Assert.IsType<GoCommand>(command);
        Assert.Equal(expected, go.Direction);
    }

    [Theory]
    [InlineData("look")]
    [InlineData("l")]
    [InlineData("ls")]
    public void Parse_Look_ReturnsLookCommand(string input)
    {
        var parser = new KeywordParser(CreateEnglishConfig());

        var command = parser.Parse(input);

        Assert.IsType<LookCommand>(command);
    }

    [Fact]
    public void Parse_LookAt_ReturnsLookCommandWithTarget()
    {
        var parser = new KeywordParser(CreateEnglishConfig());

        var command = parser.Parse("look key");

        var look = Assert.IsType<LookCommand>(command);
        Assert.Equal("key", look.Target);
    }

    [Theory]
    [InlineData("stats")]
    [InlineData("stat")]
    [InlineData("hp")]
    [InlineData("health")]
    public void Parse_Stats_ReturnsStatsCommand(string input)
    {
        var parser = new KeywordParser(CreateEnglishConfig());

        var command = parser.Parse(input);

        Assert.IsType<StatsCommand>(command);
    }

    [Theory]
    [InlineData("inventory")]
    [InlineData("inv")]
    [InlineData("i")]
    public void Parse_Inventory_ReturnsInventoryCommand(string input)
    {
        var parser = new KeywordParser(CreateEnglishConfig());

        var command = parser.Parse(input);

        Assert.IsType<InventoryCommand>(command);
    }

    [Theory]
    [InlineData("take apple")]
    [InlineData("get sword")]
    [InlineData("pick up coin")]
    public void Parse_Take_ReturnsTakeCommand(string input)
    {
        var parser = new KeywordParser(CreateEnglishConfig());

        var command = parser.Parse(input);

        Assert.IsType<TakeCommand>(command);
    }


    [Theory]
    [InlineData("take all")]
    [InlineData("get all")]
    public void Parse_TakeAll_ReturnsTakeAllCommand(string input)
    {
        var parser = new KeywordParser(CreateEnglishConfig());

        var command = parser.Parse(input);

        Assert.IsType<TakeAllCommand>(command);
    }

    [Fact]
    public void Parse_Drop_ReturnsDropCommand()
    {
        var parser = new KeywordParser(CreateEnglishConfig());

        var command = parser.Parse("drop coin");

        Assert.IsType<DropCommand>(command);
    }

    [Fact]
    public void Parse_Use_ReturnsUseCommand()
    {
        var parser = new KeywordParser(CreateEnglishConfig());

        var command = parser.Parse("use wand");

        Assert.IsType<UseCommand>(command);
    }

    [Fact]
    public void Parse_Read_ReturnsReadCommand()
    {
        var parser = new KeywordParser(CreateEnglishConfig());

        var command = parser.Parse("read sign");

        Assert.IsType<ReadCommand>(command);
    }

    [Theory]
    [InlineData("eat ice")]
    [InlineData("bite ice")]
    public void Parse_EatOrBite_ReturnsUseCommand(string input)
    {
        var parser = new KeywordParser(CreateEnglishConfig());

        var command = parser.Parse(input);

        Assert.IsType<UseCommand>(command);
    }

    [Theory]
    [InlineData("cd north")]
    public void Parse_Cd_ReturnsGoCommand(string input)
    {
        var parser = new KeywordParser(CreateEnglishConfig());

        var command = parser.Parse(input);

        Assert.IsType<GoCommand>(command);
    }

    [Theory]
    [InlineData("drop all")]
    [InlineData("drop all items")]
    public void Parse_DropAll_ReturnsDropAllCommand(string input)
    {
        var parser = new KeywordParser(CreateEnglishConfig());

        var command = parser.Parse(input);

        Assert.IsType<DropAllCommand>(command);
    }

    [Fact]
    public void Parse_GoDoor_ReturnsGoToCommand()
    {
        var parser = new KeywordParser(CreateEnglishConfig());

        var command = parser.Parse("go door");

        Assert.IsType<GoToCommand>(command);
    }

    [Theory]
    [InlineData("quit")]
    [InlineData("exit")]
    [InlineData("q")]
    public void Parse_Quit_ReturnsQuitCommand(string input)
    {
        var parser = new KeywordParser(CreateEnglishConfig());

        var command = parser.Parse(input);

        Assert.IsType<QuitCommand>(command);
    }

    [Theory]
    [InlineData("open")]
    [InlineData("open door")]
    public void Parse_Open_ReturnsOpenCommand(string input)
    {
        var parser = new KeywordParser(CreateEnglishConfig());

        var command = parser.Parse(input);

        Assert.IsType<OpenCommand>(command);
    }

    [Theory]
    [InlineData("unlock")]
    [InlineData("unlock door")]
    public void Parse_Unlock_ReturnsUnlockCommand(string input)
    {
        var parser = new KeywordParser(CreateEnglishConfig());

        var command = parser.Parse(input);

        Assert.IsType<UnlockCommand>(command);
    }

    [Fact]
    public void Parse_Unknown_ReturnsUnknownCommand()
    {
        var parser = new KeywordParser(CreateEnglishConfig());

        var command = parser.Parse("dance");

        Assert.IsType<UnknownCommand>(command);
    }
}
