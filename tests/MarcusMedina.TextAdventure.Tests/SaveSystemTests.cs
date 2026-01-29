// <copyright file="SaveSystemTests.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
namespace MarcusMedina.TextAdventure.Tests;

using MarcusMedina.TextAdventure.Engine;
using MarcusMedina.TextAdventure.Models;

public class SaveSystemTests
{
    [Fact]
    public void JsonSaveSystem_RoundTripsMemento()
    {
        var saveSystem = new JsonSaveSystem();
        var tempFile = Path.GetTempFileName();

        try
        {
            var memento = new GameMemento(
                currentLocationId: "cabin",
                inventoryItemIds: new[] { "sword", "key" },
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

    [Fact]
    public void GameState_ApplyMemento_RestoresLocationAndInventory()
    {
        var start = new Location("start");
        var cabin = new Location("cabin");
        var sword = new Item("sword", "sword");
        cabin.AddItem(sword);

        var state = new GameState(start, worldLocations: new[] { start, cabin });
        var memento = new GameMemento(
            currentLocationId: "cabin",
            inventoryItemIds: new[] { "sword" },
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
}
