# ⚡ Event System & Dynamic Economics - Living World Reactions

## Fable-Style Supply & Demand + Event-Driven Architecture

### Dynamic Pricing System (Fable-Inspired)
```csharp
// Inte bara static prices - supply & demand like Fable!
public class DynamicPricingEngine
{
    public float CalculatePrice(string itemId, Store store, MarketConditions market)
    {
        var item = store.GetItem(itemId);
        var basePrice = item.BasePrice;

        // Supply affects price dramatically
        var supplyModifier = CalculateSupplyModifier(item.Quantity);

        // Demand based on recent purchases
        var demandModifier = CalculateDemandModifier(itemId, market);

        // Regional factors
        var regionalModifier = CalculateRegionalModifier(store.Location, itemId);

        // Event-driven factors
        var eventModifier = CalculateEventModifier(itemId, market.RecentEvents);

        var finalPrice = basePrice * supplyModifier * demandModifier * regionalModifier * eventModifier;

        return Math.Max(finalPrice, basePrice * 0.1f); // Never below 10% of base
    }

    private float CalculateSupplyModifier(int quantity)
    {
        // Fable-style dramatic price changes
        return quantity switch
        {
            0 => 0.0f,          // Can't buy what doesn't exist
            1 => 5.0f,          // Last item = 500% price!
            2 => 3.0f,          // Second to last = 300%
            <= 5 => 2.0f,       // Very low stock = 200%
            <= 10 => 1.5f,      // Low stock = 150%
            <= 50 => 1.0f,      // Normal stock = base price
            <= 100 => 0.8f,     // Good stock = 80%
            <= 500 => 0.5f,     // High stock = 50%
            _ => 0.3f           // Oversupply = 30% (fire sale!)
        };
    }

    private float CalculateDemandModifier(string itemId, MarketConditions market)
    {
        var recentSales = market.GetRecentSales(itemId, TimeSpan.FromDays(7));
        var averageSales = market.GetAverageSales(itemId, TimeSpan.FromDays(30));

        if (recentSales > averageSales * 2)
            return 1.4f; // High demand = +40%

        if (recentSales < averageSales * 0.5f)
            return 0.7f; // Low demand = -30%

        return 1.0f; // Normal demand
    }

    private float CalculateEventModifier(string itemId, List<GameEvent> recentEvents)
    {
        float modifier = 1.0f;

        foreach (var gameEvent in recentEvents)
        {
            modifier *= gameEvent.GetPriceModifierFor(itemId);
        }

        return modifier;
    }
}

// Example: Dragon attack increases armor prices
public class DragonAttackEvent : GameEvent
{
    public override float GetPriceModifierFor(string itemId)
    {
        return itemId switch
        {
            "dragon_scale_armor" => 0.5f,  // Dragon dead = cheaper dragon gear
            "fire_resistance_potion" => 3.0f, // Still high demand for fire protection
            "healing_potion" => 1.8f,     // Injuries from battle
            "weapons" => 1.2f,            // Cleanup operations need weapons
            _ => 1.0f
        };
    }
}
```

## ⚡ Comprehensive Event System

### Core Event Architecture
```csharp
public class TAFEventSystem
{
    private readonly Dictionary<string, List<IEventHandler>> _eventHandlers = new();
    private readonly EventQueue _eventQueue = new();
    private readonly EventHistory _eventHistory = new();

    // Core event types
    public event Action<int> OnStepsTaken;
    public event Action<Door> OnDoorOpened;
    public event Action<Door> OnDoorLocked;
    public event Action<GameObject> OnItemPickedUp;
    public event Action<GameObject> OnItemDropped;
    public event Action<NPC> OnNPCDied;
    public event Action<Player, Room> OnRoomEntered;
    public event Action<Weapon> OnWeaponUsed;
    public event Action<Store, StoreTransaction> OnStorePurchase;

    public void RegisterEventHandler<T>(string eventType, IEventHandler<T> handler) where T : GameEvent
    {
        if (!_eventHandlers.ContainsKey(eventType))
            _eventHandlers[eventType] = new List<IEventHandler>();

        _eventHandlers[eventType].Add(handler);
    }

    public void TriggerEvent<T>(T gameEvent) where T : GameEvent
    {
        var eventType = typeof(T).Name;

        // Add to history
        _eventHistory.Record(gameEvent);

        // Process immediate handlers
        if (_eventHandlers.ContainsKey(eventType))
        {
            foreach (var handler in _eventHandlers[eventType])
            {
                if (handler is IEventHandler<T> typedHandler)
                {
                    typedHandler.Handle(gameEvent);
                }
            }
        }

        // Check for step-based triggers
        ProcessStepBasedTriggers(gameEvent);

        // Check for conditional triggers
        ProcessConditionalTriggers(gameEvent);
    }

    private void ProcessStepBasedTriggers(GameEvent gameEvent)
    {
        if (gameEvent is StepsTakenEvent stepEvent)
        {
            var totalSteps = GameWorld.Instance.Player.TotalSteps;

            // Every 100 steps
            if (totalSteps % 100 == 0)
            {
                TriggerEvent(new HundredStepsEvent { StepCount = totalSteps });
            }

            // Every 60 steps
            if (totalSteps % 60 == 0)
            {
                TriggerEvent(new SixtyStepsEvent { StepCount = totalSteps });
            }

            // Custom intervals
            CheckCustomStepTriggers(totalSteps);
        }
    }
}

// Core Event Types
public abstract class GameEvent
{
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public string EventId { get; set; } = Guid.NewGuid().ToString();
    public EventPriority Priority { get; set; } = EventPriority.Normal;

    public virtual float GetPriceModifierFor(string itemId) => 1.0f;
}

public class StepsTakenEvent : GameEvent
{
    public int StepCount { get; set; }
    public Player Player { get; set; }
    public Room FromRoom { get; set; }
    public Room ToRoom { get; set; }
    public Direction Direction { get; set; }
}

public class DoorEvent : GameEvent
{
    public Door Door { get; set; }
    public Player Player { get; set; }
    public Room Room { get; set; }
}

public class DoorOpenedEvent : DoorEvent { }
public class DoorLockedEvent : DoorEvent { }
public class DoorUnlockedEvent : DoorEvent { }

public class ItemEvent : GameEvent
{
    public GameObject Item { get; set; }
    public Player Player { get; set; }
    public Room Room { get; set; }
}

public class ItemPickedUpEvent : ItemEvent { }
public class ItemDroppedEvent : ItemEvent { }
public class ItemUsedEvent : ItemEvent
{
    public string UseContext { get; set; } // "combat", "puzzle", "crafting"
}

public class NPCEvent : GameEvent
{
    public NPC NPC { get; set; }
    public Player Player { get; set; }
    public Room Room { get; set; }
}

public class NPCDiedEvent : NPCEvent
{
    public string CauseOfDeath { get; set; }
    public bool PlayerCaused { get; set; }
}

public class NPCMetEvent : NPCEvent { }
public class NPCAttackedEvent : NPCEvent { }
```

### Smart Step-Based Triggers
```csharp
public class StepBasedTriggerSystem
{
    private readonly Dictionary<int, List<StepTrigger>> _stepTriggers = new();
    private readonly List<IntervalTrigger> _intervalTriggers = new();

    public void RegisterStepTrigger(int stepCount, StepTrigger trigger)
    {
        if (!_stepTriggers.ContainsKey(stepCount))
            _stepTriggers[stepCount] = new List<StepTrigger>();

        _stepTriggers[stepCount].Add(trigger);
    }

    public void RegisterIntervalTrigger(int interval, IntervalTrigger trigger)
    {
        trigger.Interval = interval;
        _intervalTriggers.Add(trigger);
    }

    public void ProcessStepTriggers(int currentSteps)
    {
        // Exact step triggers
        if (_stepTriggers.ContainsKey(currentSteps))
        {
            foreach (var trigger in _stepTriggers[currentSteps])
            {
                trigger.Execute(currentSteps);
            }
        }

        // Interval triggers
        foreach (var trigger in _intervalTriggers)
        {
            if (currentSteps % trigger.Interval == 0)
            {
                trigger.Execute(currentSteps);
            }
        }
    }
}

public class StepTrigger
{
    public string Id { get; set; }
    public string Description { get; set; }
    public Action<int> Action { get; set; }
    public Func<GameContext, bool> Condition { get; set; } = _ => true;
    public bool OneTime { get; set; } = true;
    public bool HasTriggered { get; set; } = false;

    public void Execute(int stepCount)
    {
        if (OneTime && HasTriggered) return;
        if (!Condition(GameWorld.Instance.Context)) return;

        Action?.Invoke(stepCount);
        HasTriggered = true;
    }
}

// Example step triggers
var stepTriggers = new List<StepTrigger>
{
    new()
    {
        Id = "spider_chase_start",
        Description = "Mutant spider starts chasing after 50 steps in cave",
        Action = steps => GameWorld.Instance.StartNPCChase("mutant_spider", "player"),
        Condition = ctx => ctx.Player.CurrentRoom.Id.Contains("cave")
    },

    new()
    {
        Id = "gate_closes",
        Description = "City gate closes after 200 steps (sunset)",
        Action = steps => GameWorld.Instance.GetDoor("city_gate").Lock(),
        Condition = ctx => GameWorld.Instance.TimeOfDay > TimeSpan.FromHours(18)
    },

    new()
    {
        Id = "merchant_restocks",
        Description = "Merchants restock every 500 steps",
        OneTime = false,
        Action = steps => GameWorld.Instance.RestockAllStores()
    }
};
```

### Object-Specific Event Handlers
```csharp
public class GameObject
{
    // Event handlers på object level
    public event Action<GameObject, Player> OnPickedUp;
    public event Action<GameObject, Player> OnDropped;
    public event Action<GameObject, Player> OnUsed;
    public event Action<GameObject, Player> OnExamined;

    // Trigger events
    public void TriggerPickedUp(Player player)
    {
        OnPickedUp?.Invoke(this, player);

        // Global event
        GameWorld.Instance.EventSystem.TriggerEvent(new ItemPickedUpEvent
        {
            Item = this,
            Player = player,
            Room = player.CurrentRoom
        });
    }
}

public class Door : GameObject
{
    public event Action<Door, Player> OnOpened;
    public event Action<Door, Player> OnClosed;
    public event Action<Door, Player> OnLocked;
    public event Action<Door, Player> OnUnlocked;

    public void Open(Player player)
    {
        if (IsLocked)
        {
            player.Output("Dörren är låst.");
            return;
        }

        IsOpen = true;
        OnOpened?.Invoke(this, player);

        // Global event
        GameWorld.Instance.EventSystem.TriggerEvent(new DoorOpenedEvent
        {
            Door = this,
            Player = player,
            Room = player.CurrentRoom
        });
    }
}

public class NPC : GameObject
{
    public event Action<NPC, Player> OnDied;
    public event Action<NPC, Player> OnMet;
    public event Action<NPC, Player> OnAttacked;
    public event Action<NPC, Player> OnTalkedTo;

    public void Die(Player killer = null)
    {
        IsAlive = false;
        OnDied?.Invoke(this, killer);

        // Global event med consequences
        GameWorld.Instance.EventSystem.TriggerEvent(new NPCDiedEvent
        {
            NPC = this,
            Player = killer,
            Room = CurrentRoom,
            PlayerCaused = killer != null,
            CauseOfDeath = DetermineCauseOfDeath(killer)
        });
    }
}
```

## 🎮 Dynamic Event-Driven Economics

### Market Events System
```csharp
public class MarketEventSystem
{
    public void ProcessMarketEvents(List<GameEvent> recentEvents)
    {
        foreach (var gameEvent in recentEvents)
        {
            ProcessEventEconomicImpact(gameEvent);
        }
    }

    private void ProcessEventEconomicImpact(GameEvent gameEvent)
    {
        switch (gameEvent)
        {
            case NPCDiedEvent npcEvent when npcEvent.NPC is StoreOwner:
                ProcessMerchantDeath(npcEvent);
                break;

            case ItemPickedUpEvent itemEvent when itemEvent.Item.IsRare:
                ProcessRareItemAcquisition(itemEvent);
                break;

            case DoorOpenedEvent doorEvent when doorEvent.Door.IsSecretDoor:
                ProcessSecretAreaDiscovery(doorEvent);
                break;

            case HundredStepsEvent stepEvent:
                ProcessTimeBasedMarketChanges(stepEvent);
                break;
        }
    }

    private void ProcessMerchantDeath(NPCDiedEvent npcEvent)
    {
        var deadMerchant = npcEvent.NPC as StoreOwner;
        var store = deadMerchant.Store;

        // Immediate price increases för related items
        var relatedStores = GetRelatedStores(store);
        foreach (var relatedStore in relatedStores)
        {
            // Scarcity drives up prices
            relatedStore.ApplyPriceMultiplier(1.5f, reason: "supply_shortage");

            // Reduce restock rate
            relatedStore.RestockRate *= 0.7f;
        }

        // Create black market opportunity
        if (GameWorld.Instance.CrimeRate > 0.6f)
        {
            CreateBlackMarketVendor(store.Type, store.Location);
        }

        // Regional economic depression
        var region = store.Location.Region;
        region.EconomicHealth -= 0.2f;
    }

    private void ProcessTimeBasedMarketChanges(HundredStepsEvent stepEvent)
    {
        // Simulate market fluctuations över time
        var allStores = GameWorld.Instance.GetAllStores();

        foreach (var store in allStores)
        {
            // Random market fluctuations
            ApplyRandomMarketForces(store);

            // Seasonal changes
            ApplySeasonalChanges(store);

            // Supply chain updates
            ProcessSupplyChainEvents(store);
        }
    }

    private void ApplyRandomMarketForces(Store store)
    {
        var random = Random.Shared;

        // 10% chance of significant price change
        if (random.NextDouble() < 0.1)
        {
            var priceMultiplier = random.NextDouble() * 0.6 + 0.7; // 0.7 to 1.3
            var affectedCategory = store.GetRandomItemCategory();

            store.ApplyCategoryPriceMultiplier(affectedCategory, priceMultiplier);

            // Notify player if they're in store
            if (store.PlayerPresent)
            {
                var changeDescription = priceMultiplier > 1.0 ? "ökat" : "minskat";
                store.Owner.SayToPlayer($"Priserna på {affectedCategory} har {changeDescription} på grund av marknadsförändringar.");
            }
        }
    }
}
```

### Event-Driven Quest System
```csharp
public class EventDrivenQuestSystem
{
    private readonly Dictionary<Type, List<QuestTrigger>> _eventQuestTriggers = new();

    public void RegisterQuestTrigger<T>(QuestTrigger trigger) where T : GameEvent
    {
        var eventType = typeof(T);
        if (!_eventQuestTriggers.ContainsKey(eventType))
            _eventQuestTriggers[eventType] = new List<QuestTrigger>();

        _eventQuestTriggers[eventType].Add(trigger);
    }

    public void ProcessEventForQuests(GameEvent gameEvent)
    {
        var eventType = gameEvent.GetType();

        if (_eventQuestTriggers.ContainsKey(eventType))
        {
            foreach (var trigger in _eventQuestTriggers[eventType])
            {
                if (trigger.ShouldTrigger(gameEvent))
                {
                    trigger.ActivateQuest(gameEvent);
                }
            }
        }
    }
}

public class QuestTrigger
{
    public string QuestId { get; set; }
    public Func<GameEvent, bool> Condition { get; set; }
    public Action<GameEvent> ActivateQuest { get; set; }

    public bool ShouldTrigger(GameEvent gameEvent) => Condition?.Invoke(gameEvent) ?? true;
}

// Example: NPCDied event triggers revenge quest
var revengeQuestTrigger = new QuestTrigger
{
    QuestId = "avenge_merchant",
    Condition = evt => evt is NPCDiedEvent npcEvt &&
                      npcEvt.NPC is StoreOwner &&
                      npcEvt.PlayerCaused,
    ActivateQuest = evt =>
    {
        var npcEvent = evt as NPCDiedEvent;
        var deadMerchant = npcEvent.NPC as StoreOwner;

        // Merchant's son wants revenge
        var son = CreateRevengefulNPC(deadMerchant);
        var quest = new RevengeQuest
        {
            Target = npcEvent.Player,
            Avenger = son,
            Reason = $"You killed my father, {deadMerchant.Name}!"
        };

        GameWorld.Instance.QuestSystem.AddQuest(quest);
    }
};
```

## 🎯 Practical Event Examples

### The Chain Reaction Economy
```csharp
// Player kills blacksmith (NPCDiedEvent)
"kill blacksmith" →
Events Triggered:
- NPCDiedEvent → StoreClosedEvent → SupplyShortageEvent
- WeaponPricesIncreasedEvent (150% price increase)
- RegionalEconomicDeclineEvent (-20% prosperity)

// 100 steps later (HundredStepsEvent)
"take 100 steps" →
Events Triggered:
- MarketFluctuationEvent → BlackMarketSpawnEvent
- NPCFearEvent (other merchants hire guards)
- QuestActivatedEvent ("Find new weaponsmith")

// Player picks up rare dragon scale (ItemPickedUpEvent)
"take dragon scale" →
Events Triggered:
- RareItemAcquisitionEvent → MarketInterestEvent
- MultipleMerchantsInterestedEvent (price war)
- ThievesGuildNoticeEvent (target för theft)
```

### Step-Based Adventure Progression
```csharp
// Adventure timer system
RegisterStepTriggers(new[]
{
    (60, "warning_message", "Du hör distant roaring... något kommer närmare."),
    (120, "enemy_appears", "En drake flyger över huvudet!"),
    (180, "time_pressure", "Draken landar! Den har märkt dig!"),
    (240, "final_warning", "Draken attacking! Fly or fight!"),
    (300, "dragon_attack", "DRAGON BATTLE BEGINS!")
});

// Dynamic difficulty baserat på player progress
RegisterIntervalTrigger(100, () =>
{
    if (player.Level < ExpectedLevelForProgress())
    {
        SpawnHelpfulNPC();
        ReduceEnemyDifficulty();
    }
    else if (player.Level > ExpectedLevelForProgress())
    {
        SpawnBonusChallenge();
        IncreaseLootQuality();
    }
});
```

### Store Supply/Demand Simulation
```csharp
// Real-time market simulation
RegisterIntervalTrigger(50, () =>
{
    foreach (var store in allStores)
    {
        // Simulate customer purchases
        SimulateNPCCustomers(store);

        // Adjust prices baserat på inventory levels
        store.AdjustPricesBasedOnStock();

        // Restock från suppliers
        if (store.ShouldRestock())
        {
            ProcessRestockEvent(store);
        }
    }
});

// Dynamic events affect economy
OnEvent<DragonAttackEvent>(evt =>
{
    // Healing potions become expensive (high demand)
    AdjustItemPrice("healing_potion", 2.5f);

    // Weapons in demand för defense
    AdjustItemPrice("weapons", 1.8f);

    // Dragon materials become worthless (dragon dead)
    AdjustItemPrice("dragon_scale", 0.3f);
});
```

---

## 🚀 Benefits av Event-Driven Dynamic Economics

### För Spelare:
✅ **Reactive World** - every action triggers natural consequences
✅ **Economic Strategy** - timing purchases/sales för best prices
✅ **Dynamic Challenges** - world responds to player progress
✅ **Emergent Storytelling** - event chains create unique narratives

### För Game Designers:
✅ **Living Economy** - Fable-style supply/demand simulation
✅ **Automatic Balancing** - events adjust difficulty dynamically
✅ **Rich Consequences** - actions trigger complex reaction chains
✅ **Flexible Content** - events create content without manual scripting

### För Developers:
✅ **Clean Architecture** - event-driven design separates concerns
✅ **Modular System** - easy att add new event types och handlers
✅ **Performance Efficient** - events only trigger when needed
✅ **Testable Logic** - clear event input/output för testing

**Event System + Dynamic Economics = Living, Breathing World! ⚡💰**

Fable's supply/demand system PLUS comprehensive event architecture creates world där:
- Kill merchant → prices skyrocket → black markets spawn → revenge quests activate
- Walk 100 steps → dragons approach → weapon demand increases → merchants panic
- Find rare item → price wars begin → thieves target you → economic opportunities emerge

Every step, every action, every death triggers cascading consequences that reshape the world economy och adventure progression! 🚀✨