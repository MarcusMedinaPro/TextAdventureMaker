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
        Item glass = new("glass", "wine glass", "A delicate wine glass.");
        Item brokenGlass = new("broken_glass", "broken glass", "Sharp glass shards.");

        _ = glass.SetDropConsequence(ActionConsequence.Break("The glass shatters!", brokenGlass));

        Assert.True(glass.HasConsequence(ItemAction.Drop));
        ActionConsequence? consequence = glass.GetConsequence(ItemAction.Drop);
        Assert.NotNull(consequence);
        Assert.True(consequence.DestroyItem);
        _ = Assert.Single(consequence.CreatedItems);
        Assert.Equal("The glass shatters!", consequence.Message);
    }

    [Fact]
    public void OnUse_SetsConsequence()
    {
        Item potion = new("potion", "health potion", "A red potion.");

        _ = potion.SetUseConsequence(ActionConsequence.Destroy("You drink the potion. It tastes like cherries."));

        Assert.True(potion.HasConsequence(ItemAction.Use));
        ActionConsequence? consequence = potion.GetConsequence(ItemAction.Use);
        Assert.NotNull(consequence);
        Assert.True(consequence.DestroyItem);
    }

    [Fact]
    public void OnTake_SetsConsequence()
    {
        Item trap = new("trap", "bear trap", "A rusty bear trap.");

        _ = trap.SetTakeConsequence(new ActionConsequence
        {
            Message = "SNAP! The trap springs shut!",
            SetFlag = "injured"
        });

        Assert.True(trap.HasConsequence(ItemAction.Take));
    }

    [Fact]
    public void MakeFragile_SetsDropConsequence()
    {
        Item vase = new("vase", "ming vase", "An ancient vase.");
        Item shards = new("shards", "ceramic shards", "Broken pieces of the vase.");

        _ = vase.MakeFragile("The vase shatters into a thousand pieces!", shards);

        Assert.True(vase.HasConsequence(ItemAction.Drop));
        ActionConsequence? consequence = vase.GetConsequence(ItemAction.Drop);
        Assert.NotNull(consequence);
        Assert.True(consequence.DestroyItem);
        _ = Assert.Single(consequence.CreatedItems);
        Assert.Equal("shards", consequence.CreatedItems[0].Id);
    }

    [Fact]
    public void MakeConsumable_SetsUseConsequence()
    {
        Item apple = new("apple", "red apple", "A juicy red apple.");

        _ = apple.MakeConsumable("You eat the apple. Delicious!");

        Assert.True(apple.HasConsequence(ItemAction.Use));
        ActionConsequence? consequence = apple.GetConsequence(ItemAction.Use);
        Assert.NotNull(consequence);
        Assert.True(consequence.DestroyItem);
        Assert.Equal("You eat the apple. Delicious!", consequence.Message);
    }

    [Fact]
    public void GetConsequence_ReturnsNull_WhenNoConsequence()
    {
        Item rock = new("rock", "rock", "A plain rock.");

        ActionConsequence? consequence = rock.GetConsequence(ItemAction.Drop);

        Assert.Null(consequence);
    }

    [Fact]
    public void HasConsequence_ReturnsFalse_WhenNoConsequence()
    {
        Item rock = new("rock", "rock", "A plain rock.");

        Assert.False(rock.HasConsequence(ItemAction.Drop));
        Assert.False(rock.HasConsequence(ItemAction.Use));
        Assert.False(rock.HasConsequence(ItemAction.Take));
    }

    [Fact]
    public void ActionConsequence_Transform_CreatesCorrectConsequence()
    {
        _ = new Item("caterpillar", "caterpillar", "A fuzzy caterpillar.");
        Item butterfly = new("butterfly", "butterfly", "A beautiful butterfly.");

        ActionConsequence consequence = ActionConsequence.Transform(butterfly, "The caterpillar transforms into a butterfly!");

        Assert.True(consequence.DestroyItem);
        _ = Assert.Single(consequence.CreatedItems);
        Assert.Equal("butterfly", consequence.CreatedItems[0].Id);
        Assert.Equal("The caterpillar transforms into a butterfly!", consequence.Message);
    }

    [Fact]
    public void ActionConsequence_Break_CreatesMultiplePieces()
    {
        _ = new Item("mirror", "mirror", "A magic mirror.");
        Item shard1 = new("shard1", "mirror shard", "A piece of the mirror.");
        Item shard2 = new("shard2", "mirror shard", "Another piece of the mirror.");

        ActionConsequence consequence = ActionConsequence.Break("The mirror breaks!", shard1, shard2);

        Assert.True(consequence.DestroyItem);
        Assert.Equal(2, consequence.CreatedItems.Count);
    }
}
