# 👁️ Spatial Awareness Revolution - TAF's Silent Hill Moment

## Silent Hill: Shattered Memories - The Breakthrough

### What Made It Revolutionary:
- **Lean around corners** → peek without commitment
- **Look behind while running** → paranoia and tension mechanics
- **Environmental awareness** → space became a character
- **Immersive positioning** → where you are matters as much as what you do

### The Core Innovation:
**Space stopped being just backdrop and became an interactive, tactical element.**

---

## 🎮 TAF's Text Adventure Revolution - Same Principle

### Traditional Text Adventures:
```
> go north
You are in a kitchen.
> look
A kitchen with pots and pans.
```
**Problem**: Static, disconnected room descriptions. No spatial relationship awareness.

### TAF's Spatial Revolution:
```
> look north
Through the doorway you see: A shadowy figure moving in the living room.

> lean north
Carefully peering around the doorframe, you spot:
The intruder is rifling through drawers, back turned to you.

> listen south
Behind you: Footsteps echo from the basement stairs - someone's coming up!

> throw keys east
Keys clatter in the dining room, drawing the intruder's attention away.

> whisper west
You softly call to your hiding partner: "Now! Run!"
```

**Revolution**: **Every direction becomes a sensory and tactical interface.**

---

## 🕵️‍♂️ Advanced Spatial Awareness System

### Core Spatial Commands:
```csharp
public enum SpatialAction
{
    // Silent Hill-inspired awareness
    Lean,           // "lean north" - peek around corner
    LookBehind,     // "look behind" - check your back
    Listen,         // "listen south" - audio awareness
    Smell,          // "smell east" - scent detection
    Feel,           // "feel west" - tactile sensing

    // Advanced positioning
    Hide,           // "hide behind north wall"
    Crouch,         // "crouch near east door"
    Position,       // "position near south exit"

    // Multi-directional awareness
    Survey,         // "survey area" - 360° awareness
    Watch,          // "watch north" - continuous monitoring
    Guard,          // "guard south" - defensive stance
}
```

### Enhanced Spatial Parser:
```csharp
public class SpatialAwarenessParser
{
    public ParsedCommand ParseSpatialCommand(string input)
    {
        var spatialPatterns = new Dictionary<Regex, Func<Match, ParsedCommand>>
        {
            // Silent Hill style positioning
            [new Regex(@"lean (north|south|east|west)")] = match =>
                new ParsedCommand("lean", direction: match.Groups[1].Value),

            [new Regex(@"look behind")] = match =>
                new ParsedCommand("look_behind"),

            [new Regex(@"(listen|hear) (north|south|east|west|around|behind)")] = match =>
                new ParsedCommand("listen", direction: match.Groups[2].Value),

            // Advanced positioning
            [new Regex(@"hide (behind|near) (north|south|east|west) (wall|door|corner)")] = match =>
                new ParsedCommand("hide", position: $"{match.Groups[1].Value} {match.Groups[2].Value} {match.Groups[3].Value}"),

            [new Regex(@"crouch (near|by) (north|south|east|west) (door|wall|exit)")] = match =>
                new ParsedCommand("crouch", position: $"{match.Groups[1].Value} {match.Groups[2].Value} {match.Groups[3].Value}"),

            // Continuous awareness
            [new Regex(@"watch (north|south|east|west)")] = match =>
                new ParsedCommand("watch", direction: match.Groups[1].Value),

            [new Regex(@"survey (area|room|surroundings)")] = match =>
                new ParsedCommand("survey"),
        };

        foreach (var (pattern, parser) in spatialPatterns)
        {
            var match = pattern.Match(input.ToLower());
            if (match.Success)
                return parser(match);
        }

        return null;
    }
}
```

---

## 🎭 Immersive Horror & Tension Mechanics

### Paranoia System (Silent Hill inspired):
```csharp
public class ParanoiaSystem
{
    public float PlayerAnxiety { get; set; } = 0.0f;
    public Dictionary<Direction, ThreatLevel> DirectionalThreats { get; set; } = new();

    public string ProcessLookBehind(Player player, GameContext context)
    {
        PlayerAnxiety += 0.1f; // Looking behind increases paranoia

        var behindDirection = player.LastMovement.GetOpposite();
        var threat = DirectionalThreats.GetValueOrDefault(behindDirection, ThreatLevel.None);

        switch (threat)
        {
            case ThreatLevel.None:
                return PlayerAnxiety > 0.7f
                    ? "You glance nervously behind you. Nothing... or is there?"
                    : "The path behind you is clear.";

            case ThreatLevel.Subtle:
                return "Was that a shadow moving? You can't be sure...";

            case ThreatLevel.Obvious:
                return "Something is definitely following you. Your heart pounds.";

            case ThreatLevel.Immediate:
                PlayerAnxiety = 1.0f;
                return "A dark figure lurks just behind you! RUN!";
        }
    }

    public string ProcessLean(Direction direction, Room currentRoom, Player player)
    {
        var pathway = currentRoom.GetPathway(direction);
        var targetRoom = pathway?.GetTargetRoom(currentRoom);

        if (targetRoom == null)
            return $"You lean toward {direction.ToSwedish()}, but there's just a wall.";

        // Stealth detection
        var stealthRoll = CalculateStealthSuccess(player);
        var awareness = GetRoomAwareness(targetRoom, stealthRoll);

        return $"Carefully leaning around the {pathway.GetDescription()}, you see:\n{awareness}";
    }
}
```

### Atmospheric Spatial Descriptions:
```csharp
public class AtmosphericSpatialSystem
{
    public string GenerateLeanDescription(Room targetRoom, float visibilityFactor, bool stealthSuccess)
    {
        var description = new StringBuilder();

        if (!stealthSuccess)
        {
            description.AppendLine("You lean out too far! Anyone in there would spot you.");
        }

        // Base room glimpse
        description.AppendLine(GetStealthyRoomDescription(targetRoom, visibilityFactor));

        // NPCs and their states
        var npcs = targetRoom.GetNPCs();
        foreach (var npc in npcs)
        {
            var npcDescription = GetNPCPostureDescription(npc);
            description.AppendLine(npcDescription);
        }

        // Atmospheric details
        var atmosphere = GetAtmosphericDetails(targetRoom);
        if (!string.IsNullOrEmpty(atmosphere))
            description.AppendLine(atmosphere);

        return description.ToString().Trim();
    }

    private string GetNPCPostureDescription(NPC npc)
    {
        return npc.CurrentState switch
        {
            NPCState.Searching => $"{npc.Name} moves methodically through the room, searching.",
            NPCState.Patrolling => $"{npc.Name} walks a regular pattern, not yet alerted.",
            NPCState.Alerted => $"{npc.Name} stands tense, listening for sounds.",
            NPCState.Hostile => $"{npc.Name} prowls angrily, weapon ready.",
            NPCState.Sleeping => $"{npc.Name} sits quietly, possibly asleep.",
            _ => $"{npc.Name} is present in the room."
        };
    }
}
```

---

## 🎯 Advanced Tactical Positioning

### Position-Based Combat System:
```csharp
public class PositionalCombat
{
    public enum PlayerPosition
    {
        Center,         // Normal position - vulnerable from all sides
        NearNorthWall,  // Protected from north, better south visibility
        BehindEastDoor, // Hidden from east, surprise advantage
        CrouchedLow,    // Harder to hit, reduced visibility
        ElevatedHigh,   // Better overview, harder to reach
    }

    public CombatModifiers GetPositionModifiers(PlayerPosition position, Direction attackDirection)
    {
        var modifiers = new CombatModifiers();

        switch (position)
        {
            case PlayerPosition.NearNorthWall:
                if (attackDirection == Direction.North)
                    modifiers.DefenseBonus += 0.3f; // Wall protection
                if (attackDirection == Direction.South)
                    modifiers.DetectionBonus += 0.2f; // Better visibility
                break;

            case PlayerPosition.BehindEastDoor:
                if (attackDirection == Direction.East)
                    modifiers.SurpriseAttackBonus += 0.5f; // Ambush potential
                modifiers.HiddenFromDirection = Direction.East;
                break;

            case PlayerPosition.CrouchedLow:
                modifiers.DefenseBonus += 0.2f; // Harder target
                modifiers.VisibilityReduction += 0.3f; // Can't see as well
                break;
        }

        return modifiers;
    }
}
```

### Environmental Positioning:
```csharp
public class EnvironmentalPosition
{
    public string Description { get; set; }
    public Dictionary<Direction, float> VisibilityModifiers { get; set; } = new();
    public Dictionary<Direction, float> DefenseModifiers { get; set; } = new();
    public List<string> AvailableActions { get; set; } = new();

    public static EnvironmentalPosition BehindNorthWall => new()
    {
        Description = "crouched behind the northern wall",
        VisibilityModifiers = new()
        {
            [Direction.North] = 0.1f,  // Can barely see north
            [Direction.South] = 1.2f,  // Great view south
            [Direction.East] = 0.8f,   // Decent side visibility
            [Direction.West] = 0.8f
        },
        DefenseModifiers = new()
        {
            [Direction.North] = 0.8f,  // Wall protects from north attacks
            [Direction.South] = -0.2f  // Exposed to south attacks
        },
        AvailableActions = { "lean_out", "peek_around_corner", "throw_from_cover" }
    };
}
```

---

## 🔊 Advanced Audio Spatial System

### Directional Audio Processing:
```csharp
public class DirectionalAudioSystem
{
    public AudioClue ProcessListenCommand(Direction direction, Room currentRoom, Player player)
    {
        var pathway = currentRoom.GetPathway(direction);
        var targetRoom = pathway?.GetTargetRoom(currentRoom);

        if (targetRoom == null)
            return new AudioClue("You press your ear to the wall. Silence.");

        var sounds = CollectSoundsFromRoom(targetRoom);
        var filteredSounds = FilterSoundsThroughPathway(sounds, pathway);
        var playerModifiers = GetPlayerHearingModifiers(player);

        return GenerateAudioDescription(filteredSounds, playerModifiers, direction);
    }

    private List<Sound> CollectSoundsFromRoom(Room room)
    {
        var sounds = new List<Sound>();

        // NPC-generated sounds
        foreach (var npc in room.GetNPCs())
        {
            sounds.AddRange(npc.GetCurrentSounds());
        }

        // Environmental sounds
        sounds.AddRange(room.GetAmbientSounds());

        // Object-based sounds
        foreach (var obj in room.GetObjects())
        {
            if (obj.IsActiveMachine())
                sounds.AddRange(obj.GetMachineSounds());
        }

        return sounds;
    }

    private string GenerateAudioDescription(List<Sound> sounds, PlayerHearingModifiers modifiers, Direction direction)
    {
        if (!sounds.Any())
            return $"You listen carefully {direction.ToSwedish()}. Complete silence.";

        var description = new StringBuilder();
        description.AppendLine($"Listening {direction.ToSwedish()}, you hear:");

        foreach (var sound in sounds.OrderByDescending(s => s.Volume))
        {
            var modifiedVolume = sound.Volume * modifiers.VolumeMultiplier;
            var clarity = CalculateClarity(modifiedVolume, sound.Type);

            var soundDesc = GenerateSoundDescription(sound, clarity);
            description.AppendLine($"  {soundDesc}");
        }

        return description.ToString().Trim();
    }
}
```

---

## 🎮 Practical Horror Scenarios

### 1. **Home Invasion** (Silent Hill inspired):
```
You're in the kitchen when you hear the front door creak...

> listen north
Heavy footsteps in the hallway. Someone's moving toward the living room.

> lean north
Through the doorway you see: A dark figure examining your family photos.

> look behind
The back door is still locked. No escape route there.

> crouch behind east counter
You duck down behind the kitchen island, heart pounding.

> watch north
You maintain sight on the doorway, ready to move if spotted.

> throw pan west
The pan clatters in the dining room, drawing the intruder away.

> whisper south
You softly call through the basement door: "Kids, stay downstairs and lock the door!"
```

### 2. **Prison Escape**:
```
Your cell door is unlocked, but guards patrol the corridor...

> lean east
A guard walks past, flashlight sweeping. He hasn't seen you yet.

> listen west
Footsteps fading... he's moving toward the far end.

> position near south wall
You press against the wall beside the door frame.

> look behind
The corridor behind you is clear - for now.

> survey area
North: Guard station (occupied)
East: Your escape route (currently clear)
South: Dead end
West: Guard approaching with keys

> throw shoe north
The shoe hits the far wall. "What was that?" The guard investigates.

> crouch low east
You slip out low and quiet while the guard is distracted.
```

### 3. **Haunted House Investigation**:
```
The old mansion creaks around you...

> listen up
Something is dragging across the floor above you.

> lean west
Through the parlor doorway: Shadows move independently of any light source.

> look behind
The front door you entered through is now... closed. You didn't close it.

> smell north
The scent of decay drifts from the dining room.

> survey area
A chill runs down your spine as you assess your surroundings:
North: Dining room (foul smell, possible danger)
East: Library (dark, books scattered)
South: Front door (mysteriously closed)
West: Parlor (supernatural activity)
Up: Second floor (dragging sounds continue)

> position center
You stay in the middle of the room, keeping all exits in view.

> watch up
You keep your eyes on the ceiling, tracking the movements above.
```

---

## 🚀 Revolutionary Impact

### What This Brings to Text Adventures:

#### **Spatial Immersion**:
✅ **Every direction becomes interactive** - not just movement destinations
✅ **Positioning matters** - where you stand affects what you can do
✅ **Environmental awareness** - constant spatial relationship feedback
✅ **Tension through geography** - space creates suspense

#### **Tactical Depth**:
✅ **Cover and concealment** - hide behind walls, doors, furniture
✅ **Ambush mechanics** - position for surprise attacks
✅ **Escape planning** - assess all exits before committing
✅ **Resource management** - attention and awareness as limited resources

#### **Narrative Innovation**:
✅ **Show don't tell** - reveal story through spatial observation
✅ **Paranoia mechanics** - looking behind increases tension
✅ **Environmental storytelling** - each direction tells part of the story
✅ **Player agency** - multiple ways to gather information and react

### **TAF's Silent Hill Moment**:
Just like Shattered Memories made **physical positioning revolutionary in horror games**, TAF makes **directional awareness revolutionary in text adventures**.

**Before TAF**: Rooms are containers you enter and leave.
**After TAF**: Rooms are tactical environments you inhabit and navigate with full spatial awareness.

---

## 🎮 Epic Scenario Collection - Complete Gameplay Examples

### 🏠 Home Invasion - Complete Sequence
```
=== SETUP ===
You're washing dishes in the kitchen when you hear the front door creak open.
No one else should be home...

=== PLAYER ACTIONS & RESPONSES ===

> listen north
You freeze, dish in hand. Heavy footsteps echo from the hallway,
moving slowly toward the living room.

> lean north
Carefully peering around the doorframe, you see:
A dark figure in a hoodie examining your family photos on the mantle.
Their back is turned to you.

> look behind
The back door behind you is still locked. Your escape route is blocked
by the kitchen island. No way out without crossing their line of sight.

> crouch behind east counter
You duck down behind the kitchen island, heart hammering in your chest.
The marble countertop hides you from view, but you can still hear them moving.

> listen north
The footsteps have stopped. Silence. Are they listening for you too?

> watch north
You maintain sight on the doorway through a gap between appliances,
ready to move if they head this way.

> throw pan west
You grab a heavy cast iron pan and hurl it toward the dining room.
*CLANG!* It crashes against the table with a tremendous noise.

> listen north
"What the hell was that?" The intruder's voice, male, nervous.
Footsteps quickly move away from you toward the dining room.

> whisper south
You cup your hands and whisper through the basement door crack:
"Kids, stay downstairs and lock the door! Don't come up no matter what!"

> lean west
The intruder is in the dining room now, flashlight beam dancing
across the walls as he investigates the noise.

> position near south door
You quietly move to the basement door, ready to escape downstairs
if they return to the kitchen.

> survey area
You assess your tactical situation:
North: Living room (intruder was here, now empty)
East: Breakfast nook (good cover, but dead end)
South: Basement door (escape route, kids are safe below)
West: Dining room (intruder currently searching)

> wait
You hold your breath and wait. The house creaks. Distant traffic.
Then... the front door slams. Footsteps on the porch, fading away.

> look north
The living room is empty. Your family photos are scattered on the floor,
but the intruder is gone. You survived.
```

### 🏰 Prison Escape - Stealth Mission
```
=== SETUP ===
Your cell door clicks open unexpectedly. The corridor is dimly lit,
and you can hear guards patrolling nearby...

=== STEALTH SEQUENCE ===

> lean east
You cautiously peer out into the corridor. A guard walks past,
flashlight sweeping back and forth. He hasn't noticed the unlocked door yet.

> listen west
Heavy boots echo from the far end of the corridor, getting softer.
The guard is moving away from your position.

> listen north
From the guard station: radio chatter, the rustle of paperwork.
At least one guard is on duty there.

> position near south wall
You press your back against the cold stone wall beside your door frame,
making yourself as small as possible in the shadows.

> look behind
Your cell is dark behind you. The makeshift rope you've been working on
hangs from the window - your backup escape plan if this goes wrong.

> survey area
You mentally map your escape routes:
North: Guard station (occupied, avoid at all costs)
East: Main corridor toward exit (currently clear)
South: Dead end, but has maintenance access
West: Corridor extends toward other cell blocks
Up: Ventilation grate in ceiling (locked from outside)

> listen east
Footsteps approaching again. The guard is making his rounds,
coming back this way. You have maybe thirty seconds.

> crouch low east
You drop to a crawl and slip out of your cell, staying beneath
the guard's eye level and light sweep pattern.

> throw shoe north
Your worn prison shoe flies down the corridor toward the guard station.
*thud* "What was that? Charlie, did you hear something?"

> hide behind east support beam
You press yourself behind a concrete pillar as voices echo:
"Probably just rats again. This place is falling apart."

> listen around
The guards are distracted, investigating the noise to the north.
Your path east is clear for the moment.

> position near east exit
You creep toward the corridor exit, staying in the shadows.
The maintenance tunnel entrance is just ahead.

> lean through exit
Beyond the door: The main prison yard, floodlights sweeping.
Guard towers in the distance. This is it - freedom or capture.

> look behind
The corridor behind you is still clear. No alarm has been raised yet.
You've made it this far without detection.

> survey final position
Your escape options:
East: Prison yard (heavily guarded, but leads to outer fence)
Down: Storm drain access (risky, but unmonitored)
Back: Return to cell (safe, but you'll never get this chance again)

The choice is yours...
```

### 👻 Haunted House Investigation - Supernatural Horror
```
=== SETUP ===
The old Blackwood mansion has been abandoned for decades.
You're here to investigate the disappearances...

=== SUPERNATURAL ENCOUNTER ===

> listen up
Something heavy is being dragged across the floor above you.
*scraaaape* ... *scraaaape* ... rhythmic, deliberate.

> lean west
Through the parlor doorway you see:
Shadows moving independently of any light source, dancing across
the walls in patterns that make your eyes water.

> look behind
The heavy front door you entered through is now... closed.
You didn't close it. The antique lock has somehow engaged itself.

> smell north
A sickly sweet scent of decay drifts from the dining room,
mixed with something else... old roses and copper pennies.

> listen north
Beneath the dragging sounds from upstairs: whispers.
Multiple voices, but you can't make out the words.

> survey area
You feel watched as you assess your surroundings:
North: Dining room (foul smell, whispers, possible danger)
East: Library (books scattered, darkness beyond your flashlight)
South: Front door (locked, your planned exit route blocked)
West: Parlor (supernatural shadow activity)
Up: Second floor (heavy dragging continues overhead)

> position center room
You stay in the middle of the foyer, keeping all exits in view.
The chandelier above creaks ominously with each step upstairs.

> watch up
You track the movement across the ceiling. Whatever's up there
is moving from room to room, always staying directly above you.

> listen west
From the parlor: the sound of piano keys being pressed,
but no melody - just random, discordant notes.

> lean north into dining room
Peering into the dining room, your flashlight reveals:
A long table set for dinner. Twelve places, twelve chairs.
But the meal on the plates is decades old, crawling with...

> smell around
The smell is stronger now. Not just decay - something was burned here.
Fabric, maybe. Or hair.

> whisper "Hello?"
Your voice echoes strangely in the foyer. The dragging upstairs stops.
Complete silence. Even the whispers have ceased.

> look behind quickly
You spin around. The front door is now slightly ajar.
Fresh night air flows in, carrying the scent of rain.

> survey area again
Everything has changed:
North: Dining room (now completely dark, your flashlight won't penetrate)
East: Library (books are reshuffling themselves on the shelves)
South: Front door (mysteriously reopened - escape possible?)
West: Parlor (piano playing a waltz now, but no one's there)
Up: Silence (whatever was dragging has stopped moving)

> position near south door
You back toward the exit, keeping your flashlight trained
on the other doorways. Something doesn't want you to leave easily.

> lean out south door
The porch extends into darkness. Your car is still there,
but the headlights are on. You didn't leave them on.

> listen behind
From deep in the house: footsteps coming down the stairs.
Slow, deliberate. Someone - or something - is coming to greet you.

The choice: Investigate further or flee while you can?
```

### 🕵️‍♂️ Spy Infiltration - Mission Impossible Style
```
=== SETUP ===
You've infiltrated the embassy through the service entrance.
The stolen documents are in the ambassador's office on the third floor...

=== INFILTRATION SEQUENCE ===

> survey area
You're in the service corridor, checking your entry point:
North: Kitchen (staff voices, clattering dishes)
East: Main hallway (marble floors - footsteps will echo)
South: Service stairs (unguarded, leads to upper floors)
West: Storage room (could provide cover if needed)

> listen north
Chef shouting orders: "The ambassador wants his coffee in ten minutes!"
Multiple staff moving around. Too risky to go through kitchen.

> lean east
The main hallway is elegant but exposed. A security guard
paces past every few minutes, checking his watch.

> position near west storage room
You ready yourself next to the storage room door,
prepared to duck inside if anyone comes this way.

> listen east carefully
Footsteps on marble... getting closer. The guard is approaching
on his rounds. You have seconds to decide.

> hide in west storage room
You slip inside among cleaning supplies and catering equipment.
Through the crack in the door, you watch the guard pass by.

> wait
Guard's footsteps fade toward the north corridor.
You count to thirty, then emerge from hiding.

> crouch near south stairs
You approach the service stairs in a low crouch,
minimizing your silhouette against the wall lights.

> listen up
From the second floor: typing sounds from an office,
a phone ringing twice then stopping.

> lean up stairwell
The stairs curve upward. Soft carpet - good for quiet movement.
A security camera points down from the third floor landing.

> survey stair route
Tactical assessment of vertical movement:
Up 1 floor: Administrative offices (some occupied)
Up 2 floors: Ambassador's office (target location)
Camera coverage: Landing on 3rd floor (avoid or disable?)

> throw coin up two flights
You toss a coin up the stairwell toward the third floor.
*clink* It hits the camera housing, adjusting its angle slightly.

> crouch up stairs
Moving in a low crouch, you stay below the camera's adjusted line of sight
as you ascend to the second floor landing.

> listen around second floor
East: Typing continues (secretary working late)
West: Television murmur (someone watching news)
Up: Silence from third floor (ambassador absent?)

> lean toward third floor
Carefully checking upward: the third floor hallway is dark
except for light under one door - the ambassador's office.

> position below camera
You press against the wall directly beneath the security camera.
Its blind spot gives you cover to plan your final approach.

> survey final approach
Mission parameters:
Target: Ambassador's office (occupied, light on)
Security: Camera above (currently angled away)
Witnesses: Staff on floor below (could hear loud noises)
Escape: Same route back, or emergency window exit?

> lean toward office door
Through the frosted glass: a figure at the desk, reading documents.
This is it - the moment of truth.

The mission continues...
```

### 🧟‍♂️ Zombie Apocalypse - Survival Horror
```
=== SETUP ===
The zombie outbreak hit three days ago. You're holed up in a abandoned
grocery store, but you hear them getting closer...

=== SURVIVAL SEQUENCE ===

> listen around
From all directions: low moaning, shuffling footsteps.
They're surrounding the building. How many? Hard to tell.

> lean north toward front windows
Through the grimy storefront glass: shapes moving in the fog.
Slow, shambling figures. At least a dozen visible.

> survey area
Your defensive position assessment:
North: Front entrance (barricaded, but zombies gathering)
East: Loading dock (reinforced, but they know you're here)
South: Employee break room (one small window, possible escape)
West: Storage area (supplies, but no exit)
Up: Roof access (via storage room ladder)

> smell around
The stench of death seeps through every crack in your barricade.
Mixed with the smell of rotting food from the store.

> listen north carefully
*THUD* *THUD* Something heavy hitting the front door.
Your barricade of shopping carts shifts slightly.

> position center store
You move to the middle aisle, keeping equal distance
from all potential breach points.

> look behind toward south exit
The employee break room window is still intact.
It's your planned escape route if they break through.

> listen up
From the roof: scraping sounds. Are they climbing up there?
Or just debris blowing in the wind?

> lean east toward loading dock
The metal rolling door is holding, but you can see shadows
of multiple figures pressed against the small windows.

> survey supplies
Quick inventory of your resources:
Weapons: Baseball bat, kitchen knife, broken bottle
Food: Enough for 2 days
Water: 3 bottles remaining
Barriers: Shopping carts, pallets (currently holding)

> listen west into storage
From the storage room: a new sound. Scratching at the walls.
They've found the weak point in your defenses.

> position near south escape route
You back toward the break room, ready to flee
if they breach the storage room wall.

> lean toward storage carefully
Through the doorway: plaster dust falling from the wall.
Bloody fingernails scraping through gaps in the drywall.

> throw can west
You hurl a soup can toward the far corner of storage.
*CRASH* The scratching stops, then moves toward the noise.

> survey final options
Escape decision time:
North: Front door (heavily besieged, certain death)
East: Loading dock (surrounded, but reinforced)
South: Break room window (unknown dangers outside, but possible)
West: Storage (compromised, they're breaking through)

> listen south toward window
From outside the break room: silence. Either empty,
or they're waiting quietly for you to emerge.

> position at window
You approach the small window, baseball bat ready.
The glass is dirty, hard to see what's waiting outside.

> lean out window carefully
The back alley appears empty, but in this fog...
anything could be lurking in the shadows.

The choice: Break the window and run, or make your last stand?
```

### 🚢 Sinking Ship - Disaster Survival
```
=== SETUP ===
The cruise ship has struck something. Water is flooding the lower decks.
You need to reach the lifeboats before it's too late...

=== DISASTER ESCAPE ===

> survey area
The corridor is tilting as the ship lists to starboard:
North: Stairs to upper decks (where lifeboats are)
East: Passenger cabins (people might be trapped)
South: Main dining hall (flooding rapidly)
West: Ship's hull (water rushing in through breach)

> listen around
Panic everywhere: screaming, rushing water, metal groaning
under stress. The ship's PA system crackles with evacuation orders.

> lean north up stairs
The stairwell is packed with panicking passengers.
Pushing, shoving, some have fallen and are being trampled.

> listen west
The terrifying sound of rushing water getting louder.
The hull breach is expanding. This section will flood soon.

> look behind south
The dining hall is already waist-deep in freezing seawater.
Tables and chairs floating like debris in a river.

> position near east cabins
You move toward the passenger corridor. Someone might need help,
and you need to find another route to the lifeboats.

> listen east into cabins
From cabin 247: "Help! The door is jammed! Please!"
A woman's voice, desperate, pounding on the door from inside.

> lean toward cabin door
Through the door's window: water seeping under the door.
The woman inside is trapped, water already around her ankles.

> survey rescue options
Tactical decision:
Help: Try to break down door (takes time, ship is sinking)
Escape: Head for lifeboats immediately (woman dies, you might live)
Alternative: Find crew member with master key?

> throw shoulder against door
*CRACK* The door frame splinters but holds.
"I can hear you! Keep trying!" the woman shouts.

> listen around urgently
The ship lurches further to starboard. Water in the corridor
now, flowing like a river toward the lower decks.

> position for another attempt
You brace against the opposite wall for leverage.
The ship's tilt actually helps - gravity assists your next charge.

> throw full weight at door
*CRASH* The door bursts open. Water rushes out as a woman
in a life vest stumbles into your arms.

> survey new situation
With passenger rescued:
North: Stairs (still crowded, but now you're responsible for two)
East: More cabins (other people trapped?)
South: Rising water (no longer an option)
West: Hull breach (getting closer)

> lead north quickly
"Stay behind me!" You push through the crowd on the stairs,
helping the woman keep her balance as the ship tilts further.

> look behind constantly
The woman follows, but she's terrified and moving slowly.
The water level is rising faster than you're climbing.

> listen up
From the upper deck: "Final call for lifeboat stations!
This is not a drill! Abandon ship immediately!"

> lean toward lifeboat deck
Through the stairwell windows: you can see the lifeboats.
Some are already being lowered. Time is running out.

> position protectively
You shield the woman from the panicking crowd,
using your body to create space for both of you to move.

> survey final sprint
The last push to survival:
Up: Lifeboat deck (salvation, but chaos)
Behind: Rising water (certain death)
Around: Panicking passengers (might push you back down)

The ship groans. You have minutes, maybe less...
```

---

## 🎯 Implementation for Slice 1

### Core Spatial Features:
- **Basic lean commands** - "lean north" to peek
- **Simple listen mechanics** - "listen south" for audio cues
- **Position awareness** - "look behind" functionality
- **Pathway-integrated visibility** - lean shows filtered room content

### Advanced Features (Future Slices):
- Complex positioning systems
- Multi-directional awareness
- Paranoia and tension mechanics
- Full environmental audio simulation

**TAF's spatial awareness system transforms text adventures from "read-respond" to "inhabit-navigate"** 🎭👁️

This is our **Silent Hill: Shattered Memories moment** - the innovation that changes everything! 🚀✨