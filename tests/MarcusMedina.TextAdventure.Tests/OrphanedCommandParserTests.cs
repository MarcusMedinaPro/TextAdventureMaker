// <copyright file="OrphanedCommandParserTests.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using MarcusMedina.TextAdventure.Commands;
using MarcusMedina.TextAdventure.Engine;
using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Interfaces;
using MarcusMedina.TextAdventure.Parsing;

namespace MarcusMedina.TextAdventure.Tests;

/// <summary>Tests that previously orphaned command classes are now reachable via the keyword parser.</summary>
public class OrphanedCommandParserTests
{
    private static KeywordParser CreateParser() => new(KeywordParserConfig.Default);

    // -------------------------------------------------------------------------
    // ThrowCommand
    // -------------------------------------------------------------------------

    [Fact]
    public void Parse_Throw_ReturnsThrowCommand()
    {
        ICommand command = CreateParser().Parse("throw sword north");

        ThrowCommand result = Assert.IsType<ThrowCommand>(command);
        Assert.Equal("sword", result.ItemName);
        Assert.Equal(Direction.North, result.Direction);
    }

    [Fact]
    public void Parse_Hurl_ReturnsThrowCommand()
    {
        ICommand command = CreateParser().Parse("hurl coin east");

        _ = Assert.IsType<ThrowCommand>(command);
    }

    [Fact]
    public void Parse_Toss_ReturnsThrowCommand()
    {
        ICommand command = CreateParser().Parse("toss rock south");

        _ = Assert.IsType<ThrowCommand>(command);
    }

    [Fact]
    public void Parse_Throw_MultiWordItem_ReturnsCorrectItemName()
    {
        ICommand command = CreateParser().Parse("throw rusty sword north");

        ThrowCommand result = Assert.IsType<ThrowCommand>(command);
        Assert.Equal("rusty sword", result.ItemName);
        Assert.Equal(Direction.North, result.Direction);
    }

    [Fact]
    public void Parse_Throw_MissingDirection_ReturnsUnknownCommand()
    {
        ICommand command = CreateParser().Parse("throw sword");

        _ = Assert.IsType<UnknownCommand>(command);
    }

    // -------------------------------------------------------------------------
    // RepairCommand
    // -------------------------------------------------------------------------

    [Fact]
    public void Parse_Repair_ReturnsRepairCommand()
    {
        ICommand command = CreateParser().Parse("repair sword");

        RepairCommand result = Assert.IsType<RepairCommand>(command);
        Assert.Equal("sword", result.ItemName);
        Assert.Null(result.ToolName);
    }

    [Fact]
    public void Parse_Fix_ReturnsRepairCommand()
    {
        ICommand command = CreateParser().Parse("fix armour");

        _ = Assert.IsType<RepairCommand>(command);
    }

    [Fact]
    public void Parse_RepairWith_ReturnsBothItemAndTool()
    {
        ICommand command = CreateParser().Parse("repair sword with hammer");

        RepairCommand result = Assert.IsType<RepairCommand>(command);
        Assert.Equal("sword", result.ItemName);
        Assert.Equal("hammer", result.ToolName);
    }

    [Fact]
    public void Parse_RepairMultiWordItemWith_ReturnsBothItemAndTool()
    {
        ICommand command = CreateParser().Parse("repair rusty sword with repair kit");

        RepairCommand result = Assert.IsType<RepairCommand>(command);
        Assert.Equal("rusty sword", result.ItemName);
        Assert.Equal("repair kit", result.ToolName);
    }

    [Fact]
    public void Parse_Repair_NoArgs_ReturnsUnknownCommand()
    {
        ICommand command = CreateParser().Parse("repair");

        _ = Assert.IsType<UnknownCommand>(command);
    }

    // -------------------------------------------------------------------------
    // SolveCommand
    // -------------------------------------------------------------------------

    [Fact]
    public void Parse_Solve_ReturnsSolveCommand()
    {
        ICommand command = CreateParser().Parse("solve puzzle1 fire");

        SolveCommand result = Assert.IsType<SolveCommand>(command);
        Assert.Equal("puzzle1", result.PuzzleId);
        Assert.Equal("fire", result.Answer);
    }

    [Fact]
    public void Parse_Answer_ReturnsSolveCommand()
    {
        ICommand command = CreateParser().Parse("answer riddle1 the wind");

        SolveCommand result = Assert.IsType<SolveCommand>(command);
        Assert.Equal("riddle1", result.PuzzleId);
        Assert.Equal("the wind", result.Answer);
    }

    [Fact]
    public void Parse_Solve_MultiWordAnswer()
    {
        ICommand command = CreateParser().Parse("solve puzzle1 forty two");

        SolveCommand result = Assert.IsType<SolveCommand>(command);
        Assert.Equal("puzzle1", result.PuzzleId);
        Assert.Equal("forty two", result.Answer);
    }

    [Fact]
    public void Parse_Solve_MissingAnswer_ReturnsUnknownCommand()
    {
        ICommand command = CreateParser().Parse("solve puzzle1");

        _ = Assert.IsType<UnknownCommand>(command);
    }

    // -------------------------------------------------------------------------
    // ShoutCommand
    // -------------------------------------------------------------------------

    [Fact]
    public void Parse_Shout_NoArgs_ReturnsShoutCommand()
    {
        ICommand command = CreateParser().Parse("shout");

        ShoutCommand result = Assert.IsType<ShoutCommand>(command);
        Assert.Null(result.Direction);
        Assert.Null(result.Message);
    }

    [Fact]
    public void Parse_Shout_WithDirection_SetsDirection()
    {
        ICommand command = CreateParser().Parse("shout north");

        ShoutCommand result = Assert.IsType<ShoutCommand>(command);
        Assert.Equal(Direction.North, result.Direction);
        Assert.Null(result.Message);
    }

    [Fact]
    public void Parse_Shout_WithDirectionAndMessage_SetsBoth()
    {
        ICommand command = CreateParser().Parse("shout north help me");

        ShoutCommand result = Assert.IsType<ShoutCommand>(command);
        Assert.Equal(Direction.North, result.Direction);
        Assert.Equal("help me", result.Message);
    }

    [Fact]
    public void Parse_Shout_WithMessageNoDirection_SetsMessageOnly()
    {
        ICommand command = CreateParser().Parse("shout hello world");

        ShoutCommand result = Assert.IsType<ShoutCommand>(command);
        Assert.Null(result.Direction);
        Assert.Equal("hello world", result.Message);
    }

    [Fact]
    public void Parse_Yell_ReturnsShoutCommand()
    {
        ICommand command = CreateParser().Parse("yell");

        _ = Assert.IsType<ShoutCommand>(command);
    }

    [Fact]
    public void Parse_Call_ReturnsShoutCommand()
    {
        ICommand command = CreateParser().Parse("call north");

        _ = Assert.IsType<ShoutCommand>(command);
    }

    // -------------------------------------------------------------------------
    // ListenCommand
    // -------------------------------------------------------------------------

    [Fact]
    public void Parse_Listen_NoArgs_ReturnsListenCommand()
    {
        ICommand command = CreateParser().Parse("listen");

        ListenCommand result = Assert.IsType<ListenCommand>(command);
        Assert.Null(result.Direction);
    }

    [Fact]
    public void Parse_Listen_WithDirection_SetsDirection()
    {
        ICommand command = CreateParser().Parse("listen north");

        ListenCommand result = Assert.IsType<ListenCommand>(command);
        Assert.Equal(Direction.North, result.Direction);
    }

    [Fact]
    public void Parse_Hear_ReturnsListenCommand()
    {
        ICommand command = CreateParser().Parse("hear");

        _ = Assert.IsType<ListenCommand>(command);
    }

    // -------------------------------------------------------------------------
    // BuyCommand
    // -------------------------------------------------------------------------

    [Fact]
    public void Parse_Buy_ReturnsBuyCommand()
    {
        ICommand command = CreateParser().Parse("buy bread");

        _ = Assert.IsType<BuyCommand>(command);
    }

    [Fact]
    public void Parse_Purchase_ReturnsBuyCommand()
    {
        ICommand command = CreateParser().Parse("purchase sword");

        _ = Assert.IsType<BuyCommand>(command);
    }

    [Fact]
    public void Parse_Buy_NoItem_ReturnsUnknownCommand()
    {
        ICommand command = CreateParser().Parse("buy");

        _ = Assert.IsType<UnknownCommand>(command);
    }

    // -------------------------------------------------------------------------
    // SellCommand
    // -------------------------------------------------------------------------

    [Fact]
    public void Parse_Sell_ReturnsSellCommand()
    {
        ICommand command = CreateParser().Parse("sell sword");

        _ = Assert.IsType<SellCommand>(command);
    }

    [Fact]
    public void Parse_Sell_NoItem_ReturnsUnknownCommand()
    {
        ICommand command = CreateParser().Parse("sell");

        _ = Assert.IsType<UnknownCommand>(command);
    }

    // -------------------------------------------------------------------------
    // ShopCommand
    // -------------------------------------------------------------------------

    [Fact]
    public void Parse_Shop_ReturnsShopCommand()
    {
        ICommand command = CreateParser().Parse("shop");

        _ = Assert.IsType<ShopCommand>(command);
    }

    [Fact]
    public void Parse_Store_ReturnsShopCommand()
    {
        ICommand command = CreateParser().Parse("store");

        _ = Assert.IsType<ShopCommand>(command);
    }

    [Fact]
    public void Parse_Wares_ReturnsShopCommand()
    {
        ICommand command = CreateParser().Parse("wares");

        _ = Assert.IsType<ShopCommand>(command);
    }

    // -------------------------------------------------------------------------
    // UndoCommand
    // -------------------------------------------------------------------------

    [Fact]
    public void Parse_Undo_ReturnsUndoCommand()
    {
        ICommand command = CreateParser().Parse("undo");

        _ = Assert.IsType<UndoCommand>(command);
    }

    // -------------------------------------------------------------------------
    // RedoCommand
    // -------------------------------------------------------------------------

    [Fact]
    public void Parse_Redo_ReturnsRedoCommand()
    {
        ICommand command = CreateParser().Parse("redo");

        _ = Assert.IsType<RedoCommand>(command);
    }

    // -------------------------------------------------------------------------
    // MapCommand
    // -------------------------------------------------------------------------

    [Fact]
    public void Parse_Map_ReturnsMapCommand()
    {
        ICommand command = CreateParser().Parse("map");

        _ = Assert.IsType<MapCommand>(command);
    }

    // -------------------------------------------------------------------------
    // HistoryCommand
    // -------------------------------------------------------------------------

    [Fact]
    public void Parse_History_ReturnsHistoryCommand()
    {
        ICommand command = CreateParser().Parse("history");

        _ = Assert.IsType<HistoryCommand>(command);
    }

    [Fact]
    public void Parse_Log_ReturnsHistoryCommand()
    {
        ICommand command = CreateParser().Parse("log");

        _ = Assert.IsType<HistoryCommand>(command);
    }

    // -------------------------------------------------------------------------
    // Builder integration: custom keyword sets flow through to parser
    // -------------------------------------------------------------------------

    [Fact]
    public void Builder_WithThrow_CustomKeyword_Works()
    {
        KeywordParserConfig config = KeywordParserConfigBuilder.BritishDefaults()
            .WithThrow("fling")
            .Build();
        KeywordParser parser = new(config);

        ICommand command = parser.Parse("fling coin north");

        ThrowCommand result = Assert.IsType<ThrowCommand>(command);
        Assert.Equal("coin", result.ItemName);
        Assert.Equal(Direction.North, result.Direction);
    }

    [Fact]
    public void Builder_WithRepair_CustomKeyword_Works()
    {
        KeywordParserConfig config = KeywordParserConfigBuilder.BritishDefaults()
            .WithRepair("mend")
            .Build();
        KeywordParser parser = new(config);

        ICommand command = parser.Parse("mend armour");

        _ = Assert.IsType<RepairCommand>(command);
    }

    [Fact]
    public void Builder_WithShop_CustomKeyword_Works()
    {
        KeywordParserConfig config = KeywordParserConfigBuilder.BritishDefaults()
            .WithShop("market")
            .Build();
        KeywordParser parser = new(config);

        ICommand command = parser.Parse("market");

        _ = Assert.IsType<ShopCommand>(command);
    }
}
