# 📚 Player History System - Debug & Storytelling Engine

## Dual-Purpose History System för TAF

### Core Concept: History as Both Debug Tool AND Story Engine
```csharp
// Inte bara logging - intelligent story reconstruction!
var historyEngine = new PlayerHistoryEngine()
    .EnableDebugMode(true)
    .EnableStorytellingMode(true)
    .WithRetentionPolicy(RetentionPolicy.GameSession)
    .WithAnalyticsLevel(AnalyticsLevel.Detailed);
```

## 🔍 Debug-Focused History

### Comprehensive Action Tracking
```csharp
public class DebugHistoryEntry
{
    public DateTime Timestamp { get; set; }
    public string PlayerId { get; set; }
    public string SessionId { get; set; }

    // Command analysis
    public string RawInput { get; set; }
    public ParsedCommand ParsedCommand { get; set; }
    public ActionResult ExecutionResult { get; set; }
    public TimeSpan ExecutionTime { get; set; }

    // World state before/after
    public WorldSnapshot StateBefore { get; set; }
    public WorldSnapshot StateAfter { get; set; }
    public List<WorldChange> Changes { get; set; } = new();

    // Context information
    public Room CurrentRoom { get; set; }
    public List<GameObject> VisibleObjects { get; set; } = new();
    public List<NPC> PresentNPCs { get; set; } = new();
    public PlayerStats PlayerStats { get; set; }

    // Error tracking
    public List<ParseError> ParseErrors { get; set; } = new();
    public List<ValidationError> ValidationErrors { get; set; } = new();
    public Exception Exception { get; set; }

    // Performance metrics
    public Dictionary<string, TimeSpan> SubsystemTimings { get; set; } = new();
    public long MemoryUsage { get; set; }
}

// Enhanced tracking för spatial actions
public class SpatialActionHistoryEntry : DebugHistoryEntry
{
    public Direction ActionDirection { get; set; }
    public Room TargetRoom { get; set; }
    public Pathway UsedPathway { get; set; }
    public TrajectoryData Trajectory { get; set; }
    public List<ImpactResult> Impacts { get; set; } = new();

    // Advanced spatial debugging
    public SightlineData SightlineInfo { get; set; }
    public SoundPropagationData SoundData { get; set; }
    public ProjectilePhysicsData PhysicsData { get; set; }
}
```

### Real-Time Debug Dashboard
```csharp
public class DebugDashboard
{
    private readonly PlayerHistoryEngine _history;
    private readonly ISignalRHub _hub;

    public async Task BroadcastPlayerAction(DebugHistoryEntry entry)
    {
        var debugInfo = new
        {
            Timestamp = entry.Timestamp,
            Command = entry.RawInput,
            ParseSuccess = entry.ParsedCommand != null,
            ExecutionTime = entry.ExecutionTime.TotalMilliseconds,

            // Spatial debug info
            CurrentRoom = entry.CurrentRoom?.Name,
            SpatialAction = entry is SpatialActionHistoryEntry spatial
                ? $"{spatial.ParsedCommand?.Verb} {spatial.ActionDirection}"
                : null,

            // Performance warnings
            PerformanceWarnings = GetPerformanceWarnings(entry),

            // Error details
            Errors = entry.ParseErrors.Concat(entry.ValidationErrors).ToList()
        };

        await _hub.Clients.Group("Developers").SendAsync("PlayerAction", debugInfo);
    }

    public List<string> GetPerformanceWarnings(DebugHistoryEntry entry)
    {
        var warnings = new List<string>();

        if (entry.ExecutionTime.TotalMilliseconds > 100)
            warnings.Add($"Slow execution: {entry.ExecutionTime.TotalMilliseconds:F0}ms");

        if (entry.MemoryUsage > 10_000_000) // 10MB
            warnings.Add($"High memory usage: {entry.MemoryUsage / 1_000_000}MB");

        if (entry.SubsystemTimings.ContainsKey("PathwayResolution") &&
            entry.SubsystemTimings["PathwayResolution"].TotalMilliseconds > 50)
            warnings.Add("Pathway resolution bottleneck detected");

        return warnings;
    }
}
```

### Debug Query Engine
```csharp
public class HistoryQueryEngine
{
    public async Task<List<DebugHistoryEntry>> FindProblematicActions(QueryCriteria criteria)
    {
        return await _historyDb.QueryAsync(q => q
            .Where(entry => entry.ExecutionTime > criteria.MaxExecutionTime)
            .Or(entry => entry.ParseErrors.Any())
            .Or(entry => entry.Exception != null)
            .OrderByDescending(entry => entry.Timestamp)
            .Take(criteria.MaxResults));
    }

    public async Task<PlayerBehaviorAnalysis> AnalyzePlayerBehavior(string playerId, TimeSpan period)
    {
        var entries = await GetPlayerHistory(playerId, period);

        return new PlayerBehaviorAnalysis
        {
            // Command patterns
            MostUsedCommands = entries
                .GroupBy(e => e.ParsedCommand?.Verb)
                .OrderByDescending(g => g.Count())
                .Take(10)
                .ToDictionary(g => g.Key, g => g.Count()),

            // Spatial awareness usage
            DirectionalActionUsage = entries.OfType<SpatialActionHistoryEntry>()
                .GroupBy(e => e.ActionDirection)
                .ToDictionary(g => g.Key, g => g.Count()),

            // Error patterns
            CommonErrors = entries
                .SelectMany(e => e.ParseErrors)
                .GroupBy(error => error.Type)
                .OrderByDescending(g => g.Count())
                .ToDictionary(g => g.Key, g => g.Count()),

            // Performance patterns
            AverageExecutionTime = entries.Average(e => e.ExecutionTime.TotalMilliseconds),
            SlowestActions = entries
                .OrderByDescending(e => e.ExecutionTime)
                .Take(5)
                .Select(e => new { e.RawInput, e.ExecutionTime })
                .ToList()
        };
    }

    // Spatial-specific debugging queries
    public async Task<List<SpatialIssue>> FindSpatialActionIssues(TimeSpan period)
    {
        var spatialEntries = await _historyDb.QueryAsync<SpatialActionHistoryEntry>(q => q
            .Where(entry => entry.Timestamp > DateTime.UtcNow - period)
            .Where(entry => entry.Trajectory != null));

        var issues = new List<SpatialIssue>();

        // Find trajectory calculation problems
        issues.AddRange(spatialEntries
            .Where(e => e.SubsystemTimings.ContainsKey("TrajectoryCalculation") &&
                       e.SubsystemTimings["TrajectoryCalculation"].TotalMilliseconds > 25)
            .Select(e => new SpatialIssue
            {
                Type = SpatialIssueType.SlowTrajectoryCalculation,
                Entry = e,
                Description = $"Trajectory calculation took {e.SubsystemTimings["TrajectoryCalculation"].TotalMilliseconds:F0}ms"
            }));

        // Find pathway resolution failures
        issues.AddRange(spatialEntries
            .Where(e => e.UsedPathway == null && e.ParsedCommand?.Direction != null)
            .Select(e => new SpatialIssue
            {
                Type = SpatialIssueType.PathwayResolutionFailure,
                Entry = e,
                Description = $"Failed to resolve pathway for direction {e.ParsedCommand.Direction}"
            }));

        return issues;
    }
}
```

## 📖 Storytelling-Focused History

### Narrative Timeline Construction
```csharp
public class StoryTimelineBuilder
{
    public PlayerStory BuildStory(List<DebugHistoryEntry> rawHistory)
    {
        var story = new PlayerStory();

        // Filter ut debug noise - fokus på story-relevanta actions
        var narrativeActions = rawHistory
            .Where(IsNarrativelySignificant)
            .OrderBy(e => e.Timestamp)
            .ToList();

        // Group actions into story beats
        var storyBeats = GroupIntoStoryBeats(narrativeActions);

        foreach (var beat in storyBeats)
        {
            var chapter = BuildChapter(beat);
            story.AddChapter(chapter);
        }

        return story;
    }

    private bool IsNarrativelySignificant(DebugHistoryEntry entry)
    {
        // Skip pure debug commands
        if (entry.RawInput.StartsWith("debug") || entry.RawInput.StartsWith("help"))
            return false;

        // Include important game actions
        if (entry.ParsedCommand?.Verb.In("go", "take", "use", "talk", "fight"))
            return true;

        // Include all spatial actions - de är per definition intressanta!
        if (entry is SpatialActionHistoryEntry)
            return true;

        // Include world-changing actions
        if (entry.Changes?.Any(c => c.Type.In(ChangeType.RoomEntered, ChangeType.ItemAcquired, ChangeType.NPCDefeated)) == true)
            return true;

        return false;
    }

    private StoryChapter BuildChapter(List<DebugHistoryEntry> beatActions)
    {
        var chapter = new StoryChapter
        {
            Title = GenerateChapterTitle(beatActions),
            StartTime = beatActions.First().Timestamp,
            EndTime = beatActions.Last().Timestamp
        };

        foreach (var action in beatActions)
        {
            var narrative = ConvertToNarrative(action);
            if (narrative != null)
                chapter.AddMoment(narrative);
        }

        return chapter;
    }

    private NarrativeMoment ConvertToNarrative(DebugHistoryEntry entry)
    {
        // Spatial actions få special treatment
        if (entry is SpatialActionHistoryEntry spatial)
        {
            return new SpatialNarrativeMoment
            {
                Timestamp = entry.Timestamp,
                Action = $"{spatial.ParsedCommand.Verb} {spatial.ParsedCommand.Object} {spatial.ActionDirection}",
                Room = spatial.CurrentRoom.Name,
                Direction = spatial.ActionDirection,
                Outcome = DetermineOutcome(spatial),
                Significance = CalculateSignificance(spatial),

                // Rich narrative details
                NarrativeText = GenerateNarrativeText(spatial),
                EmotionalTone = DetermineEmotionalTone(spatial),
                StrategicValue = AnalyzeStrategicValue(spatial)
            };
        }

        return new StandardNarrativeMoment
        {
            Timestamp = entry.Timestamp,
            Action = entry.ParsedCommand?.Verb ?? "unknown",
            Room = entry.CurrentRoom?.Name,
            Outcome = entry.ExecutionResult?.Success == true ? "success" : "failure",
            NarrativeText = GenerateStandardNarrative(entry)
        };
    }
}
```

### Emotional Journey Tracking
```csharp
public class EmotionalJourneyAnalyzer
{
    public EmotionalProfile AnalyzeEmotionalJourney(PlayerStory story)
    {
        var profile = new EmotionalProfile();

        foreach (var chapter in story.Chapters)
        {
            var chapterEmotion = AnalyzeChapterEmotion(chapter);
            profile.AddEmotionalArc(chapterEmotion);
        }

        // Special analysis för spatial actions
        var spatialMoments = story.GetMoments<SpatialNarrativeMoment>();
        profile.SpatialEngagement = AnalyzeSpatialEngagement(spatialMoments);

        return profile;
    }

    private EmotionalState AnalyzeChapterEmotion(StoryChapter chapter)
    {
        var emotionWeights = new Dictionary<EmotionType, float>();

        foreach (var moment in chapter.Moments)
        {
            if (moment is SpatialNarrativeMoment spatial)
            {
                // Spatial actions ofta medför excitement eller tension
                switch (spatial.Action.Split(' ')[0].ToLower())
                {
                    case "throw":
                        emotionWeights[EmotionType.Excitement] += 0.8f;
                        emotionWeights[EmotionType.Tension] += 0.6f;
                        break;
                    case "fire":
                    case "shoot":
                        emotionWeights[EmotionType.Adrenaline] += 1.0f;
                        emotionWeights[EmotionType.Tension] += 0.9f;
                        break;
                    case "shout":
                        emotionWeights[EmotionType.Desperation] += 0.7f;
                        break;
                    case "look":
                        emotionWeights[EmotionType.Curiosity] += 0.5f;
                        break;
                }

                // Outcome påverkar emotion
                if (spatial.Outcome.Contains("success"))
                    emotionWeights[EmotionType.Satisfaction] += 0.6f;
                else
                    emotionWeights[EmotionType.Frustration] += 0.4f;
            }
        }

        return new EmotionalState
        {
            PrimaryEmotion = emotionWeights.OrderByDescending(kvp => kvp.Value).First().Key,
            EmotionIntensity = emotionWeights.Values.Max(),
            EmotionMix = emotionWeights
        };
    }
}
```

### Adaptive Storytelling
```csharp
public class AdaptiveNarrator
{
    public string GenerateContextualNarrative(NarrativeMoment moment, EmotionalProfile playerProfile)
    {
        if (moment is SpatialNarrativeMoment spatial)
        {
            return GenerateSpatialNarrative(spatial, playerProfile);
        }

        return GenerateStandardNarrative(moment, playerProfile);
    }

    private string GenerateSpatialNarrative(SpatialNarrativeMoment spatial, EmotionalProfile profile)
    {
        var baseAction = spatial.Action.Split(' ')[0].ToLower();
        var direction = spatial.Direction.ToString().ToLower();
        var room = spatial.Room;

        // Adapt tone based på player's emotional journey
        var currentTension = profile.GetCurrentTensionLevel();

        return baseAction switch
        {
            "throw" when currentTension > 0.7f =>
                $"I desperation kastade du {spatial.ParsedCommand.Object} åt {direction}, hoppandes på att det skulle hjälpa.",

            "throw" when currentTension < 0.3f =>
                $"Du kastade lugnt {spatial.ParsedCommand.Object} åt {direction}, en välberäknad manöver.",

            "fire" when spatial.Outcome.Contains("hit") =>
                $"Skottet ekade genom {room} när du träffade målet åt {direction}. Din hand darrade av adrenalin.",

            "fire" when spatial.Outcome.Contains("miss") =>
                $"Skottet gick åt {direction} men missade. Röken från vapnet påminde dig om hur höga oddsen var.",

            "shout" when profile.GetRecentEmotions().Contains(EmotionType.Desperation) =>
                $"Ditt rop åt {direction} bar med sig all förtvivlan du kände. Någon måste höra dig.",

            "look" when profile.SpatialEngagement.CuriosityLevel > 0.8f =>
                $"Du studerade noggrant vad som låg åt {direction}, din nyfikenhet hade tagit överhand.",

            _ => $"Du {baseAction}ade åt {direction} från {room}."
        };
    }
}
```

## 💾 Data Storage & Performance

### Hierarchical Storage Strategy
```csharp
public class HistoryStorageManager
{
    private readonly IMemoryCache _recentHistory; // Senaste 1000 actions
    private readonly ISqliteDatabase _sessionDatabase; // Current game session
    private readonly ICloudStorage _longTermStorage; // Historical data

    public async Task<DebugHistoryEntry> StoreHistoryEntry(DebugHistoryEntry entry)
    {
        // Always store in memory för immediate debug access
        _recentHistory.Set(entry.Id, entry, TimeSpan.FromMinutes(30));

        // Store in session DB för current game analysis
        await _sessionDatabase.InsertAsync(entry);

        // Conditionally store in long-term för storytelling
        if (ShouldPreserveLongTerm(entry))
        {
            await _longTermStorage.StoreAsync(entry);
        }

        return entry;
    }

    private bool ShouldPreserveLongTerm(DebugHistoryEntry entry)
    {
        // Preserve all spatial actions - de är unique!
        if (entry is SpatialActionHistoryEntry)
            return true;

        // Preserve story-significant moments
        if (entry.Changes?.Any(c => c.Significance >= SignificanceLevel.High) == true)
            return true;

        // Preserve errors för learning
        if (entry.Exception != null || entry.ParseErrors.Any())
            return true;

        return false;
    }
}
```

### Efficient Querying
```csharp
public class OptimizedHistoryQueries
{
    public async Task<PlayerStory> GetPlayerStoryAsync(string playerId, TimeSpan period)
    {
        // Use memory cache för recent data
        var recentEntries = _recentHistory.GetRecentEntries(playerId, TimeSpan.FromHours(1));

        // Query database för older session data
        var sessionEntries = await _sessionDatabase.QueryAsync<DebugHistoryEntry>(q => q
            .Where(e => e.PlayerId == playerId)
            .Where(e => e.Timestamp > DateTime.UtcNow - period)
            .OrderBy(e => e.Timestamp));

        var allEntries = recentEntries.Concat(sessionEntries).ToList();

        return _storyBuilder.BuildStory(allEntries);
    }

    // Spatial-specific optimized queries
    public async Task<List<SpatialActionHistoryEntry>> GetSpatialActionsAsync(
        string playerId,
        Direction? direction = null,
        string verb = null)
    {
        var query = _sessionDatabase.Query<SpatialActionHistoryEntry>()
            .Where(e => e.PlayerId == playerId);

        if (direction.HasValue)
            query = query.Where(e => e.ActionDirection == direction.Value);

        if (!string.IsNullOrEmpty(verb))
            query = query.Where(e => e.ParsedCommand.Verb == verb);

        return await query.ToListAsync();
    }
}
```

## 🎯 Integration med TAF Systems

### Parser Integration
```csharp
public class HistoryAwareParser
{
    private readonly PlayerHistoryEngine _history;

    public ParsedCommand ParseWithHistory(string input, Player player)
    {
        var parseStart = DateTime.UtcNow;

        try
        {
            var command = _parser.Parse(input);

            // Log successful parse
            _history.RecordParseSuccess(player.Id, input, command, DateTime.UtcNow - parseStart);

            return command;
        }
        catch (ParseException ex)
        {
            // Log parse failure med detailed context
            _history.RecordParseFailure(player.Id, input, ex, GetParseContext(player));
            throw;
        }
    }

    private ParseContext GetParseContext(Player player)
    {
        return new ParseContext
        {
            CurrentRoom = player.CurrentRoom,
            VisibleObjects = player.CurrentRoom.GetVisibleObjects(),
            AvailableDirections = player.CurrentRoom.GetAvailableExits(),
            RecentCommands = _history.GetRecentCommands(player.Id, count: 5),
            PlayerVocabulary = _history.GetPlayerVocabularyProfile(player.Id)
        };
    }
}
```

### World State Tracking
```csharp
public class WorldStateTracker
{
    public void OnWorldStateChange(WorldChangeEvent changeEvent)
    {
        var historyEntry = new WorldStateChangeEntry
        {
            Timestamp = DateTime.UtcNow,
            ChangeType = changeEvent.Type,
            ChangedEntity = changeEvent.Entity,
            OldState = changeEvent.StateBefore,
            NewState = changeEvent.StateAfter,
            TriggeringAction = changeEvent.TriggeringAction,
            AffectedPlayers = GetAffectedPlayers(changeEvent)
        };

        // Track spatial state changes specifically
        if (changeEvent.Entity is Pathway pathway)
        {
            historyEntry.SpatialData = new SpatialStateChangeData
            {
                PathwayId = pathway.Id,
                FromRoom = pathway.FromRoom.Id,
                ToRoom = pathway.ToRoom.Id,
                Direction = pathway.Direction,
                StateChange = GetPathwayStateChange(changeEvent)
            };
        }

        _historyEngine.RecordWorldStateChange(historyEntry);
    }
}
```

## 🧪 Testing & Analytics

### Automated Testing with History
```csharp
[TestClass]
public class HistoryIntegratedTests
{
    [TestMethod]
    public async Task SpatialAction_RecordsDetailedHistory()
    {
        // Arrange
        var world = CreateTestWorld();
        var player = new Player();
        var historyEngine = new PlayerHistoryEngine();

        // Act
        await player.ExecuteCommand("throw knife north");

        // Assert
        var history = await historyEngine.GetPlayerHistory(player.Id, TimeSpan.FromMinutes(1));
        var spatialEntry = history.OfType<SpatialActionHistoryEntry>().First();

        Assert.AreEqual("throw", spatialEntry.ParsedCommand.Verb);
        Assert.AreEqual("knife", spatialEntry.ParsedCommand.Object);
        Assert.AreEqual(Direction.North, spatialEntry.ActionDirection);
        Assert.IsNotNull(spatialEntry.Trajectory);
        Assert.IsTrue(spatialEntry.ExecutionTime.TotalMilliseconds > 0);
    }

    [TestMethod]
    public async Task PlayerBehavior_AnalyzesCorrectly()
    {
        // Arrange
        var historyData = LoadTestHistoryData("spatial_power_user.json");
        var analyzer = new PlayerBehaviorAnalyzer();

        // Act
        var analysis = await analyzer.AnalyzePlayerBehavior(historyData);

        // Assert
        Assert.IsTrue(analysis.SpatialEngagement.DirectionalActionUsage > 0.7f);
        Assert.IsTrue(analysis.MostUsedCommands.ContainsKey("throw"));
        Assert.IsTrue(analysis.DirectionalActionUsage.ContainsKey(Direction.North));
    }
}
```

### Real-Time Analytics Dashboard
```csharp
public class HistoryAnalyticsDashboard
{
    public async Task<DashboardData> GetRealTimeAnalytics()
    {
        var last24Hours = TimeSpan.FromHours(24);

        return new DashboardData
        {
            // General metrics
            ActivePlayers = await _history.GetActivePlayerCount(last24Hours),
            TotalActions = await _history.GetActionCount(last24Hours),
            AverageSessionLength = await _history.GetAverageSessionLength(last24Hours),

            // Spatial awareness metrics
            SpatialActionUsage = await _history.GetSpatialActionMetrics(last24Hours),
            MostPopularDirections = await _history.GetDirectionUsageStats(last24Hours),
            SpatialSuccessRate = await _history.GetSpatialSuccessRate(last24Hours),

            // Debug metrics
            ErrorRate = await _history.GetErrorRate(last24Hours),
            PerformanceMetrics = await _history.GetPerformanceMetrics(last24Hours),
            CommonIssues = await _history.GetCommonIssues(last24Hours),

            // Storytelling metrics
            StoryEngagement = await _history.GetStoryEngagementMetrics(last24Hours),
            NarrativeComplexity = await _history.GetNarrativeComplexityMetrics(last24Hours)
        };
    }
}
```

---

## 🚀 Benefits av Player History System

### För Utvecklare:
✅ **Deep Debug Insights** - förstå exactly vad som händer under the hood
✅ **Performance Monitoring** - hitta bottlenecks i real-time
✅ **User Behavior Analysis** - optimera game systems baserat på real usage
✅ **Error Pattern Detection** - proactively fix common issues

### För Spelare:
✅ **Personal Story Generation** - se din adventure som en riktig berättelse
✅ **Achievement System Ready** - track epic spatial action moments
✅ **Progress Visualization** - förstå din journey through the world
✅ **Replay Value** - relive great moments och decisions

### För Game Designers:
✅ **Content Effectiveness** - vilka spatial actions används mest?
✅ **Difficulty Balancing** - where do players struggle?
✅ **Engagement Metrics** - what keeps players invested?
✅ **Narrative Impact** - do our stories resonate emotionally?

**Player History blir the foundation för data-driven game improvement OCH personalized storytelling! 📚🚀**

Detta system gör TAF both developer-friendly OCH player-centric genom att capture every moment av the adventure journey!