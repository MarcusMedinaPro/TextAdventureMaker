// <copyright file="KeywordParserConfigBuilderTests.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
namespace MarcusMedina.TextAdventure.Tests;

using MarcusMedina.TextAdventure.Commands;
using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Parsing;

public class KeywordParserConfigBuilderTests
{
    [Fact]
    public void BritishDefaults_ParsesCommonCommands()
    {
        var parser = new KeywordParser(KeywordParserConfigBuilder.BritishDefaults().Build());

        _ = Assert.IsType<LookCommand>(parser.Parse("look"));
        _ = Assert.IsType<InventoryCommand>(parser.Parse("inventory"));
        _ = Assert.IsType<QuitCommand>(parser.Parse("quit"));

        var take = Assert.IsType<TakeCommand>(parser.Parse("take key"));
        Assert.Equal("key", take.ItemName);

        var use = Assert.IsType<UseCommand>(parser.Parse("use lamp"));
        Assert.Equal("lamp", use.ItemName);

        var go = Assert.IsType<GoCommand>(parser.Parse("n"));
        Assert.Equal(Direction.North, go.Direction);
    }

    [Fact]
    public void Builder_OverridesDirectionAliases()
    {
        var config = KeywordParserConfigBuilder.BritishDefaults()
            .WithDirectionAliases(new Dictionary<string, Direction>(StringComparer.OrdinalIgnoreCase)
            {
                ["in"] = Direction.In
            })
            .Build();
        var parser = new KeywordParser(config);

        var go = Assert.IsType<GoCommand>(parser.Parse("in"));
        Assert.Equal(Direction.In, go.Direction);

        _ = Assert.IsType<UnknownCommand>(parser.Parse("n"));
    }

    [Fact]
    public void Builder_OverridesUseKeywords()
    {
        var config = KeywordParserConfigBuilder.BritishDefaults()
            .WithUse("ignite")
            .Build();
        var parser = new KeywordParser(config);

        _ = Assert.IsType<UnknownCommand>(parser.Parse("use lamp"));
        var use = Assert.IsType<UseCommand>(parser.Parse("ignite lamp"));
        Assert.Equal("lamp", use.ItemName);
    }
}
