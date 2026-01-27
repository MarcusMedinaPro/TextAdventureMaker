// <copyright file="ActionConsequenceTests.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Models;

namespace MarcusMedina.TextAdventure.Tests;

public class ActionConsequenceTests
{
    [Fact]
    public void OnDrop_SetsConsequence()
    {
        var glass = new Item("glass", "wine glass", "A delicate wine glass.");
        var brokenGlass = new Item("broken_glass", "broken glass", "Sharp glass shards.");

        glass.OnDrop(ActionConsequence.Break("The glass shatters!", brokenGlass));

        Assert.True(glass.HasConsequence(ItemAction.Drop));
        var consequence = glass.GetConsequence(ItemAction.Drop);
        Assert.NotNull(consequence);
        Assert.True(consequence.DestroyItem);
        Assert.Single(consequence.CreatedItems);
        Assert.Equal("The glass shatters!", consequence.Message);
    }

    [Fact]
    public void OnUse_SetsConsequence()
    {
        var potion = new Item("potion", "health potion", "A red potion.");

        potion.OnUse(ActionConsequence.Destroy("You drink the potion. It tastes like cherries."));

        Assert.True(potion.HasConsequence(ItemAction.Use));
        var consequence = potion.GetConsequence(ItemAction.Use);
        Assert.NotNull(consequence);
        Assert.True(consequence.DestroyItem);
    }

    [Fact]
    public void OnTake_SetsConsequence()
    {
        var trap = new Item("trap", "bear trap", "A rusty bear trap.");

        trap.OnTake(new ActionConsequence
        {
            Message = "SNAP! The trap springs shut!",
            SetFlag = "injured"
        });

        Assert.True(trap.HasConsequence(ItemAction.Take));
    }

    [Fact]
    public void MakeFragile_SetsDropConsequence()
    {
        var vase = new Item("vase", "ming vase", "An ancient vase.");
        var shards = new Item("shards", "ceramic shards", "Broken pieces of the vase.");

        vase.MakeFragile("The vase shatters into a thousand pieces!", shards);

        Assert.True(vase.HasConsequence(ItemAction.Drop));
        var consequence = vase.GetConsequence(ItemAction.Drop);
        Assert.NotNull(consequence);
        Assert.True(consequence.DestroyItem);
        Assert.Single(consequence.CreatedItems);
        Assert.Equal("shards", consequence.CreatedItems[0].Id);
    }

    [Fact]
    public void MakeConsumable_SetsUseConsequence()
    {
        var apple = new Item("apple", "red apple", "A juicy red apple.");

        apple.MakeConsumable("You eat the apple. Delicious!");

        Assert.True(apple.HasConsequence(ItemAction.Use));
        var consequence = apple.GetConsequence(ItemAction.Use);
        Assert.NotNull(consequence);
        Assert.True(consequence.DestroyItem);
        Assert.Equal("You eat the apple. Delicious!", consequence.Message);
    }

    [Fact]
    public void GetConsequence_ReturnsNull_WhenNoConsequence()
    {
        var rock = new Item("rock", "rock", "A plain rock.");

        var consequence = rock.GetConsequence(ItemAction.Drop);

        Assert.Null(consequence);
    }

    [Fact]
    public void HasConsequence_ReturnsFalse_WhenNoConsequence()
    {
        var rock = new Item("rock", "rock", "A plain rock.");

        Assert.False(rock.HasConsequence(ItemAction.Drop));
        Assert.False(rock.HasConsequence(ItemAction.Use));
        Assert.False(rock.HasConsequence(ItemAction.Take));
    }

    [Fact]
    public void ActionConsequence_Transform_CreatesCorrectConsequence()
    {
        var caterpillar = new Item("caterpillar", "caterpillar", "A fuzzy caterpillar.");
        var butterfly = new Item("butterfly", "butterfly", "A beautiful butterfly.");

        var consequence = ActionConsequence.Transform(butterfly, "The caterpillar transforms into a butterfly!");

        Assert.True(consequence.DestroyItem);
        Assert.Single(consequence.CreatedItems);
        Assert.Equal("butterfly", consequence.CreatedItems[0].Id);
        Assert.Equal("The caterpillar transforms into a butterfly!", consequence.Message);
    }

    [Fact]
    public void ActionConsequence_Break_CreatesMultiplePieces()
    {
        var mirror = new Item("mirror", "mirror", "A magic mirror.");
        var shard1 = new Item("shard1", "mirror shard", "A piece of the mirror.");
        var shard2 = new Item("shard2", "mirror shard", "Another piece of the mirror.");

        var consequence = ActionConsequence.Break("The mirror breaks!", shard1, shard2);

        Assert.True(consequence.DestroyItem);
        Assert.Equal(2, consequence.CreatedItems.Count);
    }
}
