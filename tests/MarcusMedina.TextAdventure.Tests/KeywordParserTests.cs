// <copyright file="KeywordParserTests.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using MarcusMedina.TextAdventure.Commands;
using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Helpers;
using MarcusMedina.TextAdventure.Interfaces;
using MarcusMedina.TextAdventure.Parsing;

namespace MarcusMedina.TextAdventure.Tests;

public class KeywordParserTests
{
    private static KeywordParserConfig CreateEnglishConfig()
    {
        return new(
            quit: CommandHelper.NewCommands("quit", "exit", "q"),
            look: CommandHelper.NewCommands("look", "l", "ls"),
            examine: CommandHelper.NewCommands("examine", "x"),
            inventory: CommandHelper.NewCommands("inventory", "inv", "i"),
            stats: CommandHelper.NewCommands("stats", "stat", "hp", "health"),
            open: CommandHelper.NewCommands("open"),
            unlock: CommandHelper.NewCommands("unlock"),
            take: CommandHelper.NewCommands("take", "get", "pickup", "pick"),
            drop: CommandHelper.NewCommands("drop"),
            use: CommandHelper.NewCommands("use", "eat", "bite"),
            combine: CommandHelper.NewCommands("combine", "mix"),
            pour: CommandHelper.NewCommands("pour"),
            move: CommandHelper.NewCommands("move", "push", "shift"),
            go: CommandHelper.NewCommands("go", "move", "cd"),
            read: CommandHelper.NewCommands("read"),
            talk: CommandHelper.NewCommands("talk", "speak"),
            attack: CommandHelper.NewCommands("attack", "fight"),
            flee: CommandHelper.NewCommands("flee", "run"),
            save: CommandHelper.NewCommands("save"),
            load: CommandHelper.NewCommands("load"),
            quest: CommandHelper.NewCommands("quest", "quests", "journal"),
            hint: CommandHelper.NewCommands("hint", "path"),
            all: CommandHelper.NewCommands("all"),
            ignoreItemTokens: CommandHelper.NewCommands("up", "to"),
            combineSeparators: CommandHelper.NewCommands("and", "+"),
            pourPrepositions: CommandHelper.NewCommands("into", "in"),
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
        KeywordParser parser = new(CreateEnglishConfig());

        ICommand command = parser.Parse(input);

        GoCommand go = Assert.IsType<GoCommand>(command);
        Assert.Equal(expected, go.Direction);
    }

    [Theory]
    [InlineData("look")]
    [InlineData("l")]
    [InlineData("ls")]
    public void Parse_Look_ReturnsLookCommand(string input)
    {
        KeywordParser parser = new(CreateEnglishConfig());

        ICommand command = parser.Parse(input);

        _ = Assert.IsType<LookCommand>(command);
    }

    [Theory]
    [InlineData("move stone")]
    [InlineData("push stone")]
    public void Parse_Move_ReturnsMoveCommand(string input)
    {
        KeywordParser parser = new(CreateEnglishConfig());

        ICommand command = parser.Parse(input);

        _ = Assert.IsType<MoveCommand>(command);
    }

    [Fact]
    public void Parse_MoveWithDirection_ReturnsGoCommand()
    {
        KeywordParser parser = new(CreateEnglishConfig());

        ICommand command = parser.Parse("move north");

        GoCommand go = Assert.IsType<GoCommand>(command);
        Assert.Equal(Direction.North, go.Direction);
    }

    [Theory]
    [InlineData("examine")]
    [InlineData("x")]
    public void Parse_Examine_ReturnsExamineCommand(string input)
    {
        KeywordParser parser = new(CreateEnglishConfig());

        ICommand command = parser.Parse(input);

        _ = Assert.IsType<ExamineCommand>(command);
    }

    [Fact]
    public void Parse_ExamineAt_ReturnsExamineCommandWithTarget()
    {
        KeywordParser parser = new(CreateEnglishConfig());

        ICommand command = parser.Parse("examine key");

        ExamineCommand examine = Assert.IsType<ExamineCommand>(command);
        Assert.Equal("key", examine.Target);
    }

    [Fact]
    public void Parse_LookAt_ReturnsLookCommandWithTarget()
    {
        KeywordParser parser = new(CreateEnglishConfig());

        ICommand command = parser.Parse("look key");

        LookCommand look = Assert.IsType<LookCommand>(command);
        Assert.Equal("key", look.Target);
    }

    [Theory]
    [InlineData("stats")]
    [InlineData("stat")]
    [InlineData("hp")]
    [InlineData("health")]
    public void Parse_Stats_ReturnsStatsCommand(string input)
    {
        KeywordParser parser = new(CreateEnglishConfig());

        ICommand command = parser.Parse(input);

        _ = Assert.IsType<StatsCommand>(command);
    }

    [Theory]
    [InlineData("inventory")]
    [InlineData("inv")]
    [InlineData("i")]
    public void Parse_Inventory_ReturnsInventoryCommand(string input)
    {
        KeywordParser parser = new(CreateEnglishConfig());

        ICommand command = parser.Parse(input);

        _ = Assert.IsType<InventoryCommand>(command);
    }

    [Theory]
    [InlineData("take apple")]
    [InlineData("get sword")]
    [InlineData("pick up coin")]
    public void Parse_Take_ReturnsTakeCommand(string input)
    {
        KeywordParser parser = new(CreateEnglishConfig());

        ICommand command = parser.Parse(input);

        _ = Assert.IsType<TakeCommand>(command);
    }

    [Theory]
    [InlineData("take all")]
    [InlineData("get all")]
    public void Parse_TakeAll_ReturnsTakeAllCommand(string input)
    {
        KeywordParser parser = new(CreateEnglishConfig());

        ICommand command = parser.Parse(input);

        _ = Assert.IsType<TakeAllCommand>(command);
    }

    [Fact]
    public void Parse_Drop_ReturnsDropCommand()
    {
        KeywordParser parser = new(CreateEnglishConfig());

        ICommand command = parser.Parse("drop coin");

        _ = Assert.IsType<DropCommand>(command);
    }

    [Fact]
    public void Parse_Use_ReturnsUseCommand()
    {
        KeywordParser parser = new(CreateEnglishConfig());

        ICommand command = parser.Parse("use wand");

        _ = Assert.IsType<UseCommand>(command);
    }

    [Fact]
    public void Parse_Synonym_ReturnsCanonicalCommand()
    {
        KeywordParserConfig config = KeywordParserConfigBuilder.BritishDefaults()
            .WithTake("take")
            .AddSynonyms("take", "grab", "pickup")
            .Build();

        KeywordParser parser = new(config);

        ICommand command = parser.Parse("grab coin");

        _ = Assert.IsType<TakeCommand>(command);
    }

    [Fact]
    public void Parse_Read_ReturnsReadCommand()
    {
        KeywordParser parser = new(CreateEnglishConfig());

        ICommand command = parser.Parse("read sign");

        _ = Assert.IsType<ReadCommand>(command);
    }

    [Fact]
    public void Parse_Talk_ReturnsTalkCommand()
    {
        KeywordParser parser = new(CreateEnglishConfig());

        ICommand command = parser.Parse("talk fox");

        _ = Assert.IsType<TalkCommand>(command);
    }

    [Fact]
    public void Parse_Attack_ReturnsAttackCommand()
    {
        KeywordParser parser = new(CreateEnglishConfig());

        ICommand command = parser.Parse("attack troll");

        _ = Assert.IsType<AttackCommand>(command);
    }

    [Fact]
    public void Parse_Flee_ReturnsFleeCommand()
    {
        KeywordParser parser = new(CreateEnglishConfig());

        ICommand command = parser.Parse("flee");

        _ = Assert.IsType<FleeCommand>(command);
    }

    [Fact]
    public void Parse_Save_ReturnsSaveCommand()
    {
        KeywordParser parser = new(CreateEnglishConfig());

        ICommand command = parser.Parse("save");

        _ = Assert.IsType<SaveCommand>(command);
    }

    [Fact]
    public void Parse_Load_ReturnsLoadCommand()
    {
        KeywordParser parser = new(CreateEnglishConfig());

        ICommand command = parser.Parse("load savegame.json");

        _ = Assert.IsType<LoadCommand>(command);
    }

    [Theory]
    [InlineData("quest")]
    [InlineData("quests")]
    [InlineData("journal")]
    public void Parse_Quest_ReturnsQuestCommand(string input)
    {
        KeywordParser parser = new(CreateEnglishConfig());

        ICommand command = parser.Parse(input);

        _ = Assert.IsType<QuestCommand>(command);
    }

    [Theory]
    [InlineData("eat ice")]
    [InlineData("bite ice")]
    public void Parse_EatOrBite_ReturnsUseCommand(string input)
    {
        KeywordParser parser = new(CreateEnglishConfig());

        ICommand command = parser.Parse(input);

        _ = Assert.IsType<UseCommand>(command);
    }

    [Theory]
    [InlineData("cd north")]
    public void Parse_Cd_ReturnsGoCommand(string input)
    {
        KeywordParser parser = new(CreateEnglishConfig());

        ICommand command = parser.Parse(input);

        _ = Assert.IsType<GoCommand>(command);
    }

    [Theory]
    [InlineData("drop all")]
    [InlineData("drop all items")]
    public void Parse_DropAll_ReturnsDropAllCommand(string input)
    {
        KeywordParser parser = new(CreateEnglishConfig());

        ICommand command = parser.Parse(input);

        _ = Assert.IsType<DropAllCommand>(command);
    }

    [Fact]
    public void Parse_GoDoor_ReturnsGoToCommand()
    {
        KeywordParser parser = new(CreateEnglishConfig());

        ICommand command = parser.Parse("go door");

        _ = Assert.IsType<GoToCommand>(command);
    }

    [Theory]
    [InlineData("quit")]
    [InlineData("exit")]
    [InlineData("q")]
    public void Parse_Quit_ReturnsQuitCommand(string input)
    {
        KeywordParser parser = new(CreateEnglishConfig());

        ICommand command = parser.Parse(input);

        _ = Assert.IsType<QuitCommand>(command);
    }

    [Theory]
    [InlineData("open")]
    [InlineData("open door")]
    public void Parse_Open_ReturnsOpenCommand(string input)
    {
        KeywordParser parser = new(CreateEnglishConfig());

        ICommand command = parser.Parse(input);

        _ = Assert.IsType<OpenCommand>(command);
    }

    [Theory]
    [InlineData("unlock")]
    [InlineData("unlock door")]
    public void Parse_Unlock_ReturnsUnlockCommand(string input)
    {
        KeywordParser parser = new(CreateEnglishConfig());

        ICommand command = parser.Parse(input);

        _ = Assert.IsType<UnlockCommand>(command);
    }

    [Fact]
    public void Parse_Unknown_ReturnsUnknownCommand()
    {
        KeywordParser parser = new(CreateEnglishConfig());

        ICommand command = parser.Parse("dance");

        _ = Assert.IsType<UnknownCommand>(command);
    }
}
