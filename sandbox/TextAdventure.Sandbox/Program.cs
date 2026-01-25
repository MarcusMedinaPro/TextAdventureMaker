// <copyright file="Program.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
using MarcusMedina.TextAdventure.Models;
using MarcusMedina.TextAdventure.Engine;
using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Commands;
using MarcusMedina.TextAdventure.Parsing;
using MarcusMedina.TextAdventure.Extensions;
using MarcusMedina.TextAdventure.Helpers;
using MarcusMedina.TextAdventure.Localization;
using MarcusMedina.TextAdventure.Interfaces;
using MarcusMedina.TextAdventure.Dsl;

var languagePath = Path.Combine(AppContext.BaseDirectory, "lang", "gamelang.sv.txt");
if (File.Exists(languagePath))
{
    Language.SetProvider(new FileLanguageProvider(languagePath));
}

var adventurePath = Path.Combine(AppContext.BaseDirectory, "forest.adventure");
var adventure = new AdventureDslParser().ParseFile(adventurePath);
var state = adventure.State;

// Locations from DSL
var entrance = adventure.Locations["entrance"];
var forest = adventure.Locations["forest"];
var cave = adventure.Locations["cave"];
var deepCave = adventure.Locations["deep_cave"];
var clearing = adventure.Locations["clearing"];
var cabin = adventure.Locations["cabin"];
var shed = adventure.Locations["shed"];

// Items and keys from DSL
var ice = adventure.Items["ice"];
var sign = adventure.Items["sign"];
var fire = adventure.Items["fire"];
var letter = adventure.Items["letter"];
var glass = adventure.Items["glass"];
var tome = adventure.Items["tome"];
var newspaper = adventure.Items["newspaper"];

ice.SetReaction(ItemAction.Take, "The cold chills your hand.")
    .SetReaction(ItemAction.Drop, "It lands with a soft thump.")
    .SetReaction(ItemAction.Use, "You take a bite. Your teeth ache.");

sign.SetTakeable(false)
    .SetReadable()
    .SetReadText("Welcome to the Dark Forest!");

fire.SetWeight(0.5f);

letter.SetReadable()
    .RequireTakeToRead()
    .RequiresToRead(s => s.Inventory.Items.Any(i => i.Id == "lantern"))
    .SetReadText("🎶 Meet me at midnight... 🎵");

glass.SetReaction(ItemAction.Take, "The glas surface is smooth")
    .SetReaction(ItemAction.Drop, "The glass bounces on the floor")
    .SetReaction(ItemAction.Destroy, "The glass shatters into 1000 pieces");

tome.SetReadable()
    .RequireTakeToRead()
    .SetReadingCost(3)
    .SetReadText("The secret to defeating the dragon is...");

newspaper.SetReadable()
    .RequireTakeToRead()
    .SetReadText("HEADLINE: Dragon spotted near village!");

// Extras (still showing different creation styles)
var extraItems = new GameItemList()
    .AddMany("Cat", "Rubber chicken", "Map", "Shovel");
extraItems["cat"].SetWeight(3f).AddAliases("kitten", "kitteh");
extraItems["rubber chicken"].SetWeight(0.8f).AddAliases("chicken", "rubberchicken");
extraItems["map"].SetWeight(0.2f).AddAliases("parchment", "chart");
extraItems["shovel"].SetWeight(2.5f).AddAliases("spade");

var itemList = new ItemList()
    .AddMany("Compass", "Blanket");
itemList["compass"].SetWeight(0.3f).AddAliases("northfinder");
itemList["blanket"].SetWeight(1.0f);

var keyList = new KeyList()
    .AddMany("watchtower key");
keyList["watchtower key"].SetWeight(0.2f).AddAliases("watchkey");

var doorList = new DoorList()
    .AddMany("watchtower door");
doorList["watchtower door"]
    .RequiresKey(keyList["watchtower key"])
    .SetReaction(DoorAction.Unlock, "The watchtower door unlocks with a click.");

var locationList = new LocationList();
var watchtower = locationList.Add("watchtower", "A wind-bitten watchtower overlooking the forest.");
var garden = locationList.Add("garden", "A quiet garden. A flat stone rests near an old gate.");
var courtyard = locationList.Add("courtyard", "A small courtyard where something waits.");
var attic = locationList.Add("attic", "Rain drums against the roof. A leak gathers overhead.");
var office = locationList.Add("office", "A quiet office with a locked terminal.");
var libraryOutside = locationList.Add("library_outside", "Snow falls quietly outside a locked library.");
var library = locationList.Add("library", "Warm light and quiet pages surround you.");
var meeting = locationList.Add("meeting", "A small meeting room. A mirror hangs by the door.");
var cafe = locationList.Add("cafe", "A warm café with soft light and a small table.");
var bankLobby = locationList.Add("bank_lobby", "A quiet bank lobby with a ticket machine.");
var bankCounter = locationList.Add("bank_counter", "A teller waits behind the counter.");
var alley = locationList.Add("alley", "A dim alley. Footsteps echo behind you.");
var interviewLobby = locationList.Add("interview_lobby", "A calm lobby with a glass of water.");
var interviewRoom = locationList.Add("interview_room", "A quiet interview room with two chairs.");
var stationHall = locationList.Add("station_hall", "A quiet station hall with a flickering departure board.");
var platform = locationList.Add("platform", "The last train's lights disappear into the night.");
var sideStreet = locationList.Add("side_street", "A side street with rain-slick pavement.");
var busStop = locationList.Add("bus_stop", "A lonely bus stop with a torn schedule.");
var footbridge = locationList.Add("footbridge", "A narrow footbridge over the tracks.");
var taxiStand = locationList.Add("taxi_stand", "A small taxi stand with no cabs in sight.");
var hospitalEntrance = locationList.Add("hospital_entrance", "Automatic doors slide open to a bright lobby.");
var reception = locationList.Add("reception", "A reception desk with a stack of forms.");
var waitingRoom = locationList.Add("waiting_room", "Plastic chairs and a quiet TV loop.");
var examRoom = locationList.Add("exam_room", "A clean exam room with a curtained bed.");
var roadside = locationList.Add("roadside", "Your car sits dead on the shoulder with the hood up.");
var gasStation = locationList.Add("gas_station", "A small gas station with a service bay.");
var apartmentLobby = locationList.Add("apartment_lobby", "A tidy lobby with a directory and fresh paint.");
var apartmentUnit = locationList.Add("apartment_unit", "A bright unit with tall windows and clean floors.");
var balcony = locationList.Add("balcony", "A small balcony overlooking the street.");
var bar = locationList.Add("bar", "A noisy bar with clinking glasses.");
var barAlley = locationList.Add("bar_alley", "A narrow alley behind the bar.");
var nightStreet = locationList.Add("night_street", "A long street with patchy streetlights.");
var underpass = locationList.Add("underpass", "A shadowy underpass humming with distant traffic.");
var frontPorch = locationList.Add("front_porch", "A quiet front porch with a locked gate.");

entrance.AddItem(extraItems["map"]);
entrance.AddItem(keyList["watchtower key"]);
forest.AddItem(extraItems["cat"]);
clearing.AddItem(extraItems["rubber chicken"]);
shed.AddItem(extraItems["shovel"]);
watchtower.AddItem(itemList["compass"]);
cabin.AddItem(itemList["blanket"]);
garden.AddItem(new Item("stone", "stone", "A heavy flat stone."));
attic.AddItem(new Item("bucket", "bucket", "A metal bucket."));
office.AddItem(new Item("note", "post-it note", "A note with a hint: 0420."));
var coffeeCup = new Item("coffee", "coffee", "A hot cup of coffee.")
    .SetHint("This cup would look nicer with some coffee in it");
office.AddItem(coffeeCup);
office.AddItem(new Item("papers", "papers", "Notes for the meeting."));
meeting.AddItem(new Item("mirror", "mirror", "A mirror for a quick check.").SetTakeable(false));
cafe.AddItem(new Item("menu", "menu", "A small menu with handwritten specials.").SetTakeable(false));
cafe.AddItem(new Item("coffee_cup", "coffee", "A fresh cup of coffee."));
bankLobby.AddItem(new Item("ticket", "number ticket", "Your place in line."));
alley.AddItem(new Item("coin", "coin", "A single coin with a dull shine."));
var libraryKey = new Key("library_key", "library key", "Cold metal in your hand.")
    .SetHint("Hmm, what do we usually use keys for...duh");
courtyard.AddItem(libraryKey);
stationHall.AddItem(new Item("board", "departure board", "The next train is marked CANCELLED.").SetTakeable(false));
stationHall.AddItem(new Item("ticket_stub", "ticket stub", "The punch marks show you were late."));
busStop.AddItem(new Item("schedule", "schedule", "Next bus in 20 minutes.").SetTakeable(false));
taxiStand.AddItem(new Item("sign", "rideshare sign", "Maybe a rideshare app could work.").SetTakeable(false));
reception.AddItem(new Item("forms", "intake forms", "Paperwork asking the usual questions."));
reception.AddItem(new Item("clipboard", "clipboard", "A clipboard for check-in."));
waitingRoom.AddItem(new Item("magazine", "magazine", "Outdated magazines and a crossword."));
examRoom.AddItem(new Item("results", "test results", "The results are normal.").SetTakeable(false));
roadside.AddItem(new Item("phone", "phone", "Low battery, but still works."));
gasStation.AddItem(new Item("wrench", "wrench", "A sturdy wrench for a stubborn bolt."));
gasStation.AddItem(new Item("jack", "jack", "A heavy jack for lifting the car."));
apartmentLobby.AddItem(new Item("brochure", "brochure", "A brochure listing amenities and fees."));
apartmentUnit.AddItem(new Item("inspection", "inspection list", "A checklist of items to review."));
balcony.AddItem(new Item("view", "view", "The city lights shimmer below.").SetTakeable(false));
bar.AddItem(new Item("stool", "bar stool", "A sturdy stool bolted to the floor.").SetTakeable(false));
bar.AddItem(new Item("glass", "glass", "An empty glass with a chipped rim."));
barAlley.AddItem(new Item("poster", "poster", "A torn poster flaps in the wind.").SetTakeable(false));
nightStreet.AddItem(new Item("streetlight", "streetlight", "A flickering streetlight buzzes overhead.").SetTakeable(false));
underpass.AddItem(new Item("whistle", "whistle", "A small whistle on a frayed cord."));
frontPorch.AddItem(new Item("gate", "gate", "The gate is locked, but the house lights are on.").SetTakeable(false));

forest.AddExit(Direction.NorthEast, watchtower, doorList["watchtower door"]);
clearing.AddExit(Direction.South, garden);
cabin.AddExit(Direction.Up, attic);
cabin.AddExit(Direction.East, office);
cabin.AddExit(Direction.North, meeting);
courtyard.AddExit(Direction.North, libraryOutside);
library.AddExit(Direction.East, cafe);
library.AddExit(Direction.North, bankLobby);
bankLobby.AddExit(Direction.North, bankCounter);
bankCounter.AddExit(Direction.Out, alley);
bankCounter.AddExit(Direction.East, interviewLobby);
interviewLobby.AddExit(Direction.In, interviewRoom);
cafe.AddExit(Direction.South, stationHall);
stationHall.AddExit(Direction.East, platform);
stationHall.AddExit(Direction.West, sideStreet);
sideStreet.AddExit(Direction.South, busStop);
sideStreet.AddExit(Direction.West, taxiStand);
platform.AddExit(Direction.North, footbridge);
footbridge.AddExit(Direction.East, busStop);
busStop.AddExit(Direction.South, hospitalEntrance);
hospitalEntrance.AddExit(Direction.In, reception);
reception.AddExit(Direction.East, waitingRoom);
waitingRoom.AddExit(Direction.North, examRoom);
taxiStand.AddExit(Direction.South, roadside);
roadside.AddExit(Direction.East, gasStation);
gasStation.AddExit(Direction.North, apartmentLobby);
apartmentLobby.AddExit(Direction.In, apartmentUnit);
apartmentUnit.AddExit(Direction.East, balcony);
sideStreet.AddExit(Direction.East, bar);
bar.AddExit(Direction.South, barAlley);
barAlley.AddExit(Direction.East, nightStreet);
nightStreet.AddExit(Direction.East, underpass);
underpass.AddExit(Direction.North, frontPorch);

var gardenKey = new Key("garden_key", "iron key", "A small iron key.")
    .SetHint("Hmm, what do we usually use keys for...duh");
var gardenGate = new Door("garden_gate", "garden gate", "An old iron gate.")
    .RequiresKey(gardenKey)
    .SetReaction(DoorAction.Unlock, "The gate creaks open.")
    .SetHint("It needs a key, duh");
garden.AddExit(Direction.Out, courtyard, gardenGate);

var libraryDoor = new Door("library_door", "library door", "A heavy wooden door.")
    .RequiresKey(libraryKey)
    .SetReaction(DoorAction.Unlock, "The library door unlocks.")
    .SetHint("It needs a key, duh");
libraryOutside.AddExit(Direction.In, library, libraryDoor);

coffeeCup.OnUse += _ => coffeeCup.SetHint("Yum! Coffee good!");

// Doors from DSL
var cabinDoor = adventure.Doors["cabin_door"];
var shedDoor = adventure.Doors["shed_door"];

cabinDoor
    .SetReaction(DoorAction.Unlock, "The lock clicks open.")
    .SetReaction(DoorAction.Open, "The door creaks as it swings wide.");

shedDoor.SetReaction(DoorAction.Unlock, "The shed door unlocks with a click.");

// Register extra locations for save/load
state.RegisterLocations(new[] { watchtower, garden, courtyard, attic, office, libraryOutside, library, meeting, cafe, bankLobby, bankCounter, alley, interviewLobby, interviewRoom, stationHall, platform, sideStreet, busStop, footbridge, taxiStand, hospitalEntrance, reception, waitingRoom, examRoom, roadside, gasStation, apartmentLobby, apartmentUnit, balcony, bar, barAlley, nightStreet, underpass, frontPorch });

// Create NPCs
var npcList = new NpcList()
    .AddMany("fox", "dragon", "storm", "date", "teller", "mugger", "interviewer", "receptionist", "nurse", "mechanic", "agent", "bouncer", "brawler", "stranger");
var fox = npcList["fox"]
    .Description("A curious fox with bright eyes.")
    .SetDialog(new DialogNode("The fox tilts its head, listening.")
        .AddOption("Ask about the cabin")
        .AddOption("Ask about the cave"));

var dragonPatrol = new PatrolNpcMovement(new[] { cave, deepCave });
var dragon = npcList["dragon"]
    .SetState(NpcState.Friendly)
    .SetStats(new Stats(40))
    .Description("A massive dragon sleeps among the shadows.")
    .Dialog("The dragon snores softly.")
    .SetMovement(new NoNpcMovement());

var storm = npcList["storm"]
    .SetState(NpcState.Hostile)
    .SetStats(new Stats(12))
    .Description("A relentless leak you must endure.")
    .SetMovement(new NoNpcMovement());

var date = npcList["date"]
    .SetState(NpcState.Friendly)
    .Description("A calm smile across the table.")
    .SetDialog(new DialogNode("The café is quiet. They wait for your first words.")
        .AddOption("Ask about their day")
        .AddOption("Compliment their outfit")
        .AddOption("Order coffee"));

var teller = npcList["teller"]
    .SetState(NpcState.Friendly)
    .Description("A patient teller taps the desk.")
    .SetDialog(new DialogNode("How can I help you today?")
        .AddOption("Fix my account issue")
        .AddOption("Ask about fees"));

var mugger = npcList["mugger"]
    .SetState(NpcState.Hostile)
    .SetStats(new Stats(15))
    .Description("A shadow steps forward, hand in pocket.")
    .SetDialog(new DialogNode("Give me your wallet.")
        .AddOption("Try to talk your way out")
        .AddOption("Throw a coin and run"));

var interviewer = npcList["interviewer"]
    .SetState(NpcState.Friendly)
    .Description("An interviewer with a kind smile.")
    .SetDialog(new DialogNode("Thanks for coming. Ready to begin?")
        .AddOption("Talk about your experience")
        .AddOption("Ask about the team")
        .AddOption("Admit you're nervous"));

var receptionist = npcList["receptionist"]
    .SetState(NpcState.Friendly)
    .Description("A receptionist with a calm voice.")
    .SetDialog(new DialogNode("Can I get your name and date of birth?")
        .AddOption("Hand over the clipboard")
        .AddOption("Ask about the wait time"));

var nurse = npcList["nurse"]
    .SetState(NpcState.Friendly)
    .Description("A nurse taps a tablet, waiting for you.")
    .SetDialog(new DialogNode("We can bring you in now.")
        .AddOption("Follow to the exam room")
        .AddOption("Ask about the tests"));

var mechanic = npcList["mechanic"]
    .SetState(NpcState.Friendly)
    .Description("A mechanic wipes grease from their hands.")
    .SetDialog(new DialogNode("Need a tool or a lift?")
        .AddOption("Ask for a wrench")
        .AddOption("Ask for a jack"));

var agent = npcList["agent"]
    .SetState(NpcState.Friendly)
    .Description("An agent waits with a small stack of keys.")
    .SetDialog(new DialogNode("Any questions about the unit?")
        .AddOption("Ask about noise levels")
        .AddOption("Ask about utilities")
        .AddOption("Ask about move-in dates"));

var bouncer = npcList["bouncer"]
    .SetState(NpcState.Friendly)
    .Description("A bouncer watches the room with folded arms.")
    .SetDialog(new DialogNode("Keep it calm in here.")
        .AddOption("Back off")
        .AddOption("Ask for help"));

var brawler = npcList["brawler"]
    .SetState(NpcState.Hostile)
    .SetStats(new Stats(10))
    .Description("A restless patron looks for a fight.")
    .SetDialog(new DialogNode("What are you looking at?")
        .AddOption("Apologize and step away")
        .AddOption("Stand your ground"));

var stranger = npcList["stranger"]
    .SetState(NpcState.Hostile)
    .SetStats(new Stats(8))
    .Description("A stranger lingers in the shadows.")
    .SetDialog(new DialogNode("You lost?")
        .AddOption("Keep walking")
        .AddOption("Ask for directions"));

forest.AddNpc(fox);
cave.AddNpc(dragon);
attic.AddNpc(storm);
cafe.AddNpc(date);
bankCounter.AddNpc(teller);
alley.AddNpc(mugger);
interviewRoom.AddNpc(interviewer);
reception.AddNpc(receptionist);
examRoom.AddNpc(nurse);
gasStation.AddNpc(mechanic);
apartmentUnit.AddNpc(agent);
bar.AddNpc(bouncer);
bar.AddNpc(brawler);
nightStreet.AddNpc(stranger);

// Recipes
state.RecipeBook.Add(new ItemCombinationRecipe("ice", "fire", () => new FluidItem("water", "water", "Clear and cold.")));
var dragonAwake = false;
var missedTrainNotified = false;
var hospitalCalled = false;
var barTensionNotified = false;
var nightWalkNotified = false;
var dragonHunt = new Quest("dragon_hunt", "Dragon Hunt", "Find the sword and slay the dragon.")
    .AddCondition(new HasItemCondition("sword"))
    .AddCondition(new NpcStateCondition(dragon, NpcState.Dead))
    .AddCondition(new WorldFlagCondition("dragon_defeated"))
    .AddCondition(new WorldCounterCondition("villagers_saved", 1))
    .AddCondition(new RelationshipCondition("fox", 2))
    .Start();
var loginQuest = new Quest("login", "Access the Terminal", "Find the password hint and log in.")
    .AddCondition(new HasItemCondition("note"))
    .Start();
var loginQuestComplete = false;

state.Events.Subscribe(GameEventType.EnterLocation, e =>
{
    if (dragonAwake) return;
    if (e.Location != null && e.Location.Id.TextCompare("cave"))
    {
        dragonAwake = true;
        dragon.SetState(NpcState.Hostile);
        dragon.Description("A massive dragon rises, eyes blazing.");
        dragon.Dialog("The dragon roars as it wakes.");
        dragon.SetMovement(dragonPatrol);
        Console.WriteLine("\nThe dragon stirs and awakens!");
    }
});

state.Events.Subscribe(GameEventType.EnterLocation, e =>
{
    if (missedTrainNotified) return;
    if (e.Location != null && e.Location.Id.TextCompare("platform"))
    {
        missedTrainNotified = true;
        Console.WriteLine("\nThe train pulls away as you arrive. You'll need another route.");
    }
});

state.Events.Subscribe(GameEventType.EnterLocation, e =>
{
    if (hospitalCalled) return;
    if (e.Location != null && e.Location.Id.TextCompare("waiting_room"))
    {
        hospitalCalled = true;
        Console.WriteLine("\nA nurse calls your name from the hall.");
    }
});

state.Events.Subscribe(GameEventType.EnterLocation, e =>
{
    if (barTensionNotified) return;
    if (e.Location != null && e.Location.Id.TextCompare("bar"))
    {
        barTensionNotified = true;
        Console.WriteLine("\nThe room goes quiet for a moment. Tension hangs in the air.");
    }
});

state.Events.Subscribe(GameEventType.EnterLocation, e =>
{
    if (nightWalkNotified) return;
    if (e.Location != null && e.Location.Id.TextCompare("night_street"))
    {
        nightWalkNotified = true;
        Console.WriteLine("\nYour footsteps echo. The street feels longer at night.");
    }
});

state.Events.Subscribe(GameEventType.PickupItem, e =>
{
    if (e.Item == null) return;
    if (e.Item.Id.TextCompare("stone"))
    {
        if (!garden.Items.Contains(gardenKey) && !state.Inventory.Items.Contains(gardenKey))
        {
            garden.AddItem(gardenKey);
            Console.WriteLine("You lift the stone and find a key beneath it.");
        }
    }
});

state.Events.Subscribe(GameEventType.UnlockDoor, e =>
{
    if (e.Door == null) return;
    if (e.Door.Id.TextCompare("garden_gate") || e.Door.Id.TextCompare("library_door"))
    {
        e.Door.SetHint("It's a nice unlocked door.");
    }
});

state.Events.Subscribe(GameEventType.EnterLocation, e =>
{
    state.WorldState.Increment("days_elapsed");
    if (e.Location != null)
    {
        state.WorldState.AddTimeline($"Entered {e.Location.Id}.");
    }
});

state.Events.Subscribe(GameEventType.PickupItem, e =>
{
    if (e.Item == null) return;
    if (e.Item.Id == "apple")
    {
        state.WorldState.Increment("villagers_saved");
    }
});

state.Events.Subscribe(GameEventType.TalkToNpc, e =>
{
    if (e.Npc == null) return;
    if (e.Npc.Id == "fox")
    {
        var reputation = state.WorldState.GetRelationship("fox") + 1;
        state.WorldState.SetRelationship("fox", reputation);
        if (reputation >= 2)
        {
            e.Npc.SetDialog(new DialogNode("The fox seems to trust you now.")
                .AddOption("Ask about the dragon")
                .AddOption("Ask about the shed"));
        }
    }
});

Console.WriteLine("=== FOREST ADVENTURE ===");
Console.WriteLine("Find the key and unlock the cabin!");
Console.WriteLine($"Quest started: {dragonHunt.Title} - {dragonHunt.Description}");
var commands = new[]
{
    "go", "look", "talk", "attack", "flee", "read", "open", "unlock", "take", "drop", "use", "inventory", "stats", "combine", "pour", "save", "load", "quit"
};
Console.WriteLine($"Commands: {commands.CommaJoin()} (or just type a direction)\n");

var parserConfig = new KeywordParserConfig(
    quit: CommandHelper.NewCommands("quit", "exit", "q"),
    look: CommandHelper.NewCommands("look", "l", "ls"),
    inventory: CommandHelper.NewCommands("inventory", "inv", "i"),
    stats: CommandHelper.NewCommands("stats", "stat", "hp", "health"),
    open: CommandHelper.NewCommands("open"),
    unlock: CommandHelper.NewCommands("unlock"),
    take: CommandHelper.NewCommands("take", "get", "pickup", "pick"),
    drop: CommandHelper.NewCommands("drop"),
    use: CommandHelper.NewCommands("use", "eat", "bite"),
    combine: CommandHelper.NewCommands("combine", "mix"),
    pour: CommandHelper.NewCommands("pour"),
    go: CommandHelper.NewCommands("go", "move", "cd"),
    read: CommandHelper.NewCommands("read"),
    talk: CommandHelper.NewCommands("talk", "speak"),
    attack: CommandHelper.NewCommands("attack", "fight"),
    flee: CommandHelper.NewCommands("flee", "run"),
    save: CommandHelper.NewCommands("save"),
    load: CommandHelper.NewCommands("load"),
    all: CommandHelper.NewCommands("all"),
    ignoreItemTokens: CommandHelper.NewCommands("up", "to"),
    combineSeparators: CommandHelper.NewCommands("and", "+"),
    pourPrepositions: CommandHelper.NewCommands("into", "in"),
    directionAliases: new Dictionary<string, Direction>(StringComparer.OrdinalIgnoreCase)
    {
        ["n"] = Direction.North,
        ["s"] = Direction.South,
        ["e"] = Direction.East,
        ["w"] = Direction.West,
        ["ne"] = Direction.NorthEast,
        ["nw"] = Direction.NorthWest,
        ["se"] = Direction.SouthEast,
        ["sw"] = Direction.SouthWest,
        ["u"] = Direction.Up,
        ["d"] = Direction.Down,
        ["in"] = Direction.In,
        ["out"] = Direction.Out
    },
    allowDirectionEnumNames: true);

var parser = new KeywordParser(parserConfig);

var game = GameBuilder.Create()
    .UseState(state)
    .UseParser(parser)
    .UsePrompt("> ")
    .AddTurnStart(g =>
    {
        var lookResult = g.State.Look();
        g.Output.WriteLine($"\n{lookResult.Message}");
        var inventoryResult = g.State.InventoryView();
        g.Output.WriteLine(inventoryResult.Message);
    })
    .AddTurnEnd((g, command, result) =>
    {
        if (!g.State.WorldState.GetFlag("dragon_defeated") && !dragon.IsAlive)
        {
            g.State.WorldState.SetFlag("dragon_defeated", true);
            g.State.WorldState.AddTimeline("Dragon defeated.");
        }

        if (dragonHunt.CheckProgress(g.State))
        {
            g.Output.WriteLine($"\n*** QUEST COMPLETE: {dragonHunt.Title}! ***");
        }

        if (!loginQuestComplete && loginQuest.CheckProgress(g.State))
        {
            loginQuestComplete = true;
            g.Output.WriteLine($"\n*** QUEST COMPLETE: {loginQuest.Title}! ***");
        }

        if (command is LookCommand look && !string.IsNullOrWhiteSpace(look.Target))
        {
            var location = g.State.CurrentLocation;
            IGameEntity? entityHint = location.FindItem(look.Target)
                ?? g.State.Inventory.FindItem(look.Target);

            if (entityHint == null)
            {
                entityHint = location.Exits.Values
                    .Select(e => e.Door)
                    .FirstOrDefault(d => d != null && (look.Target.TextCompare("door") || d.Name.TextCompare(look.Target)));
            }

            if (entityHint == null)
            {
                entityHint = location.Exits.Values
                    .Select(e => e.Door?.RequiredKey)
                    .FirstOrDefault(k => k != null && k.Name.TextCompare(look.Target));
            }

            if (entityHint != null)
            {
                var hint = entityHint.GetHint();
                if (!string.IsNullOrWhiteSpace(hint))
                {
                    g.Output.WriteLine($"Hint: {hint}");
                }
            }
        }

        if (command is GoCommand go && result.Success)
        {
            if (g.State.IsCurrentRoomId("cabin"))
            {
                g.Output.WriteLine("\n*** CONGRATULATIONS! You found the treasure! ***");
                g.RequestStop();
            }
            else if (go.Direction == Direction.Down && g.State.CurrentLocation.Id == "entrance")
            {
                g.Output.WriteLine("Oops! You fell through a hole and ended up back at the entrance!");
            }
        }
    })
    .Build();

game.Run();

Console.WriteLine("\nThanks for playing!");
