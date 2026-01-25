using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Extensions;
using MarcusMedina.TextAdventure.Interfaces;
using MarcusMedina.TextAdventure.Localization;
using MarcusMedina.TextAdventure.Models;

namespace MarcusMedina.TextAdventure.Engine;

public class GameState : IGameState
{
    public ILocation CurrentLocation { get; private set; }
    public string? LastMoveError { get; private set; }
    public GameError LastMoveErrorCode { get; private set; }
    public IStats Stats { get; }
    public IInventory Inventory { get; }
    public RecipeBook RecipeBook { get; }
    public IEventSystem Events { get; }
    public ICombatSystem CombatSystem { get; }
    public IWorldState WorldState { get; }

    public GameState(
        ILocation startLocation,
        IStats? stats = null,
        IInventory? inventory = null,
        RecipeBook? recipeBook = null,
        IEventSystem? eventSystem = null,
        ICombatSystem? combatSystem = null,
        IWorldState? worldState = null)
    {
        ArgumentNullException.ThrowIfNull(startLocation);
        CurrentLocation = startLocation;
        Stats = stats ?? new Stats(100);
        Inventory = inventory ?? new Inventory();
        RecipeBook = recipeBook ?? new RecipeBook();
        Events = eventSystem ?? new EventSystem();
        CombatSystem = combatSystem ?? new TurnBasedCombat();
        WorldState = worldState ?? new WorldState();
    }

    public bool Move(Direction direction)
    {
        LastMoveError = null;
        LastMoveErrorCode = GameError.None;
        var exit = CurrentLocation.GetExit(direction);

        if (exit == null)
        {
            LastMoveError = Language.CantGoThatWay;
            LastMoveErrorCode = GameError.NoExitInDirection;
            return false;
        }

        if (!exit.IsPassable)
        {
            if (exit.Door?.State == DoorState.Locked)
            {
                LastMoveError = Language.DoorLocked(exit.Door.Name);
                LastMoveErrorCode = GameError.DoorIsLocked;
            }
            else
            {
                LastMoveError = Language.DoorClosed(exit.Door!.Name);
                LastMoveErrorCode = GameError.DoorIsClosed;
            }
            return false;
        }

        var previousLocation = CurrentLocation;
        Events.Publish(new GameEvent(GameEventType.ExitLocation, this, previousLocation));
        CurrentLocation = exit.Target;
        Events.Publish(new GameEvent(GameEventType.EnterLocation, this, CurrentLocation));
        return true;
    }

    public bool IsCurrentRoomId(string id)
    {
        if (string.IsNullOrWhiteSpace(id)) return false;
        return CurrentLocation.Id.TextCompare(id);
    }
}
