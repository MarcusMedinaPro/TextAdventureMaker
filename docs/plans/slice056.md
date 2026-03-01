## Slice 56: Advanced NPC Communication

**Mål:** Utökad NPC-kommunikation med minne, personlighet, relationer och kontextmedveten dialog.

**Referens:** `docs/plans/imported/NPC_Communication_System.md`

### Task 56.1: INpcMemory interface

```csharp
public interface INpcMemory
{
    void Remember(string key, object value, MemoryType type = MemoryType.ShortTerm);
    T? Recall<T>(string key);
    bool Knows(string key);
    void Forget(string key);

    IEnumerable<MemoryEntry> GetRecentMemories(int count = 10);
    IEnumerable<MemoryEntry> GetMemoriesAbout(string subject);
}

public record MemoryEntry(
    string Key,
    object Value,
    MemoryType Type,
    DateTime Timestamp,
    int Importance  // 1-10
);

public enum MemoryType
{
    ShortTerm,   // Glöms efter några turns
    LongTerm,    // Persistent
    Emotional    // Påverkar relationer
}
```

### Task 56.2: NpcMemory implementation

```csharp
public class NpcMemory : INpcMemory
{
    private readonly Dictionary<string, MemoryEntry> _memories = [];
    private readonly int _shortTermCapacity = 20;

    public void Remember(string key, object value, MemoryType type = MemoryType.ShortTerm)
    {
        var entry = new MemoryEntry(key, value, type, DateTime.Now, CalculateImportance(key, value));
        _memories[key] = entry;

        if (type == MemoryType.ShortTerm)
            CleanupShortTermMemories();
    }

    public T? Recall<T>(string key) =>
        _memories.TryGetValue(key, out var entry) ? (T)entry.Value : default;

    public bool Knows(string key) => _memories.ContainsKey(key);

    public void OnTick()
    {
        // Short-term memories fade
        var toForget = _memories
            .Where(m => m.Value.Type == MemoryType.ShortTerm)
            .Where(m => (DateTime.Now - m.Value.Timestamp).TotalMinutes > 5)
            .Select(m => m.Key)
            .ToList();

        foreach (var key in toForget)
            _memories.Remove(key);
    }

    private void CleanupShortTermMemories()
    {
        var shortTerm = _memories
            .Where(m => m.Value.Type == MemoryType.ShortTerm)
            .OrderBy(m => m.Value.Importance)
            .ThenBy(m => m.Value.Timestamp)
            .ToList();

        while (shortTerm.Count > _shortTermCapacity)
        {
            _memories.Remove(shortTerm[0].Key);
            shortTerm.RemoveAt(0);
        }
    }
}
```

### Task 56.3: Personality System

```csharp
public class NpcPersonality
{
    // Big Five personality traits (0.0 - 1.0)
    public float Openness { get; init; } = 0.5f;
    public float Conscientiousness { get; init; } = 0.5f;
    public float Extraversion { get; init; } = 0.5f;
    public float Agreeableness { get; init; } = 0.5f;
    public float Neuroticism { get; init; } = 0.5f;

    // Derived behaviors
    public bool IsWillingToHelp => Agreeableness > 0.6f;
    public bool IsSuspicious => Neuroticism > 0.6f && Agreeableness < 0.4f;
    public bool IsTalkative => Extraversion > 0.6f;
    public bool IsReserved => Extraversion < 0.4f;

    public string GetMoodModifier(float relationship)
    {
        if (relationship > 50 && Extraversion > 0.5f)
            return "warmly";
        if (relationship < -20 && Agreeableness < 0.4f)
            return "coldly";
        if (Neuroticism > 0.7f)
            return "nervously";
        return "";
    }
}
```

### Task 56.4: Contextual Dialog System

```csharp
public class ContextualDialogSystem
{
    public string GenerateResponse(INpc npc, string topic, DialogContext context)
    {
        var memory = npc.Memory;
        var personality = npc.Personality;
        var relationship = context.State.GetRelationship(npc.Id);

        // Välj response baserat på context
        var responses = npc.DialogRules
            .Where(r => r.Matches(topic, context))
            .OrderByDescending(r => r.Priority)
            .ToList();

        if (!responses.Any())
            return GetDefaultResponse(npc, personality);

        var response = responses.First();

        // Modifiera baserat på personlighet och relation
        var text = response.Text;
        var mood = personality.GetMoodModifier(relationship);

        if (!string.IsNullOrEmpty(mood))
            text = $"{npc.Name} says {mood}: \"{text}\"";
        else
            text = $"{npc.Name} says: \"{text}\"";

        // Spara i minnet att vi pratade om detta
        memory.Remember($"talked_about_{topic}", DateTime.Now, MemoryType.ShortTerm);

        return text;
    }

    private static string GetDefaultResponse(INpc npc, NpcPersonality personality)
    {
        if (personality.IsTalkative)
            return $"{npc.Name} seems eager to chat but doesn't know about that.";
        if (personality.IsReserved)
            return $"{npc.Name} shrugs silently.";
        return $"{npc.Name} doesn't seem to know about that.";
    }
}
```

### Task 56.5: Topic-Based Conversation

```csharp
public class TopicConversation
{
    private readonly Dictionary<string, TopicNode> _topics = [];

    public void AddTopic(string id, string question, string response, params string[] unlocks)
    {
        _topics[id] = new TopicNode(id, question, response, unlocks.ToList());
    }

    public IEnumerable<string> GetAvailableTopics(INpc npc, IGameState state)
    {
        return _topics.Values
            .Where(t => !npc.Memory.Knows($"discussed_{t.Id}"))
            .Where(t => t.Prerequisites.All(p => npc.Memory.Knows($"discussed_{p}") || state.HasFlag(p)))
            .Select(t => t.Question);
    }

    public string Discuss(string topicId, INpc npc, IGameState state)
    {
        if (!_topics.TryGetValue(topicId, out var topic))
            return "That's not something we can discuss.";

        npc.Memory.Remember($"discussed_{topicId}", true, MemoryType.LongTerm);

        // Unlock new topics
        foreach (var unlock in topic.Unlocks)
        {
            state.SetFlag($"topic_unlocked_{unlock}");
        }

        return topic.Response;
    }
}

public record TopicNode(
    string Id,
    string Question,
    string Response,
    List<string> Unlocks,
    List<string> Prerequisites = null
)
{
    public List<string> Prerequisites { get; } = Prerequisites ?? [];
}
```

### Task 56.6: Relationship Evolution

```csharp
public class RelationshipSystem
{
    public void ModifyRelationship(IGameState state, string npcId, int change, string reason)
    {
        var current = state.GetRelationship(npcId);
        var newValue = Math.Clamp(current + change, -100, 100);
        state.SetRelationship(npcId, newValue);

        // Spara varför relationen ändrades
        var npc = state.GetNpc(npcId);
        npc?.Memory.Remember($"relationship_change_{DateTime.Now.Ticks}",
            new { Change = change, Reason = reason },
            MemoryType.Emotional);

        // Trigger dialog baserat på relationship milestones
        if (current < 50 && newValue >= 50)
            state.RaiseEvent("relationship_friendly", new { NpcId = npcId });
        if (current >= 0 && newValue < 0)
            state.RaiseEvent("relationship_hostile", new { NpcId = npcId });
    }
}
```

### Task 56.7: AskCommand med topic-stöd

```csharp
public class AskCommand(string npcName, string topic) : ICommand
{
    public CommandResult Execute(CommandContext context)
    {
        var npc = context.State.CurrentLocation.Npcs
            .FirstOrDefault(n => n.Matches(npcName));

        if (npc == null)
            return CommandResult.Fail($"There's no {npcName} here.");

        var dialogSystem = new ContextualDialogSystem();
        var response = dialogSystem.GenerateResponse(npc, topic,
            new DialogContext(context.State));

        return CommandResult.Ok(response);
    }
}

// Parser: "ask guard about key" → AskCommand("guard", "key")
```

### Task 56.8: Tester

```csharp
[Fact]
public void NpcMemory_RemembersAndRecalls()
{
    var npc = CreateNpc();

    npc.Memory.Remember("player_helped", true, MemoryType.LongTerm);

    Assert.True(npc.Memory.Knows("player_helped"));
    Assert.True(npc.Memory.Recall<bool>("player_helped"));
}

[Fact]
public void Personality_AffectsResponse()
{
    var friendly = CreateNpc(personality: new NpcPersonality { Agreeableness = 0.9f });
    var hostile = CreateNpc(personality: new NpcPersonality { Agreeableness = 0.1f });

    var friendlyResponse = new ContextualDialogSystem().GenerateResponse(friendly, "help", context);
    var hostileResponse = new ContextualDialogSystem().GenerateResponse(hostile, "help", context);

    Assert.Contains("warmly", friendlyResponse);
    Assert.Contains("coldly", hostileResponse);
}
```

### Task 56.9: Sandbox — byn med personligheter

Demo med NPCs som har olika personligheter, minns tidigare samtal och vars relationer utvecklas.

---
