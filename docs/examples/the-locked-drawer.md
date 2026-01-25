# The Locked Drawer

_Slice tag: Slice 2 — Doors + Keys. Demo focuses on locked doors and keys blocking progress._

A tiny, focused demo with one room, a desk, a locked drawer, a photo, and a secret.

## Story beats (max ~10 steps)
1) You enter a quiet study.
2) You notice a desk with a locked drawer.
3) Find the key under the blotter.
4) Unlock the drawer.
5) Discover a photo.
6) Notice the note on the back.

## Example (core engine + doors/keys only)
```csharp
using MarcusMedina.TextAdventure.Engine;
using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Models;

// Location
var study = new Location("study", "A quiet study with a heavy desk and a single lamp.");

// Items
var key = new Key("drawer_key", "small key", "A small brass key.")
    .SetWeight(0.1f);
var photo = new Item("photo", "old photo", "A faded photo of a stranger.");

study.AddItem(key);

// Door-as-drawer (locked)
var drawer = new Door("drawer", "drawer", "A locked desk drawer.")
    .RequiresKey(key)
    .SetReaction(DoorAction.Unlock, "The drawer clicks open.");

// Exit as a fake container gate
study.AddExit(Direction.In, study, drawer);

// Game state
var state = new GameState(study, worldLocations: new[] { study });

// Simple flow (no commands yet in Slice 2)
Console.WriteLine("You find a small key under the blotter.");
var unlocked = drawer.Unlock(key);
if (unlocked)
{
    drawer.Open();
    study.AddItem(photo);
    Console.WriteLine("Inside the drawer lies an old photo.");
    Console.WriteLine("On the back: 'Meet me at the pier.'");
}
```
