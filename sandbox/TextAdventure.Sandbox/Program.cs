using MarcusMedina.TextAdventure.Commands;
using MarcusMedina.TextAdventure.Dsl;
using MarcusMedina.TextAdventure.Engine;
using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Extensions;
using MarcusMedina.TextAdventure.Interfaces;
using MarcusMedina.TextAdventure.Localization;
using MarcusMedina.TextAdventure.Models;
using MarcusMedina.TextAdventure.Parsing;
using MarcusMedina.TextAdventure.Tools;
using static MarcusMedina.TextAdventure.Extensions.ConsoleExtensions;

string dsl = """
world: Blackthorn Lighthouse
goal: Relight the beacon before the cutters run aground.
start: quay

location: quay | A wet stone quay with a fog horn, an iron chain, and a weather board nailed to a piling.
item: chain | iron chain | A salt-stiff iron chain bolted into the quay stones. | aliases=chain,iron chain | takeable=false
item: horn | fog horn | A brass fog horn on a cracked wooden stand. | aliases=horn,fog horn | takeable=false
item: board | weather board | A chalk board listing tide times and keeper notes. | aliases=board,weather board | takeable=false
item: chart | sea chart | A chart of local shoals and hidden reefs. | aliases=chart,map
key: brass_key | brass key | A tarnished brass key with a lighthouse crest. | aliases=key
exit: east -> keeper_house | door=keeper_door
exit: north -> signal_stairs
exit: west -> cliff_path

location: keeper_house | The keeper's house with a ledger desk, a paraffin kettle, and a framed rescue map.
item: ledger | signal ledger | A thick ledger recording lamp failures and ship sightings. | aliases=ledger,book | takeable=false
item: kettle | paraffin kettle | A dented kettle warming over a spirit flame. | aliases=kettle | takeable=false
item: rescue_map | rescue map | A map with lifeboat routes inked in red. | aliases=map,rescue map | takeable=false
exit: down -> cellar | door=cellar_hatch
exit: west -> quay | door=keeper_door

location: cellar | A cramped coal cellar with a split crate, a medicine tin, and a wall valve.
item: crate | split crate | A split crate still full of coal dust. | aliases=crate | takeable=false
item: valve | wall valve | A rusted wall valve controlling fuel feed. | aliases=valve,wheel | takeable=false
item: coal | lamp coal | A sack of lamp coal, dry enough to burn cleanly. | aliases=coal
item: tonic | storm tonic | A bitter tonic used to steady shaking hands. | aliases=tonic
exit: up -> keeper_house | door=cellar_hatch

location: signal_stairs | Narrow stairs with a handrail, a warning bell, and a cracked inspection mirror.
item: handrail | iron handrail | A cold handrail polished by decades of anxious hands. | aliases=rail,handrail | takeable=false
item: bell | warning bell | A warning bell with a frayed pull rope. | aliases=bell
item: mirror | inspection mirror | A mirror used to check lamp smoke from below. | aliases=mirror | takeable=false
exit: south -> quay
exit: up -> lantern_gallery
timed_door: up | opens_at=dusk | closes_at=day | message=The gallery gate clicks open for the evening watch. | closed_message=The gallery gate is bolted for daylight repairs.

location: lantern_gallery | The lantern gallery with a lens cradle, oil feed, and beacon controls.
item: lens_cradle | lens cradle | A brass cradle where the primary lens should sit. | aliases=cradle,lens cradle | takeable=false
item: oil_feed | oil feed | A copper oil feed with a pressure gauge. | aliases=feed,oil feed | takeable=false
item: controls | beacon controls | A set of old controls marked PRIME, IGNITE, and SHUT. | aliases=controls,panel | takeable=false
item: reserve_lens | reserve lens | A heavy reserve lens wrapped in canvas.
exit: down -> signal_stairs

location: cliff_path | A cliff path with marker posts, a mile bell, and a bent signal flag.
item: posts | marker posts | Marker posts painted white to catch moonlight. | aliases=posts,markers | takeable=false
item: mile_bell | mile bell | A bell used to report visibility by sound. | aliases=bell,mile bell | takeable=false
item: flag | signal flag | A torn signal flag snapping in the wind. | aliases=flag
exit: east -> quay

# Doors

door: keeper_door | keeper door | A thick oak door swollen by sea damp. | key=brass_key

door: cellar_hatch | cellar hatch | A bolted hatch with a stiff locking plate. | key=brass_key

# Timed world objects

timed_spawn: flare | appears_at=night | disappears_at=day | message=A red distress flare has washed against the quay wall.
""";

DslParser dslParser = new();
DslAdventure adventure = dslParser.ParseString(dsl);
GameState state = adventure.State;

var time = new TimeSystem()
    .Enable()
    .SetTicksPerDay(8)
    .SetStartTime(TimeOfDay.Dusk);
state.SetTimeSystem(time);

Location quay = adventure.Locations["quay"];
Location keeperHouse = adventure.Locations["keeper_house"];
Location cellar = adventure.Locations["cellar"];
Location signalStairs = adventure.Locations["signal_stairs"];
Location lanternGallery = adventure.Locations["lantern_gallery"];
Location cliffPath = adventure.Locations["cliff_path"];

Item chart = adventure.Items["chart"];
Item ledger = adventure.Items["ledger"];
Item tonic = adventure.Items["tonic"];
Item coal = adventure.Items["coal"];
Item reserveLens = adventure.Items["reserve_lens"];
Item bell = adventure.Items["bell"];

_ = chart.SetReadable().SetReadText("The chart marks Black Fang Reef as lethal in fog. The safest channel aligns with the lighthouse beacon.");
_ = ledger.SetReadable().SetReadText("Last entry: 'Lens cracked at dusk. If the beacon fails, ring the warning bell every ten minutes.'");
_ = tonic.AsDrink(6).WithPoison(1, 2);
_ = coal.AsStack(3, 0.4f).SetHint("Use this with the beacon controls once the lens is mounted.");
_ = reserveLens.SetWeight(1.8f).SetHint("Mount this at the lens cradle in the gallery.");
_ = bell.SetHint("Ring it to warn boats when visibility collapses.");

quay.SetDynamicDescription(new DynamicDescription()
    .When(s => s.WorldState.GetFlag("beacon_lit"),
        "The quay glitters under the restored beacon. The fog horn, chain, and weather board seem less ominous now.")
    .When(s => s.WorldState.GetFlag("storm_worsening"),
        "The quay is lashed by spray. The fog horn shudders, the iron chain rattles, and chalk has run across the weather board.")
    .Default("A wet stone quay with a fog horn, an iron chain, and a weather board nailed to a piling."));

lanternGallery.SetDynamicDescription(new DynamicDescription()
    .When(s => s.WorldState.GetFlag("lens_mounted") && !s.WorldState.GetFlag("beacon_lit"),
        "The reserve lens now sits in the cradle. The oil feed hisses, waiting for ignition.")
    .When(s => s.WorldState.GetFlag("beacon_lit"),
        "The gallery pulses with warm light. The lens throws a steady beam across black water.")
    .Default("The lantern gallery with a lens cradle, oil feed, and beacon controls."));

cliffPath.SetDynamicDescription(new DynamicDescription()
    .When(_ => cliffPath.FindItem("signal flag") is null,
        "A cliff path with marker posts and a mile bell above the surf.")
    .Default("A cliff path with marker posts, a mile bell, and a bent signal flag."));

INpc keeper = new Npc("keeper", "Keeper Sable")
    .Description("A weather-beaten keeper with oil-stained cuffs and a voice like gravel.")
    .SetMovement(new PatrolNpcMovement([keeperHouse, quay, keeperHouse]))
    .SetDialog(new DialogNode("If the lens is in place and the feed is primed, we might still save them.")
        .AddOption("What should I do first?", new DialogNode("Find the reserve lens in the gallery and secure fuel from the cellar."))
        .AddOption("How bad is the storm?", new DialogNode("Bad enough that the horn must be sounded if the beacon stays dark.")));
keeperHouse.AddNpc(keeper);

state.DramaticIrony.PlayerLearn("keeper_broke_primary_lens");
state.DramaticIrony.RegisterAction("keeper_broke_primary_lens", "ask_keeper");

IQuest lightQuest = new Quest("light_beacon", "Relight The Beacon", "Mount the reserve lens and ignite the beacon.")
    .AddCondition(new WorldFlagCondition("lens_mounted"))
    .AddCondition(new WorldFlagCondition("beacon_lit"));
_ = lightQuest.Start();
_ = state.Quests.Add(lightQuest);

IQuest warningQuest = new Quest("sound_warning", "Sound The Warning Bell", "Ring the warning bell twice if the weather turns.")
    .AddCondition(new WorldCounterCondition("bell_rings", 2));
_ = warningQuest.Start();
_ = state.Quests.Add(warningQuest);

KeywordParser parser = new(
    KeywordParserConfigBuilder.BritishDefaults()
        .WithFuzzyMatching(true, 1)
        .WithPhraseAlias("look at", "look")
        .Build());

SetupC64("Blackthorn Lighthouse");
WriteLineC64("=== BLACKTHORN LIGHTHOUSE ===");
WriteLineC64(adventure.WorldName ?? "Blackthorn Lighthouse");
WriteLineC64($"Goal: {adventure.Goal}");
WriteLineC64("Try: read ledger, take reserve lens, mount lens, light beacon, ring bell, map, quests.");
WriteLineC64();
state.ShowRoom();

INpc? activeConversationNpc = null;
IDialogNode? activeDialogNode = null;

while (true)
{
    foreach ((string sourceName, int damage) in state.TickPoisons())
        WriteLineC64($"{sourceName} burns in your throat. You take {damage} damage.");

    WriteLineC64();
    WritePromptC64(activeDialogNode is { Options.Count: > 0 } ? "choice> " : "> ");
    string? input = Console.ReadLine();
    if (input is null)
        break;

    string trimmed = input.Trim();
    if (string.IsNullOrWhiteSpace(trimmed))
        continue;

    if (TryHandleDialogChoice(trimmed, state, ref activeConversationNpc, ref activeDialogNode))
    {
        AdvanceWorldForCustomTurn(state, adventure);
        continue;
    }

    activeConversationNpc = null;
    activeDialogNode = null;

    if (trimmed.TextCompare("map"))
    {
        WriteLineC64(MarcusMedina.TextAdventure.Tools.MapGenerator.Render(state));
        continue;
    }

    if (trimmed.TextCompare("unlock cellar hatch") || trimmed.TextCompare("unlock hatch"))
    {
        if (!state.IsCurrentRoomId("keeper_house"))
        {
            WriteLineC64("You are not near the cellar hatch.");
            continue;
        }

        Exit? cellarExit = state.CurrentLocation.GetExit(Direction.Down);
        if (cellarExit?.Door is not Door cellarHatch)
        {
            WriteLineC64("There is no cellar hatch here.");
            continue;
        }

        IKey? brassKey = null;
        foreach (IItem item in state.Inventory.Items)
        {
            if (item is IKey key && key.Id.TextCompare("brass_key"))
            {
                brassKey = key;
                break;
            }
        }

        if (brassKey is null)
        {
            WriteLineC64("You need the brass key.");
            continue;
        }

        if (!cellarHatch.Unlock(brassKey))
        {
            WriteLineC64("That key doesn't fit.");
            continue;
        }

        _ = cellarHatch.Open();
        WriteLineC64("You unlock and open the cellar hatch.");
        AdvanceWorldForCustomTurn(state, adventure);
        continue;
    }

    if (trimmed.TextCompare("ring bell") || trimmed.TextCompare("sound bell") || trimmed.TextCompare("sound warning"))
    {
        if (!state.IsCurrentRoomId("signal_stairs") && !state.IsCurrentRoomId("cliff_path"))
        {
            WriteLineC64("You cannot reach a warning bell from here.");
            continue;
        }

        int count = state.WorldState.Increment("bell_rings");
        state.WorldState.AddTimeline($"Warning bell sounded ({count}).");
        WriteLineC64(count < 2
            ? "The bell tolls once across the surf."
            : "The bell tolls again, carrying out over the black water.");

        AdvanceWorldForCustomTurn(state, adventure);
        if (state.Quests.CheckAll(state))
            WriteLineC64("Quest updated.");

        if (TryShowEnding(state))
            break;

        continue;
    }

    if (trimmed.TextCompare("ask keeper"))
    {
        if (!state.IsCurrentRoomId("keeper_house") && !state.IsCurrentRoomId("quay"))
        {
            WriteLineC64("Keeper Sable is not within earshot.");
            continue;
        }

        if (!state.DramaticIrony.GetAvailableActions().Contains("ask_keeper"))
        {
            WriteLineC64("You have nothing pressing to challenge him about.");
            continue;
        }

        WriteLineC64("You ask about the shattered lens. He exhales slowly and nods.");
        WriteLineC64("'A careless turn of the winch. My fault. Fix the beacon and we will settle blame later.'");
        state.DramaticIrony.NpcLearn(keeper, "keeper_broke_primary_lens");
        state.WorldState.SetRelationship("keeper", state.WorldState.GetRelationship("keeper") + 5);

        AdvanceWorldForCustomTurn(state, adventure);
        continue;
    }

    if (trimmed.TextCompare("mount lens") || trimmed.TextCompare("fit lens"))
    {
        if (!state.IsCurrentRoomId("lantern_gallery"))
        {
            WriteLineC64("You need to be in the lantern gallery to mount the lens.");
            continue;
        }

        IItem? carriedLens = state.Inventory.FindById("reserve_lens");
        if (carriedLens is null)
        {
            WriteLineC64("You are not carrying the reserve lens.");
            continue;
        }

        _ = state.Inventory.Remove(carriedLens);
        state.WorldState.SetFlag("lens_mounted", true);
        WriteLineC64("You lock the reserve lens into the brass cradle.");

        AdvanceWorldForCustomTurn(state, adventure);
        if (state.Quests.CheckAll(state))
            WriteLineC64("Quest updated.");

        continue;
    }

    if (trimmed.TextCompare("light beacon") || trimmed.TextCompare("ignite beacon"))
    {
        if (!state.IsCurrentRoomId("lantern_gallery"))
        {
            WriteLineC64("You can only ignite the beacon from the lantern gallery.");
            continue;
        }

        if (!state.WorldState.GetFlag("lens_mounted"))
        {
            WriteLineC64("The lens cradle is empty; igniting now would waste fuel and blind the mechanism.");
            continue;
        }

        if (state.Inventory.FindById("coal") is null)
        {
            WriteLineC64("You need lamp coal before the beacon can hold a steady flame.");
            continue;
        }

        state.WorldState.SetFlag("beacon_lit", true);
        state.WorldState.AddTimeline("Beacon reignited.");
        WriteLineC64("You prime the feed and strike ignition. The beacon roars to life.");

        AdvanceWorldForCustomTurn(state, adventure);
        if (state.Quests.CheckAll(state))
            WriteLineC64("Quest updated.");

        if (TryShowEnding(state))
            break;

        continue;
    }

    ICommand command = parser.Parse(trimmed);
    INpc? talkNpc = command is TalkCommand pendingTalk && !string.IsNullOrWhiteSpace(pendingTalk.Target)
        ? state.CurrentLocation.FindNpc(pendingTalk.Target)
        : null;
    CommandResult result = state.Execute(command);

    if (command is TalkCommand && result.Success)
    {
        INpc? npc = talkNpc;
        if (npc is null && result.Message.Contains(Language.DialogOptionsLabel, StringComparison.OrdinalIgnoreCase))
            npc = state.CurrentLocation.Npcs.FirstOrDefault(candidate => candidate.DialogRoot is { Options.Count: > 0 });

        if (npc?.DialogRoot is IDialogNode { Options.Count: > 0 } dialog)
        {
            activeConversationNpc = npc;
            activeDialogNode = dialog;
        }
    }

    if (command is ReadCommand && result.Success && state.IsCurrentRoomId("keeper_house"))
        state.WorldState.SetFlag("read_ledger", true);

    if (command is GoCommand && result.Success)
    {
        int crossings = state.WorldState.Increment("crossings");
        if (crossings >= 4 && !state.WorldState.GetFlag("storm_worsening"))
            state.WorldState.SetFlag("storm_worsening", true);
    }

    ProcessTimedWorld(adventure, state);

    if (state.Quests.CheckAll(state))
        result = result.WithSuggestion("Quest updated.");

    state.DisplayResult(command, result);

    if (result.ShouldQuit)
        break;

    if (TryShowEnding(state))
        break;
}

static bool TryHandleDialogChoice(string input, GameState state, ref INpc? activeConversationNpc, ref IDialogNode? activeDialogNode)
{
    if (activeConversationNpc is null || activeDialogNode is null || activeDialogNode.Options.Count == 0)
        return false;

    string trimmed = input.Trim();
    DialogOption? selectedOption = null;

    if (int.TryParse(trimmed.TrimEnd('.'), out int index))
    {
        if (index < 1 || index > activeDialogNode.Options.Count)
        {
            WriteLineC64("Choose a listed option number.");
            return true;
        }

        selectedOption = activeDialogNode.Options[index - 1];
    }
    else
    {
        selectedOption = activeDialogNode.Options.FirstOrDefault(option => option.Text.TextCompare(trimmed));
        if (selectedOption is null)
            return false;
    }

    if (selectedOption.Next is not IDialogNode nextNode)
    {
        WriteLineC64($"{activeConversationNpc.Name} has nothing more to add.");
        activeConversationNpc = null;
        activeDialogNode = null;
        return true;
    }

    WriteDialogNode(nextNode);

    if (nextNode.Options.Count == 0)
    {
        activeConversationNpc = null;
        activeDialogNode = null;
    }
    else
    {
        activeDialogNode = nextNode;
    }

    return true;
}

static void WriteDialogNode(IDialogNode node)
{
    WriteLineC64(node.Text);
    if (node.Options.Count == 0)
        return;

    WriteLineC64(Language.DialogOptionsLabel);
    for (var i = 0; i < node.Options.Count; i++)
        WriteLineC64(Language.DialogOption(i + 1, node.Options[i].Text));
}

static bool TryShowEnding(GameState state)
{
    if (!state.WorldState.GetFlag("beacon_lit") || state.WorldState.GetCounter("bell_rings") < 2)
        return false;

    WriteLineC64();
    WriteLineC64("A distant cutter answers with two short horn blasts. The channel is safe for tonight.");
    WriteLineC64("=== END: THE BEACON HOLDS ===");
    return true;
}

static void AdvanceWorldForCustomTurn(GameState state, DslAdventure adventure)
{
    state.TimeSystem.Tick(state);
    state.RandomEvents.Tick(state);
    state.TickNpcTriggers();
    state.Tension.Tick(state);
    ProcessTimedWorld(adventure, state);
}

static void ProcessTimedWorld(DslAdventure adventure, GameState state)
{
    int tick = state.TimeSystem.CurrentTick;
    TimePhase phase = state.TimeSystem.CurrentPhase;

    foreach (Location location in adventure.Locations.Values)
    {
        foreach (TimedSpawn spawn in location.TimedSpawns)
        {
            bool shouldAppear = spawn.AppearTicks.Contains(tick)
                || spawn.AppearPhases.Contains(phase)
                || spawn.AppearConditions.Any(condition => condition(state));

            if (shouldAppear && location.FindItem(spawn.ItemId) == null && adventure.Items.TryGetValue(spawn.ItemId, out Item? template))
            {
                location.AddItem(template.Clone());
                if (!string.IsNullOrWhiteSpace(spawn.MessageText))
                    Console.WriteLine(spawn.MessageText);
            }

            bool shouldDisappear = spawn.DisappearPhases.Contains(phase);
            if (shouldDisappear)
            {
                IItem? spawnedItem = location.Items.FirstOrDefault(item => item.Id.TextCompare(spawn.ItemId));
                if (spawnedItem is not null)
                    _ = location.RemoveItem(spawnedItem);
            }
        }

        foreach (Exit exit in location.Exits.Values)
        {
            if (exit.Door is not Door door || exit.TimedDoor is not TimedDoor timedDoor)
                continue;

            bool shouldOpen = timedDoor.OpenTicks.Contains(tick)
                || timedDoor.OpenPhases.Contains(phase)
                || timedDoor.OpenConditions.Any(condition => condition(state))
                || (timedDoor.PermanentOpenCondition?.Invoke(state) ?? false);

            if (shouldOpen && door.State != DoorState.Open)
            {
                _ = door.Open();
                if (!string.IsNullOrWhiteSpace(timedDoor.MessageText))
                    Console.WriteLine(timedDoor.MessageText);
            }

            bool shouldClose = timedDoor.CloseTicks.Contains(tick)
                || timedDoor.ClosePhases.Contains(phase)
                || timedDoor.CloseConditions.Any(condition => condition(state));

            if (shouldClose && door.State == DoorState.Open)
            {
                _ = door.Close();
                if (!string.IsNullOrWhiteSpace(timedDoor.ClosedMessageText))
                    Console.WriteLine(timedDoor.ClosedMessageText);
            }
        }
    }
}
