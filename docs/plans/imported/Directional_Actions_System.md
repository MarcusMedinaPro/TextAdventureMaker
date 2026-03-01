# 🎯 Directional Actions System - Tactical Gameplay Revolution

## Revolutionary Concept: Directions as Action Targets

### Traditional vs TAF Approach:
```
Traditional:
"throw wallet" → vague action, unclear target

TAF Directional Actions:
"throw wallet north" → specific target room/direction
"fire gun south" → shoot at approaching enemy
"shout east" → call to someone in adjacent room
"smoke signal up" → send message to tower above
```

## 🎮 Core Directional Action Categories

### 1. **Defensive Actions**
```csharp
// Throw items to distract/escape
"throw wallet north" → Distraction for robber
"toss coins west" → Misdirect pursuers
"drop lamp east" → Create light in dark passage
"scatter papers south" → Block follower's view

// Create barriers
"push table north" → Block doorway
"tip bookshelf east" → Create obstacle
"break plank west" → Seal passage
```

### 2. **Offensive Actions**
```csharp
// Ranged combat
"fire gun south" → Shoot approaching enemy
"shoot arrow north" → Attack distant target
"throw knife east" → Ranged weapon attack
"cast fireball west" → Magic ranged attack

// Area effects
"throw grenade up" → Attack room above
"spray poison north" → Gas attack through door
"blast magic south" → Area damage spell
```

### 3. **Communication Actions**
```csharp
// Social interaction
"shout north" → Call to person in next room
"whisper east" → Quiet communication
"signal up" → Hand gestures to tower
"knock west" → Tap message on wall

// Information gathering
"listen south" → Eavesdrop on adjacent room
"peek north" → Already implemented looking
"smell east" → Detect scents from direction
```

### 4. **Environmental Manipulation**
```csharp
// Object placement
"place bomb north" → Time-delayed trap
"set trap east" → Prepare ambush
"plant seed down" → Grow barrier/ladder
"pour oil south" → Create slippery surface

// Utility actions
"send rope up" → Help someone climb
"lower ladder down" → Create access
"extend bridge east" → Connect areas
```

## 🏗️ Technical Architecture

### DirectionalAction base class:
```csharp
public abstract class DirectionalAction
{
    public string Verb { get; set; }
    public GameObject UsedObject { get; set; }
    public Direction Direction { get; set; }
    public Room SourceRoom { get; set; }
    public Room TargetRoom { get; set; }
    public Pathway Pathway { get; set; }

    // Core execution pipeline
    public ActionResult Execute(Player player, GameContext context)
    {
        var validation = ValidateAction(player, context);
        if (!validation.IsValid)
            return validation.ToActionResult();

        var trajectory = CalculateTrajectory();
        var impact = ProcessImpact(trajectory);
        var consequences = HandleConsequences(impact);

        return new ActionResult
        {
            Success = true,
            Description = GenerateDescription(impact, consequences),
            WorldChanges = consequences
        };
    }

    protected abstract ValidationResult ValidateAction(Player player, GameContext context);
    protected abstract Trajectory CalculateTrajectory();
    protected abstract ImpactResult ProcessImpact(Trajectory trajectory);
    protected abstract List<WorldChange> HandleConsequences(ImpactResult impact);
}
```

### Specific action implementations:
```csharp
public class ThrowItemAction : DirectionalAction
{
    protected override ValidationResult ValidateAction(Player player, GameContext context)
    {
        if (!player.HasItem(UsedObject))
            return ValidationResult.Fail($"Du har ingen {UsedObject.Name}.");

        if (!Pathway.AllowsProjectiles)
            return ValidationResult.Fail($"Du kan inte kasta saker åt {Direction.ToSwedish()} härifrån.");

        if (Pathway.IsBlocked)
            return ValidationResult.Fail($"Vägen är blockerad åt {Direction.ToSwedish()}.");

        return ValidationResult.Success();
    }

    protected override Trajectory CalculateTrajectory()
    {
        var trajectory = new Trajectory
        {
            StartRoom = SourceRoom,
            Direction = Direction,
            Range = UsedObject.ThrowRange,
            ArcType = UsedObject.ThrowArc, // High, Low, Straight
            Accuracy = CalculateAccuracy()
        };

        // Physics simulation
        if (Pathway.IsOpen)
        {
            trajectory.TargetRoom = TargetRoom;
            trajectory.LandsSuccessfully = true;
        }
        else if (Pathway.IsTransparent) // Glass, bars
        {
            trajectory.HitsObstacle = Pathway;
            trajectory.LandsSuccessfully = false;
        }
        else // Solid door
        {
            trajectory.BlockedBy = Pathway;
            trajectory.LandsSuccessfully = false;
        }

        return trajectory;
    }

    protected override ImpactResult ProcessImpact(Trajectory trajectory)
    {
        var impact = new ImpactResult();

        if (trajectory.LandsSuccessfully)
        {
            // Object lands in target room
            impact.ObjectLocation = trajectory.TargetRoom;
            impact.Effect = $"{UsedObject.Name} flyger genom luften och landar i {trajectory.TargetRoom.Name}.";

            // Check for NPCs/targets in target room
            var npcsInRoom = trajectory.TargetRoom.GetNPCs();
            foreach (var npc in npcsInRoom)
            {
                if (npc.Position == NPCPosition.NearEntrance(Direction.GetOpposite()))
                {
                    impact.HitTargets.Add(npc);
                    impact.Effect += $" {UsedObject.Name} träffar {npc.Name}!";
                }
            }
        }
        else if (trajectory.HitsObstacle != null)
        {
            // Object hits barrier
            impact.ObjectLocation = SourceRoom; // Bounces back
            impact.Effect = $"{UsedObject.Name} studsar mot {trajectory.HitsObstacle.GetDescription()}.";

            // Damage to barrier if applicable
            if (UsedObject.IsHeavy && trajectory.HitsObstacle.IsFragile)
            {
                impact.BarrierDamage = true;
                impact.Effect += $" {trajectory.HitsObstacle.GetDescription()} får sprickor.";
            }
        }

        return impact;
    }
}

public class RangedCombatAction : DirectionalAction
{
    protected override ValidationResult ValidateAction(Player player, GameContext context)
    {
        var weapon = UsedObject as RangedWeapon;
        if (weapon == null)
            return ValidationResult.Fail($"{UsedObject.Name} är inte ett avståndsvapen.");

        if (!weapon.HasAmmunition)
            return ValidationResult.Fail($"{weapon.Name} har ingen ammunition.");

        if (!Pathway.AllowsRangedAttacks)
            return ValidationResult.Fail($"Du kan inte skjuta åt {Direction.ToSwedish()} härifrån.");

        return ValidationResult.Success();
    }

    protected override ImpactResult ProcessImpact(Trajectory trajectory)
    {
        var impact = new ImpactResult();
        var weapon = UsedObject as RangedWeapon;

        // Find targets in line of fire
        var targets = FindTargetsInDirection(trajectory);

        if (targets.Any())
        {
            var primaryTarget = targets.First();
            var hitRoll = CalculateHitChance(weapon, primaryTarget, trajectory);

            if (hitRoll.Success)
            {
                var damage = CalculateDamage(weapon, primaryTarget);
                impact.CombatResults.Add(new CombatResult
                {
                    Target = primaryTarget,
                    Damage = damage,
                    Description = $"Du träffar {primaryTarget.Name} med {weapon.Name}!"
                });

                if (damage >= primaryTarget.Health)
                {
                    impact.Effect += $" {primaryTarget.Name} faller till marken!";
                }
            }
            else
            {
                impact.Effect = $"Du missar {primaryTarget.Name}!";
            }
        }
        else
        {
            impact.Effect = $"Din {weapon.Name} avfyras åt {Direction.ToSwedish()}, men träffar inget.";
        }

        // Consume ammunition
        weapon.ConsumeAmmunition();

        return impact;
    }
}
```

## 🎯 Pathway Integration

### Enhanced pathway properties:
```csharp
public class Pathway
{
    // Existing properties...
    public bool IsOpen { get; set; } = true;
    public bool IsTransparent { get; set; } = false;

    // NEW: Action-specific properties
    public bool AllowsProjectiles { get; set; } = true;
    public bool AllowsRangedAttacks { get; set; } = true;
    public bool AllowsSound { get; set; } = true;
    public bool AllowsSmells { get; set; } = true;

    // Physical properties affecting actions
    public float ProjectileAccuracyModifier { get; set; } = 1.0f;
    public bool IsFragile { get; set; } = false; // Can be broken by thrown objects
    public List<DamageResistance> Resistances { get; set; } = new();

    public ActionResult ProcessProjectile(GameObject projectile, Direction direction)
    {
        if (!AllowsProjectiles)
            return ActionResult.Blocked($"{projectile.Name} studsar mot {GetDescription()}.");

        if (IsOpen || IsTransparent)
            return ActionResult.Success($"{projectile.Name} passerar genom {GetDescription()}.");

        // Hit solid barrier
        var result = new ActionResult();
        result.Description = $"{projectile.Name} träffar {GetDescription()}.";

        if (projectile.IsHeavy && IsFragile)
        {
            TakeDamage(projectile.ImpactDamage);
            result.Description += $" {GetDescription()} tar skada!";
        }

        return result;
    }
}
```

## 🎮 Advanced Tactical Scenarios

### 1. **Robber Encounter**
```csharp
// Setup
var alley = world.GetRoom("dark_alley");
var street = world.GetRoom("main_street");
var robber = new NPC("robber").WithHostility(true);
alley.AddNPC(robber);

// Player scenario
"A dangerous-looking robber steps out from the shadows!"

// Player options:
"throw wallet north" →
"You throw your wallet toward the street. The robber chases after it, allowing you to escape!"

"fire gun south" →
"You shoot at the robber! *BANG* The robber stumbles and flees into the darkness."

"shout north" →
"You shout for help toward the main street. People hear you and come running!"
```

### 2. **Siege Defense**
```csharp
// Castle under attack
"Enemies approach from the south!"

"drop oil south" →
"You pour boiling oil down on the attackers below. They scatter in pain!"

"fire arrow south" →
"Your arrow finds its mark in the lead attacker. The others hesitate."

"push boulder down" →
"The massive boulder crashes down, creating a barrier and crushing several enemies."
```

### 3. **Stealth Communication**
```csharp
// Prison escape scenario
"You hear your ally in the cell to the east."

"whisper east" →
"You whisper through the wall. Your ally responds - they have the key!"

"tap wall east" →
"You tap a coded message. Three taps back confirm - the plan is on."

"slide note east" →
"You slip the escape plan under the wall gap. Success!"
```

## 🔊 Audio & Environmental Actions

### Sound-based directional actions:
```csharp
public class SoundAction : DirectionalAction
{
    public SoundLevel Volume { get; set; }
    public SoundType Type { get; set; }

    // Sound travels through pathways based on their properties
    protected override ImpactResult ProcessImpact(Trajectory trajectory)
    {
        var impact = new ImpactResult();
        var soundRange = CalculateSoundRange(Volume, Pathway);

        var roomsInRange = GetRoomsInSoundRange(trajectory.Direction, soundRange);

        foreach (var room in roomsInRange)
        {
            var npcsInRoom = room.GetNPCs();
            foreach (var npc in npcsInRoom)
            {
                npc.OnHearSound(Type, Volume, trajectory.Direction.GetOpposite());
                impact.AlertedNPCs.Add(npc);
            }
        }

        return impact;
    }
}

// Usage examples:
"shout north" → NPCs in northern rooms hear and react
"whisper east" → Only adjacent room receives message
"slam door south" → Loud noise alerts multiple rooms
"play music up" → Reaches upper floors, may attract/distract
```

## 🧪 Physics & Realism

### Realistic trajectory calculations:
```csharp
public class TrajectoryCalculator
{
    public Trajectory CalculateThrow(GameObject item, Direction direction, Room source)
    {
        var trajectory = new Trajectory();

        // Object properties affect trajectory
        trajectory.Range = item.Weight < 1.0f ? 2 : 1; // Light objects go further
        trajectory.Accuracy = item.Size == Size.Small ? 0.8f : 0.6f; // Small objects more accurate
        trajectory.ArcHeight = item.Shape == Shape.Round ? ArcHeight.Medium : ArcHeight.Low;

        // Environmental factors
        if (source.HasProperty("windy"))
            trajectory.Accuracy *= 0.7f;

        if (source.HasProperty("narrow"))
            trajectory.Accuracy *= 0.5f; // Hard to throw in tight spaces

        return trajectory;
    }
}
```

### Object interaction matrix:
```csharp
public static class ObjectInteractions
{
    private static readonly Dictionary<(ObjectType, MaterialType), InteractionResult> Interactions = new()
    {
        // Throwing interactions
        [(ObjectType.Rock, MaterialType.Glass)] = InteractionResult.Break,
        [(ObjectType.Paper, MaterialType.Fire)] = InteractionResult.Burn,
        [(ObjectType.Metal, MaterialType.Magnet)] = InteractionResult.Attract,

        // Projectile vs barrier
        [(ObjectType.Arrow, MaterialType.Wood)] = InteractionResult.Stick,
        [(ObjectType.Bullet, MaterialType.Metal)] = InteractionResult.Ricochet,
        [(ObjectType.Magic, MaterialType.AntiMagic)] = InteractionResult.Dispel,
    };
}
```

## 🎯 Parser Integration

### Enhanced command parsing:
```csharp
public class DirectionalActionParser
{
    public ParsedCommand ParseDirectionalAction(string input)
    {
        var patterns = new Dictionary<Regex, Func<Match, ParsedCommand>>
        {
            // "throw wallet north"
            [new Regex(@"throw (.*) (north|south|east|west|up|down)")] = match =>
                new ParsedCommand("throw", object: match.Groups[1].Value, direction: match.Groups[2].Value),

            // "fire gun south"
            [new Regex(@"(fire|shoot) (.*) (north|south|east|west|up|down)")] = match =>
                new ParsedCommand("ranged_attack", object: match.Groups[2].Value, direction: match.Groups[3].Value),

            // "shout north"
            [new Regex(@"(shout|yell|scream|call) (north|south|east|west|up|down)")] = match =>
                new ParsedCommand("shout", direction: match.Groups[2].Value),

            // "whisper east"
            [new Regex(@"whisper (.*) (north|south|east|west|up|down)")] = match =>
                new ParsedCommand("whisper", message: match.Groups[1].Value, direction: match.Groups[2].Value),

            // "push table north" (move furniture to block)
            [new Regex(@"push (.*) (north|south|east|west|up|down)")] = match =>
                new ParsedCommand("push_directional", object: match.Groups[1].Value, direction: match.Groups[2].Value)
        };

        foreach (var (pattern, parser) in patterns)
        {
            var match = pattern.Match(input.ToLower());
            if (match.Success)
                return parser(match);
        }

        return null;
    }
}
```

## 🚀 Benefits & Impact

### Revolutionary Gameplay:
✅ **Tactical Positioning** - rum blir tactical spaces istället för bara containers
✅ **Ranged Interaction** - agera på distance utan movement
✅ **Environmental Strategy** - använd directions för problem-solving
✅ **Immersive Physics** - realistiska trajectory och impact calculations

### Enhanced Narrative:
✅ **Dramatic Moments** - "throw wallet north" creates tension and story
✅ **Player Agency** - multiple solutions till varje problem
✅ **Emergent Gameplay** - unexpected interactions mellan systems
✅ **Classic Feel** - påminner om bästa gamla textäventyr

### Technical Excellence:
✅ **Pathway Integration** - använder existing infrastructure
✅ **Modular Design** - easy att lägga till nya directional actions
✅ **Physics Ready** - realistic trajectory calculations
✅ **Future Proof** - foundation för advanced features

---

## 🎯 Integration med Slice 1

### Core Features (Slice 1):
- **Basic throw actions** - "throw wallet north"
- **Simple sound actions** - "shout north"
- **Pathway validation** - check if direction allows action
- **Basic impact resolution** - object lands in target room

### Advanced Features (Future Slices):
- Complex trajectory physics
- Ranged combat systems
- Environmental interactions
- Multi-room sound propagation

**Directional Actions revolutionerar textäventyr från "go places" till "interact tactically with space"!** 🎯🚀

Detta är GENIUS - TAF becomes the first framework där directions är inte bara navigation utan tactical tools! 💥✨