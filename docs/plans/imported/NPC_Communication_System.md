# 🗣️ NPC Communication System - Spatial Social Interactions

## Revolutionary Concept: Directional Communication

### Spatial Awareness Meets Social Interaction
```csharp
// Inte bara "talk to guard" - spatial communication!
"shout north"      → Ropa till NPC i rummet norrut
"whisper east"     → Viska till NPC österut
"threaten south"   → Hota NPC söderut från distance
"compliment west"  → Berömma NPC västerut
"gesture up"       → Signalera till NPC på våningen ovan
```

## 🎯 Core Spatial Communication Mechanics

### Bidirectional Awareness System
```csharp
public class SpatialAwarenessEngine
{
    // Om spelare kan se NPC, kan NPC se spelare!
    public List<NPC> GetMutuallyVisibleNPCs(Player player, Direction direction)
    {
        var pathway = player.CurrentRoom.GetPathway(direction);
        if (pathway?.AllowsVisualContact != true) return new List<NPC>();

        var targetRoom = pathway.GetTargetRoom(player.CurrentRoom);
        var visibleNPCs = targetRoom.GetNPCs()
            .Where(npc => npc.CanSeeInDirection(direction.GetOpposite()))
            .Where(npc => !npc.IsHiding)
            .ToList();

        // NPCs blir medvetna om spelaren
        foreach (var npc in visibleNPCs)
        {
            npc.NoticePlayer(player, direction.GetOpposite());
        }

        return visibleNPCs;
    }
}

public class NPC
{
    public SpatialAwareness Awareness { get; set; } = new();
    public PersonalityProfile Personality { get; set; } = new();
    public RelationshipStatus RelationshipWithPlayer { get; set; } = new();

    // NPC märker när spelare tittar på dem
    public void NoticePlayer(Player player, Direction fromDirection)
    {
        Awareness.PlayerLastSeen = DateTime.UtcNow;
        Awareness.PlayerDirection = fromDirection;
        Awareness.PlayerVisible = true;

        // Personality påverkar reaktion
        if (Personality.Traits.Contains("paranoid"))
        {
            OnPlayerSpotted?.Invoke(player, fromDirection);
        }

        if (Personality.Traits.Contains("friendly"))
        {
            ConsiderWaving(player, fromDirection);
        }
    }

    // NPCs kan initiera kommunikation tillbaka
    public void ConsiderWaving(Player player, Direction toDirection)
    {
        if (RelationshipWithPlayer.Level >= RelationshipLevel.Neutral)
        {
            var waveAction = new DirectionalSocialAction
            {
                Type = SocialActionType.Wave,
                Direction = toDirection,
                Message = $"{Name} vinkar glatt åt dig från {CurrentRoom.Name}!"
            };

            ExecuteAction(waveAction);
        }
    }
}
```

### Directional Social Actions
```csharp
public class DirectionalSocialAction : DirectionalAction
{
    public SocialActionType Type { get; set; }
    public string Message { get; set; }
    public EmotionalTone Tone { get; set; }
    public float Volume { get; set; } = 1.0f; // 0.1 = whisper, 2.0 = shout

    protected override ValidationResult ValidateAction(Player player, GameContext context)
    {
        if (!Pathway.AllowsSound && Type.RequiresSound())
            return ValidationResult.Fail($"Ljud når inte genom {Pathway.GetDescription()}.");

        if (!Pathway.AllowsVisualContact && Type.RequiresVisibility())
            return ValidationResult.Fail($"Du kan inte se {Direction.ToSwedish()} för att {Type.ToSwedish()}.");

        var targetNPCs = context.GetNPCsInDirection(player.CurrentRoom, Direction);
        if (!targetNPCs.Any())
            return ValidationResult.Fail($"Det finns ingen att {Type.ToSwedish()} åt {Direction.ToSwedish()}.");

        return ValidationResult.Success();
    }

    protected override ImpactResult ProcessImpact(Trajectory trajectory)
    {
        var impact = new ImpactResult();
        var targetRoom = trajectory.TargetRoom;
        var npcsInRoom = targetRoom.GetNPCs();

        foreach (var npc in npcsInRoom)
        {
            var reaction = npc.ReactToSocialAction(this);
            impact.NPCReactions.Add(reaction);

            // Update relationship baserat på action type
            UpdateRelationship(npc, Type, Tone);
        }

        // Sound propagation för hörbara actions
        if (Type.RequiresSound())
        {
            var soundRange = CalculateSoundRange(Volume);
            PropagateSound(trajectory, soundRange, impact);
        }

        return impact;
    }
}

public enum SocialActionType
{
    // Friendly actions
    Greet,      // "greet north"
    Wave,       // "wave east"
    Compliment, // "compliment south"
    Bow,        // "bow west"

    // Neutral communication
    Shout,      // "shout north"
    Call,       // "call east"
    Whisper,    // "whisper south"
    Gesture,    // "gesture up"

    // Hostile actions
    Threaten,   // "threaten north"
    Mock,       // "mock south"
    Intimidate, // "intimidate east"
    Insult,     // "insult west"

    // Information gathering
    Question,   // "question north" (vague inquiry)
    Spy,        // "spy on east" (stealthy observation)
    Eavesdrop   // "eavesdrop south" (listen secretly)
}
```

### NPC Reaction System
```csharp
public class NPCReactionEngine
{
    public NPCReaction ReactToSocialAction(NPC npc, DirectionalSocialAction action)
    {
        var reaction = new NPCReaction
        {
            NPC = npc,
            Action = action,
            Timestamp = DateTime.UtcNow
        };

        // Base reaction på personality
        var personalityModifier = CalculatePersonalityResponse(npc.Personality, action.Type);

        // Relationship påverkar reaction
        var relationshipModifier = CalculateRelationshipResponse(npc.RelationshipWithPlayer, action.Type);

        // Contextual factors
        var contextModifier = CalculateContextualResponse(npc, action);

        var totalResponse = personalityModifier + relationshipModifier + contextModifier;

        // Generate reaction baserat på total response
        reaction.Type = DetermineReactionType(totalResponse, action.Type);
        reaction.Message = GenerateReactionMessage(npc, action, reaction.Type);
        reaction.Consequence = DetermineConsequence(npc, action, reaction.Type);

        return reaction;
    }

    private float CalculatePersonalityResponse(PersonalityProfile personality, SocialActionType actionType)
    {
        return actionType switch
        {
            SocialActionType.Compliment => personality.GetTrait("vanity") * 0.8f + personality.GetTrait("friendliness") * 0.6f,
            SocialActionType.Threaten => -personality.GetTrait("courage") * 0.9f - personality.GetTrait("stubbornness") * 0.3f,
            SocialActionType.Whisper => personality.GetTrait("curiosity") * 0.7f - personality.GetTrait("suspicion") * 0.4f,
            SocialActionType.Shout => -personality.GetTrait("nervousness") * 0.6f + personality.GetTrait("boldness") * 0.3f,
            _ => 0.0f
        };
    }

    private string GenerateReactionMessage(NPC npc, DirectionalSocialAction action, ReactionType reactionType)
    {
        var direction = action.Direction.GetOpposite().ToSwedish();

        return reactionType switch
        {
            ReactionType.Positive => GeneratePositiveReaction(npc, action, direction),
            ReactionType.Negative => GenerateNegativeReaction(npc, action, direction),
            ReactionType.Neutral => GenerateNeutralReaction(npc, action, direction),
            ReactionType.Confused => GenerateConfusedReaction(npc, action, direction),
            _ => $"{npc.Name} reagerar inte på ditt {action.Type.ToSwedish()}."
        };
    }

    private string GeneratePositiveReaction(NPC npc, DirectionalSocialAction action, string direction)
    {
        return action.Type switch
        {
            SocialActionType.Compliment => $"{npc.Name} ler brett och vinkar tacksamt åt {direction}!",
            SocialActionType.Greet => $"{npc.Name} ropar glatt tillbaka: 'Hej där!' från {direction}.",
            SocialActionType.Wave => $"{npc.Name} vinkar entusiastiskt tillbaka från {direction}!",
            _ => $"{npc.Name} ser nöjd ut och nickar åt {direction}."
        };
    }

    private string GenerateNegativeReaction(NPC npc, DirectionalSocialAction action, string direction)
    {
        return action.Type switch
        {
            SocialActionType.Threaten => $"{npc.Name} blir arg och hotar tillbaka från {direction}: 'Våga inte!'",
            SocialActionType.Mock => $"{npc.Name} blir förolämpad och vänder dig ryggen åt {direction}.",
            SocialActionType.Shout when action.Volume > 1.5f => $"{npc.Name} täcker öronen och skriker: 'Sluta skrik!' från {direction}.",
            _ => $"{npc.Name} ser missnöjd ut och ignorerar dig från {direction}."
        };
    }
}
```

## 🎭 Advanced Communication Scenarios

### Multi-Room Conversations
```csharp
public class MultiRoomConversation
{
    public string ConversationId { get; set; }
    public Player Player { get; set; }
    public NPC NPC { get; set; }
    public List<Room> InvolvedRooms { get; set; } = new();
    public ConversationState State { get; set; } = ConversationState.Active;

    // Conversation som sträcker sig över flera rum
    public void HandleDirectionalDialogue(string playerInput, Direction direction)
    {
        var targetRoom = Player.CurrentRoom.GetConnectedRoom(direction);
        var npc = targetRoom?.GetNPCs().FirstOrDefault(n => n.Id == NPC.Id);

        if (npc == null)
        {
            Player.Output($"Du hör inget svar från {direction.ToSwedish()}.");
            return;
        }

        // Process conversation across distance
        var response = ProcessDistantDialogue(playerInput, direction);

        // NPC svarar genom samma pathway
        DeliverNPCResponse(response, direction.GetOpposite());
    }

    private string ProcessDistantDialogue(string input, Direction direction)
    {
        // Modify dialogue baserat på distance och pathway properties
        var pathway = Player.CurrentRoom.GetPathway(direction);

        if (!pathway.AllowsSound)
            return "Du hör inget svar - ljudet når inte genom.";

        if (pathway.SoundDistortion > 0.5f)
            return "Du hör mummel som svar, men kan inte urskilja orden.";

        // Normal conversation processing med distance modifier
        return NPC.ProcessDialogue(input, distanceModifier: 0.8f);
    }
}
```

### Stealth Communication
```csharp
public class StealthCommunicationSystem
{
    public ActionResult AttemptStealthyCommunication(Player player, Direction direction, string message)
    {
        var targetRoom = player.CurrentRoom.GetConnectedRoom(direction);
        var pathway = player.CurrentRoom.GetPathway(direction);

        // Check för detection risk
        var detectionRisk = CalculateDetectionRisk(player, pathway, message);

        if (detectionRisk > 0.7f && Random.Shared.NextDouble() < detectionRisk)
        {
            return HandleDetection(player, direction, targetRoom);
        }

        // Successful stealth communication
        var targetNPCs = targetRoom.GetNPCs()
            .Where(npc => npc.RelationshipWithPlayer.Level >= RelationshipLevel.Friendly)
            .ToList();

        if (targetNPCs.Any())
        {
            return DeliverSecretMessage(targetNPCs.First(), message, direction);
        }

        return ActionResult.Failure("Ingen att kommunicera hemligt med åt det hållet.");
    }

    private float CalculateDetectionRisk(Player player, Pathway pathway, string message)
    {
        float risk = 0.0f;

        // Pathway properties påverkar risk
        if (!pathway.AllowsStealth) risk += 0.8f;
        if (pathway.HasGuards) risk += 0.6f;
        if (pathway.IsWellLit) risk += 0.4f;

        // Message content påverkar risk
        if (message.Contains("escape") || message.Contains("plan")) risk += 0.3f;

        // Player skills minskar risk
        risk -= player.Stats.Stealth * 0.1f;

        return Math.Clamp(risk, 0.0f, 1.0f);
    }
}
```

### Emotional Communication
```csharp
public class EmotionalCommunicationEngine
{
    public void ProcessEmotionalAction(Player player, DirectionalSocialAction action)
    {
        var emotionalContext = AnalyzeEmotionalContext(player, action);

        // Modify action baserat på player's emotional state
        ModifyActionByEmotion(action, emotionalContext);

        // Execute action med emotional modifiers
        var result = ExecuteEmotionalAction(action);

        // Update player's emotional state baserat på result
        UpdatePlayerEmotionalState(player, result);
    }

    private EmotionalContext AnalyzeEmotionalContext(Player player, DirectionalSocialAction action)
    {
        return new EmotionalContext
        {
            PlayerMood = player.CurrentMood,
            StressLevel = player.Stats.Stress,
            RecentEvents = player.History.GetRecentEmotionalEvents(),
            RelationshipContext = GetRelationshipContext(player, action.Direction)
        };
    }

    private void ModifyActionByEmotion(DirectionalSocialAction action, EmotionalContext context)
    {
        // High stress makes shouting more aggressive
        if (context.StressLevel > 0.8f && action.Type == SocialActionType.Shout)
        {
            action.Tone = EmotionalTone.Aggressive;
            action.Volume *= 1.5f;
        }

        // Depression makes compliments less effective
        if (context.PlayerMood == Mood.Depressed && action.Type == SocialActionType.Compliment)
        {
            action.Tone = EmotionalTone.Halfhearted;
        }

        // Confidence boosts intimidation
        if (context.PlayerMood == Mood.Confident && action.Type == SocialActionType.Threaten)
        {
            action.Tone = EmotionalTone.Commanding;
        }
    }
}
```

## 🎮 Practical Usage Examples

### Prison Break Scenario
```csharp
// Spatial social communication för escape planning
"look east" →
"Genom järngallret ser du din medkomplice i cellen bredvid."

"whisper east 'guards change shift at midnight'" →
"Du viskar hemliga instruktioner. Din medkomplice nickar förstående."

"gesture east" →
"Du gör en diskret handrörelse. Din kompis förstår signalen."

// Guard detection risk
"shout east 'now!'" →
"Du ropar för högt! En vakt hör dig: 'Vad pågår där?'"
```

### Diplomatic Encounter
```csharp
"look north" →
"Genom det öppna fönstret ser du kungen i tronsalen."

"bow north" →
"Du böjer dig djupt mot tronsalen. Kungen ser dig och nickar uppskattande."

"compliment north 'Your Majesty looks magnificent today'" →
"Kungen ler brett från tronsalen: 'Du har gott öga för kvalitet!'"

// Relationship improvement
RelationshipWithPlayer.Level += 0.2f; // Compliment worked!
```

### Stealth Mission
```csharp
"spy on south" →
"Du smyger för att observera rummet söderut utan att bli upptäckt."

"eavesdrop south" →
"Du lyssnar försiktigt på samtalet i rummet söderut: 'Nycklarna ligger under matten...'"

"whisper south 'pssst'" →
"Du försöker fånga uppmärksamheten från någon söderut utan att väcka misstankar."
```

## 🔊 Sound Propagation & Communication

### Advanced Sound System
```csharp
public class SpatialSoundEngine
{
    public void PropagateSound(DirectionalSocialAction action, List<Room> affectedRooms)
    {
        var soundData = new SoundPropagationData
        {
            Source = action.SourceRoom,
            Type = action.Type.ToSoundType(),
            Volume = action.Volume,
            Message = action.Message,
            Direction = action.Direction
        };

        foreach (var room in affectedRooms)
        {
            var distance = CalculateDistance(action.SourceRoom, room);
            var attenuatedVolume = ApplyDistanceAttenuation(soundData.Volume, distance);

            if (attenuatedVolume > 0.1f) // Threshold för hearing
            {
                NotifyRoomOfSound(room, soundData, attenuatedVolume, distance);
            }
        }
    }

    private void NotifyRoomOfSound(Room room, SoundPropagationData sound, float volume, int distance)
    {
        var npcsInRoom = room.GetNPCs();

        foreach (var npc in npcsInRoom)
        {
            var hearingResult = npc.ProcessDistantSound(sound, volume, distance);

            if (hearingResult.Reacts)
            {
                ExecuteNPCResponse(npc, hearingResult.Response);
            }
        }

        // Player får också höra sounds från andra rum
        if (room.HasPlayer)
        {
            var player = room.GetPlayer();
            var soundDescription = DescribeSoundFromDistance(sound, distance);
            player.Output(soundDescription);
        }
    }

    private string DescribeSoundFromDistance(SoundPropagationData sound, int distance)
    {
        var direction = DetermineApproximateDirection(sound.Source);

        return distance switch
        {
            1 => $"Du hör {sound.Type.ToDescription()} från {direction}.",
            2 => $"Du hör svagt {sound.Type.ToDescription()} från {direction} håll.",
            3 => $"Du anar att du hör {sound.Type.ToDescription()} från långt borta åt {direction}.",
            _ => $"Du hör mycket svagt något från {direction} håll."
        };
    }
}
```

### NPC Sound Responses
```csharp
public class NPCSoundResponse
{
    public bool Reacts { get; set; }
    public string Response { get; set; }
    public NPCAction Action { get; set; }

    public static NPCSoundResponse CreateGuardResponse(SoundPropagationData sound, float volume)
    {
        if (sound.Type == SoundType.Shout && volume > 0.6f)
        {
            return new NPCSoundResponse
            {
                Reacts = true,
                Response = "Vakten hör ropet och börjar röra sig mot ljudet.",
                Action = new MoveTowardsSourceAction(sound.Source)
            };
        }

        if (sound.Type == SoundType.Whisper && volume > 0.3f)
        {
            return new NPCSoundResponse
            {
                Reacts = true,
                Response = "Vakten spetsar öronen, misstänksam.",
                Action = new IncreasedAlertAction()
            };
        }

        return new NPCSoundResponse { Reacts = false };
    }
}
```

## 🧪 Testing & Integration

### Communication System Tests
```csharp
[TestClass]
public class SpatialCommunicationTests
{
    [TestMethod]
    public void ShoutNorth_WithNPCInTargetRoom_NPCReacts()
    {
        // Arrange
        var world = CreateTestWorld();
        var player = new Player();
        var npc = new NPC("friendly_guard") { Personality = PersonalityProfile.Friendly() };

        world.GetRoom("kitchen").AddPlayer(player);
        world.GetRoom("living_room").AddNPC(npc);

        // Act
        var result = player.ExecuteCommand("shout north 'hello there!'");

        // Assert
        Assert.IsTrue(result.Success);
        Assert.Contains("friendly_guard", result.Description);
        Assert.IsTrue(npc.RelationshipWithPlayer.Level > 0); // Relationship improved
    }

    [TestMethod]
    public void WhisperEast_WithClosedDoor_SoundBlocked()
    {
        // Arrange
        var world = CreateTestWorld();
        var pathway = world.GetPathway("kitchen_to_bedroom");
        pathway.Close(); // Door is closed
        pathway.AllowsSound = false; // Solid door blocks sound

        var player = new Player();
        world.GetRoom("kitchen").AddPlayer(player);

        // Act
        var result = player.ExecuteCommand("whisper east 'can you hear me?'");

        // Assert
        Assert.IsFalse(result.Success);
        Assert.Contains("reach through", result.Description);
    }

    [TestMethod]
    public void ThreatenSouth_WithFriendlyNPC_RelationshipDamaged()
    {
        // Arrange
        var world = CreateTestWorld();
        var npc = new NPC("ally")
        {
            RelationshipWithPlayer = { Level = RelationshipLevel.Friendly }
        };

        // Act
        var result = ExecuteDirectionalSocialAction(
            new DirectionalSocialAction
            {
                Type = SocialActionType.Threaten,
                Direction = Direction.South
            });

        // Assert
        Assert.IsTrue(result.Success);
        Assert.IsTrue(npc.RelationshipWithPlayer.Level < RelationshipLevel.Friendly);
        Assert.Contains("angry", result.Description.ToLower());
    }
}
```

---

## 🚀 Benefits av Spatial NPC Communication

### För Spelare:
✅ **Rich Social Interaction** - kommunicera från distance, inte bara face-to-face
✅ **Tactical Communication** - använd spatial awareness för stealth eller diplomacy
✅ **Emergent Storytelling** - NPCs reagerar naturligt på dina spatial actions
✅ **Realistic Behavior** - om du ser dem, ser de dig också!

### För Game Designers:
✅ **Advanced NPC Behavior** - NPCs blir mer levande och reaktiva
✅ **Flexible Encounter Design** - kommunikation påverkar encounters
✅ **Relationship Mechanics** - spatial actions påverkar NPC relationships
✅ **Stealth & Diplomacy** - nya gameplay mechanics genom spatial communication

### För Utvecklare:
✅ **Modular Design** - bygger på befintliga spatial awareness systems
✅ **Event-Driven Architecture** - NPCs reagerar på player actions naturligt
✅ **Testable Systems** - clear input/output för automated testing
✅ **Performance Optimized** - sound propagation använder smart caching

**Spatial NPC Communication revolutionerar social interaction i textäventyr! 🗣️🚀**

Istället för bara "talk to guard", får vi "threaten north", "whisper east", "compliment south" - och NPCs som reagerar intelligent på spatial context och kan kommunicera tillbaka genom samma pathways!