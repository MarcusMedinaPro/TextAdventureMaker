using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Interfaces;

namespace MarcusMedina.TextAdventure.Models;

public class Door : IDoor
{
    public string Id { get; }
    public string Name { get; }
    public DoorState State { get; private set; }
    public IKey? RequiredKey { get; private set; }

    public bool IsPassable => State == DoorState.Open || State == DoorState.Destroyed;

    public Door(string id, string name, DoorState initialState = DoorState.Closed)
    {
        Id = id;
        Name = name;
        State = initialState;
    }

    public Door RequiresKey(IKey key)
    {
        RequiredKey = key;
        State = DoorState.Locked;
        return this;
    }

    public bool Open()
    {
        if (State == DoorState.Locked) return false;
        if (State == DoorState.Destroyed) return true;
        State = DoorState.Open;
        return true;
    }

    public bool Close()
    {
        if (State == DoorState.Destroyed) return false;
        if (State == DoorState.Locked) return false;
        State = DoorState.Closed;
        return true;
    }

    public bool Lock(IKey key)
    {
        if (State == DoorState.Destroyed) return false;
        if (State == DoorState.Open) return false;
        if (RequiredKey != null && RequiredKey.Id != key.Id) return false;
        RequiredKey = key;
        State = DoorState.Locked;
        return true;
    }

    public bool Unlock(IKey key)
    {
        if (State != DoorState.Locked) return false;
        if (RequiredKey == null || RequiredKey.Id != key.Id) return false;
        State = DoorState.Closed;
        return true;
    }

    public bool Destroy()
    {
        State = DoorState.Destroyed;
        return true;
    }
}
