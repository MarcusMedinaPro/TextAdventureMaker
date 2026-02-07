// <copyright file="SaveLoadCommandTests.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using MarcusMedina.TextAdventure.Commands;
using MarcusMedina.TextAdventure.Engine;
using MarcusMedina.TextAdventure.Interfaces;
using MarcusMedina.TextAdventure.Models;

namespace MarcusMedina.TextAdventure.Tests;

public class SaveLoadCommandTests
{
    private sealed class StubSaveSystem : ISaveSystem
    {
        public string? LastPath { get; private set; }
        public GameMemento? LastMemento { get; private set; }

        public void Save(string path, GameMemento memento)
        {
            LastPath = path;
            LastMemento = memento;
        }

        public GameMemento Load(string path)
        {
            LastPath = path;
            return LastMemento ?? new GameMemento(
                "start",
                Array.Empty<string>(),
                100,
                100,
                new Dictionary<string, bool>(),
                new Dictionary<string, int>(),
                new Dictionary<string, int>(),
                []);
        }
    }

    [Fact]
    public void SaveCommand_UsesSaveSystem()
    {
        StubSaveSystem saveSystem = new();
        GameState state = new(new Location("start"), saveSystem: saveSystem);

        _ = new SaveCommand("slot1.json").Execute(new CommandContext(state));

        Assert.Equal("slot1.json", saveSystem.LastPath);
        Assert.NotNull(saveSystem.LastMemento);
    }

    [Fact]
    public void LoadCommand_UsesSaveSystem()
    {
        StubSaveSystem saveSystem = new();
        GameState state = new(new Location("start"), saveSystem: saveSystem);

        _ = new LoadCommand("slot1.json").Execute(new CommandContext(state));

        Assert.Equal("slot1.json", saveSystem.LastPath);
    }
}
