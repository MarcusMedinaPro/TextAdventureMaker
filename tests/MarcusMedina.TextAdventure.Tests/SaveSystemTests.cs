// <copyright file="SaveSystemTests.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace MarcusMedina.TextAdventure.Tests;

using MarcusMedina.TextAdventure.Engine;
using MarcusMedina.TextAdventure.Models;

public class SaveSystemTests
{
    private static readonly string[] CabinInventory = ["sword"];
    private static readonly string[] CabinInventoryWithKey = ["sword", "key"];

    [Fact]
    public void GameState_ApplyMemento_RestoresLocationAndInventory()
    {
        Location start = new("start");
        Location cabin = new("cabin");
        Item sword = new("sword", "sword");
        cabin.AddItem(sword);

        GameState state = new(start, worldLocations: [start, cabin]);
        GameMemento memento = new(
            currentLocationId: "cabin",
            inventoryItemIds: CabinInventory,
            health: 80,
            maxHealth: 100,
            flags: [],
            counters: [],
            relationships: [],
            timeline: []);

        state.ApplyMemento(memento);

        Assert.Equal("cabin", state.CurrentLocation.Id);
        Assert.Contains(state.Inventory.Items, item => item.Id == "sword");
        Assert.Equal(80, state.Stats.Health);
    }

    [Fact]
    public void JsonSaveSystem_RoundTripsMemento()
    {
        JsonSaveSystem saveSystem = new();
        var tempFile = Path.GetTempFileName();

        try
        {
            GameMemento memento = new(
                currentLocationId: "cabin",
                inventoryItemIds: CabinInventoryWithKey,
                health: 50,
                maxHealth: 100,
                flags: new Dictionary<string, bool> { ["dragon_defeated"] = true },
                counters: new Dictionary<string, int> { ["days"] = 2 },
                relationships: new Dictionary<string, int> { ["fox"] = 3 },
                timeline: ["Entered cave."]);

            saveSystem.Save(tempFile, memento);
            var loaded = saveSystem.Load(tempFile);

            Assert.Equal("cabin", loaded.CurrentLocationId);
            Assert.Contains("sword", loaded.InventoryItemIds);
            Assert.Equal(50, loaded.Health);
            Assert.True(loaded.Flags["dragon_defeated"]);
        }
        finally
        {
            File.Delete(tempFile);
        }
    }
}
