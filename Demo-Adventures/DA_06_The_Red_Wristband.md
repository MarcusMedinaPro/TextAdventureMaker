# Demo Adventure 06: The Red Wristband

Hospital horror demo with many red herrings and a fixed scream ending.

## Story Goal

You are the on-call doctor, sent to fetch supplies from the basement.
A patient rides the lift with you.
Something limps towards the lift in the basement.
You survive one decision, then discover you were never safe.

## Drop-In Sandbox Program (C#)

```csharp
using MarcusMedina.TextAdventure.Commands;
using MarcusMedina.TextAdventure.Engine;
using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Extensions;
using MarcusMedina.TextAdventure.Models;
using MarcusMedina.TextAdventure.Parsing;
using static MarcusMedina.TextAdventure.Extensions.ConsoleExtensions;

StoryStage stage = StoryStage.Briefing;
bool basementSeen;
bool suppliesReadyAnnounced;

Location wardCorridor = new("ward_corridor", "A sterile night corridor. Fluorescent lights buzz over polished floors.");
Location liftCar = new("lift_car", "A mirrored hospital lift that smells faintly of antiseptic and metal.");
Location basementLanding = new("basement_landing", "A dim basement landing with flickering strips of light and long concrete shadows.");
Location supplyStore = new("supply_store", "Tall metal shelves are stacked with wrapped packs and labelled boxes.");

wardCorridor.AddExit(Direction.Down, liftCar);
liftCar.AddExit(Direction.Up, wardCorridor);
liftCar.AddExit(Direction.Down, basementLanding);
basementLanding.AddExit(Direction.Up, liftCar);
basementLanding.AddExit(Direction.North, supplyStore);
supplyStore.AddExit(Direction.South, basementLanding);

// Red herrings: corridor clutter
wardCorridor.AddItem(new Item("clipboard", "Clipboard", "A plastic clipboard full of unsigned forms.").SetReadable().SetReadText("Consent forms, bed transfers, and handwriting no one can decipher."));
wardCorridor.AddItem(new Item("poster", "Safety Poster", "A laminated poster about hand hygiene and proper glove disposal.").SetTakeable(false).SetReadable().SetReadText("WASH. DRY. SANITISE. REPEAT."));
wardCorridor.AddItem(new Item("sanitiser", "Sanitiser Pump", "A wall-mounted sanitiser pump with a sticky nozzle.").SetTakeable(false));
wardCorridor.AddItem(new Item("vending", "Vending Machine", "Chocolate bars, isotonic drinks, and crisps no one admits to buying.").SetTakeable(false));
wardCorridor.AddItem(new Item("chair", "Plastic Chair", "A waiting chair with one short leg.").SetTakeable(false));
wardCorridor.AddItem(new Item("clock", "Wall Clock", "A ticking wall clock frozen two minutes slow.").SetTakeable(false));
wardCorridor.AddItem(new Item("umbrella", "Umbrella Stand", "Three damp umbrellas and one bent walking stick.").SetTakeable(false));
wardCorridor.AddItem(new Item("noticeboard", "Noticeboard", "Shift swaps, cafeteria offers, and half a missing crossword.").SetTakeable(false).SetReadable().SetReadText("Night shift short-staffed. Bring your own tea."));

// Red herrings: lift details
liftCar.AddItem(new Item("panel", "Button Panel", "Rows of numbered buttons, one cracked and taped over.").SetTakeable(false));
liftCar.AddItem(new Item("mirror", "Lift Mirror", "Your face looks older under hospital light.").SetTakeable(false));
liftCar.AddItem(new Item("camera", "Ceiling Camera", "A tiny red LED blinks without comfort.").SetTakeable(false));
liftCar.AddItem(new Item("mat", "Lift Mat", "A rubber mat marked with muddy heel prints.").SetTakeable(false));

// Red herrings: basement and store
basementLanding.AddItem(new Item("mop", "Mop", "A wet mop leaning against a caution sign."));
basementLanding.AddItem(new Item("sign", "Caution Sign", "CAUTION: WET FLOOR. The plastic is cracked at the base.").SetTakeable(false).SetReadable().SetReadText("Someone has scribbled 'still slippery' in pen."));
basementLanding.AddItem(new Item("vent", "Vent Grille", "A rusted vent grille humming with distant machinery.").SetTakeable(false));
basementLanding.AddItem(new Item("trolley", "Service Trolley", "An empty steel trolley with one squeaking wheel.").SetTakeable(false));
basementLanding.AddItem(new Item("intercom", "Wall Intercom", "A beige intercom handset with faded labels.").SetTakeable(false));

supplyStore.AddItem(new Item("gauze", "Gauze Pack", "Sterile gauze packs in sealed blue wrappers."));
supplyStore.AddItem(new Item("saline", "Saline Bag", "Clear fluid bags hanging from a wire rack."));
supplyStore.AddItem(new Item("mask_box", "Mask Box", "Disposable masks in a cardboard dispenser."));
supplyStore.AddItem(new Item("locker", "Storage Locker", "A tall metal locker with chipped paint and a stubborn latch.").SetTakeable(false));
supplyStore.AddItem(new Item("manifest", "Delivery Manifest", "A clipboard listing deliveries to theatre and ICU.").SetReadable().SetReadText("Sutures, dressings, tubing, syringes. Everything except peace of mind."));
supplyStore.AddItem(new Item("sharps_box", "Sharps Box", "A yellow sharps box bolted to the wall.").SetTakeable(false));
supplyStore.AddItem(new Item("blanket", "Thermal Blanket", "A folded silver thermal blanket that crackles like foil."));
supplyStore.AddItem(new Item("torch", "Pocket Torch", "A cheap pocket torch with a weak battery."));

Npc woman = new("woman", "Patient")
    .Description("A calm woman in a hospital gown, standing with her hands folded.")
    .Dialog("Quiet night, doctor?");

GameState state = new(wardCorridor, worldLocations: [wardCorridor, liftCar, basementLanding, supplyStore]);
KeywordParser parser = new(KeywordParserConfigBuilder.BritishDefaults().Build());

SetupC64("The Red Wristband");
WriteLineC64("=== THE RED WRISTBAND ===");
WriteLineC64();
WriteLineC64("You are the on-call doctor. A nurse asks you to fetch supplies from the basement.");
WriteLineC64("Patients wear coloured wristbands here: green for alive, red for deceased.");
WriteLineC64();
WriteLineC64("Goal: take the lift down, collect supplies, and return upstairs.");
WriteLineC64("Try: look, go, take, read, open locker, examine objects.");

state.ShowRoom();

while (stage != StoryStage.Ended)
{
    WriteLineC64();
    WritePromptC64("> ");
    string? input = Console.ReadLine();
    if (input is null)
        break;

    string text = input.Trim();
    if (string.IsNullOrWhiteSpace(text))
        continue;

    if (HandleSpecial(text))
        continue;

    ICommand command = parser.Parse(text);
    CommandResult result = state.Execute(command);
    state.DisplayResult(command, result);

    if (command is TakeCommand && result.Success && !suppliesReadyAnnounced && HasRequiredSupplies())
    {
        suppliesReadyAnnounced = true;
        WriteLineC64("You have enough supplies. Time to get back upstairs.");
    }

    if (result.ShouldQuit)
        break;

    if (command is GoCommand go && result.Success)
        HandleMoveBeat(go.Direction);
}

return;

bool HandleSpecial(string text)
{
    if ((text.TextCompare("go up") || text.TextCompare("up") || text.TextCompare("go upstairs"))
        && stage == StoryStage.BasementReached
        && state.IsCurrentRoomId("basement_landing")
        && !HasRequiredSupplies())
    {
        WriteLineC64("You were sent for supplies. You should grab at least gauze and a saline bag first.");
        return true;
    }

    if (text.TextCompare("open vending machine"))
    {
        WriteLineC64("You rattle the flap. It stays shut and judges you quietly.");
        return true;
    }

    if (text.TextCompare("open locker"))
    {
        if (!state.IsCurrentRoomId("supply_store"))
        {
            WriteLineC64("There is no locker to open here.");
            return true;
        }

        WriteLineC64("You force the locker open. Aprons, mop heads, and an old radio. Nothing useful.");
        return true;
    }

    if (text.TextCompare("open vent") || text.TextCompare("open vent grille"))
    {
        WriteLineC64("You tug at the grille. Rust flakes down, but it does not budge.");
        return true;
    }

    if (text.TextCompare("call porter") || text.TextCompare("use intercom") || text.TextCompare("call security"))
    {
        if (!state.IsCurrentRoomId("basement_landing"))
        {
            WriteLineC64("You mutter for help to no one in particular.");
            return true;
        }

        WriteLineC64("You lift the intercom. Only static answers back.");
        return true;
    }

    if (text.TextCompare("check wristband") || text.TextCompare("look wristband"))
    {
        if (!state.IsCurrentRoomId("lift_car"))
        {
            WriteLineC64("No patient stands close enough here.");
            return true;
        }

        WriteLineC64("Her wrist is turned away. You tell yourself you imagined the unease.");
        return true;
    }

    if (text.TextCompare("press basement") || text.TextCompare("press b") || text.TextCompare("press down"))
    {
        if (!state.IsCurrentRoomId("lift_car"))
        {
            WriteLineC64("You press nothing in the air. The lift panel is not here.");
            return true;
        }

        WriteLineC64("You press for the basement.");
        bool moved = state.Move(Direction.Down);
        if (moved)
        {
            state.ShowRoom();
            HandleMoveBeat(Direction.Down);
        }

        return true;
    }

    return false;
}

bool HasRequiredSupplies() =>
    state.Inventory.FindItem("gauze") is not null
    && state.Inventory.FindItem("saline") is not null;

void HandleMoveBeat(Direction direction)
{
    if (stage == StoryStage.Briefing && state.IsCurrentRoomId("lift_car"))
    {
        stage = StoryStage.InLift;
        liftCar.AddNpc(woman);

        WriteLineC64("The lift doors open. A patient is already inside, calm and silent.");
        WriteLineC64("You smile politely and step in.");
        return;
    }

    if (stage == StoryStage.InLift && state.IsCurrentRoomId("basement_landing"))
    {
        stage = StoryStage.BasementReached;
        basementSeen = true;

        WriteLineC64("The doors part to the basement.");
        WriteLineC64("In the distance, a man limps towards the lift, dragging one foot.");
        WriteLineC64("His shape is wrong in a way you feel before you understand.");
        WriteLineC64("Maybe collect supplies, then return upstairs quickly.");
        return;
    }

    if (stage == StoryStage.BasementReached
        && basementSeen
        && state.IsCurrentRoomId("lift_car")
        && direction == Direction.Up)
    {
        TriggerFinalReveal();
    }
}

void TriggerFinalReveal()
{
    stage = StoryStage.Reveal;

    WriteLineC64("You hammer the close button and ride back up, pulse pounding.");
    WriteLineC64("The patient turns to you, annoyed.");
    WriteLineC64("'Why did you do that? He was trying to use the lift.'");
    WriteLineC64();
    WriteLineC64("'Did you see his wrist?' you ask. 'It was red. He died last night. I performed his surgery.'");
    WriteLineC64();
    WriteLineC64("She lifts her wrist slowly.");
    WriteLineC64("A red band catches the fluorescent light.");
    WriteLineC64("She smiles. 'Like this one?' ");
    WriteLineC64();
    WriteLineC64("A scream tears out of your throat before the doors open.");
    WriteLineC64();
    WriteLineC64("=== GAME OVER ===");

    stage = StoryStage.Ended;
}

enum StoryStage
{
    Briefing,
    InLift,
    BasementReached,
    Reveal,
    Ended
}
```
