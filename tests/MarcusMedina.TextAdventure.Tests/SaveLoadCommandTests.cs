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
                new List<string>());
        }
    }

    [Fact]
    public void SaveCommand_UsesSaveSystem()
    {
        var saveSystem = new StubSaveSystem();
        var state = new GameState(new Location("start"), saveSystem: saveSystem);

        new SaveCommand("slot1.json").Execute(new CommandContext(state));

        Assert.Equal("slot1.json", saveSystem.LastPath);
        Assert.NotNull(saveSystem.LastMemento);
    }

    [Fact]
    public void LoadCommand_UsesSaveSystem()
    {
        var saveSystem = new StubSaveSystem();
        var state = new GameState(new Location("start"), saveSystem: saveSystem);

        new LoadCommand("slot1.json").Execute(new CommandContext(state));

        Assert.Equal("slot1.json", saveSystem.LastPath);
    }
}
