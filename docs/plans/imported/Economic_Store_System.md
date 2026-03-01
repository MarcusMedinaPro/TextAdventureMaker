# 💰 Economic & Store System - Living Economy with Consequences

## Revolutionary Concept: Actions Have Economic Consequences

### Moral Economic Choices
```csharp
// Inte bara "buy sword" - moral and economic consequences!
"rob blacksmith" → Få weapons gratis, men destroy local economy
"befriend merchant" → Få discounts, men kräver tid och relationship building
"protect store" → Earn reputation, få exclusive items
"undercut prices" → Start economic war med andra merchants
```

## 🏪 Dynamic Store System

### Living Store Ecosystem
```csharp
public class Store
{
    public string Id { get; set; }
    public string Name { get; set; }
    public StoreType Type { get; set; }
    public NPC Owner { get; set; }
    public StoreStatus Status { get; set; } = StoreStatus.Operating;

    // Economic health
    public float Prosperity { get; set; } = 1.0f; // 0.0 = bankrupt, 2.0 = thriving
    public int DailyCustomers { get; set; }
    public float Reputation { get; set; } = 0.5f; // 0.0 = terrible, 1.0 = excellent

    // Inventory management
    public Dictionary<string, StoreItem> Inventory { get; set; } = new();
    public float RestockRate { get; set; } = 1.0f; // Affected by owner status
    public List<string> PreferredSuppliers { get; set; } = new();

    // Owner relationship affects everything
    public OwnerStatus OwnerStatus { get; set; } = OwnerStatus.Alive;
    public RelationshipLevel PlayerRelationship { get; set; } = RelationshipLevel.Neutral;

    public StoreTransaction ProcessTransaction(Player player, TransactionType type, string itemId, int quantity = 1)
    {
        var transaction = new StoreTransaction
        {
            Player = player,
            Store = this,
            Type = type,
            Timestamp = DateTime.UtcNow
        };

        // Check store viability
        if (Status != StoreStatus.Operating)
        {
            return transaction.Fail($"{Name} är stängd. {GetClosureReason()}");
        }

        if (OwnerStatus != OwnerStatus.Alive)
        {
            return HandleOwnerlessStore(transaction, type, itemId, quantity);
        }

        return ProcessNormalTransaction(transaction, type, itemId, quantity);
    }

    private StoreTransaction HandleOwnerlessStore(StoreTransaction transaction, TransactionType type, string itemId, int quantity)
    {
        // Consequences av att ha dödat owner
        switch (type)
        {
            case TransactionType.Buy:
                return transaction.Fail("Det finns ingen att handla med. Ägaren är... borta.");

            case TransactionType.Sell:
                return transaction.Fail("Du har ingen att sälja till. Vad har du gjort?");

            case TransactionType.Steal:
                // Can loot dead store, but limited inventory
                var item = Inventory.GetValueOrDefault(itemId);
                if (item == null || item.Quantity <= 0)
                    return transaction.Fail($"Det finns ingen {itemId} kvar att stjäla.");

                // Success, but consequences
                transaction.Success = true;
                transaction.Message = $"Du stjäl {item.Name} från den övergivna butiken.";
                transaction.ReputationChange = -0.2f; // Society notices

                // Store degrades without owner
                Prosperity -= 0.1f;
                RestockRate = 0.0f; // No more restocking

                return transaction;

            default:
                return transaction.Fail("Butiken är övergiven och dyster.");
        }
    }
}

public enum StoreStatus
{
    Operating,      // Normal business
    Struggling,     // Low prosperity, limited inventory
    Closed,         // Temporarily closed (owner scared, sick, etc.)
    Abandoned,      // Permanently closed (owner dead/fled)
    UnderAttack,    // Being robbed right now
    Rebuilding      // Recovering from attack
}

public enum OwnerStatus
{
    Alive,          // Normal operations
    Injured,        // Reduced service, higher prices
    Scared,         // Store closed temporarily
    Dead,           // No restocking, looting only
    Fled            // Store abandoned, may return later
}
```

### Weapon Leveling & Upgrade System
```csharp
public class WeaponProgression
{
    public string WeaponId { get; set; }
    public int Level { get; set; } = 1;
    public int Experience { get; set; } = 0;
    public int ExperienceToNextLevel => Level * 100;

    public List<WeaponUpgrade> AvailableUpgrades { get; set; } = new();
    public List<WeaponUpgrade> AppliedUpgrades { get; set; } = new();

    // Weapons gain XP through use
    public void GainExperience(int amount, string source)
    {
        Experience += amount;

        if (Experience >= ExperienceToNextLevel)
        {
            LevelUp();
        }

        // Different sources give different XP
        var xpGained = source switch
        {
            "combat" => amount,
            "training" => amount * 0.7f,  // Training less effective
            "crafting" => amount * 1.2f,  // Crafting more effective
            _ => amount
        };
    }

    private void LevelUp()
    {
        Level++;
        Experience = 0;

        // Unlock new upgrade options
        UnlockUpgrades();
    }
}

public class WeaponUpgrade
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public UpgradeType Type { get; set; }

    // Cost and requirements
    public int GoldCost { get; set; }
    public List<string> RequiredMaterials { get; set; } = new();
    public int MinimumWeaponLevel { get; set; }
    public string RequiredCrafter { get; set; } // Specific NPC needed

    // Effects
    public Dictionary<string, float> StatModifiers { get; set; } = new();
    public List<string> SpecialAbilities { get; set; } = new();

    public UpgradeResult ApplyToWeapon(Weapon weapon, Store store)
    {
        var result = new UpgradeResult();

        // Check requirements
        if (!ValidateRequirements(weapon, store))
        {
            return result.Fail(GetRequirementFailureMessage(weapon, store));
        }

        // Apply upgrade
        ApplyStatModifiers(weapon);
        AddSpecialAbilities(weapon);

        // Update store relationship
        store.PlayerRelationship.AddPoints(0.1f); // Building trust
        store.Owner.GainExperience("crafting", 50);

        return result.Success($"{weapon.Name} har uppgraderats med {Name}!");
    }
}

public enum UpgradeType
{
    Sharpness,      // +damage
    Durability,     // +health, slower degradation
    Balance,        // +accuracy, +speed
    Enchantment,    // Magical effects
    Appearance,     // Cosmetic, +intimidation
    Special         // Unique weapon-specific upgrades
}
```

### Store Attendant Psychology & Secrets
```csharp
public class StoreOwner : NPC
{
    public StoreKnowledge Knowledge { get; set; } = new();
    public List<SecretDialogOption> SecretOptions { get; set; } = new();
    public BusinessPersonality BusinessStyle { get; set; } = new();

    // Secret knowledge unlocks with relationship
    public List<DialogOption> GetAvailableDialogOptions(Player player)
    {
        var options = base.GetBasicDialogOptions();

        // Add business-specific options
        options.AddRange(GetBusinessDialogOptions());

        // Add secret options based on relationship
        var relationship = RelationshipWithPlayer.Level;
        var trustLevel = RelationshipWithPlayer.TrustLevel;

        if (relationship >= RelationshipLevel.Friendly)
        {
            options.Add(new DialogOption
            {
                Id = "gossip",
                Text = "Vad händer i stan?",
                Response = GenerateGossip(),
                RequiredRelationship = RelationshipLevel.Friendly
            });
        }

        if (trustLevel >= TrustLevel.Confidant)
        {
            options.AddRange(GetSecretDialogOptions());
        }

        // Special options om player har räddat store
        if (Knowledge.PlayerSavedStore)
        {
            options.Add(new DialogOption
            {
                Id = "hero_discount",
                Text = "Kan du ge mig en rabatt?",
                Response = "Självklart! Du räddade min butik. 20% rabatt på allt!",
                Action = () => ApplyHeroDiscount(player)
            });
        }

        return options;
    }

    private List<DialogOption> GetSecretDialogOptions()
    {
        return SecretOptions.Where(secret => secret.IsUnlocked(this))
            .Select(secret => new DialogOption
            {
                Id = secret.Id,
                Text = secret.PromptText,
                Response = secret.Response,
                Action = secret.Action,
                RequiredTrust = secret.RequiredTrust
            }).ToList();
    }
}

public class SecretDialogOption
{
    public string Id { get; set; }
    public string PromptText { get; set; }
    public string Response { get; set; }
    public TrustLevel RequiredTrust { get; set; }
    public Action Action { get; set; }

    // Conditions för när secret unlocks
    public Func<StoreOwner, bool> UnlockCondition { get; set; }

    public bool IsUnlocked(StoreOwner owner) => UnlockCondition?.Invoke(owner) ?? true;
}

// Example secret dialog options
var blacksmithSecrets = new List<SecretDialogOption>
{
    new()
    {
        Id = "legendary_weapon_location",
        PromptText = "Finns det några speciella vapen?",
        RequiredTrust = TrustLevel.Confidant,
        Response = "Mellan oss sagt... Det finns ett legendariskt svärd gömt i Ancient Cave. Men var försiktig - draken vaktar det.",
        Action = () => player.Knowledge.Add("legendary_sword_location"),
        UnlockCondition = owner => owner.RelationshipWithPlayer.TrustLevel >= TrustLevel.Confidant
    },

    new()
    {
        Id = "competitor_weakness",
        PromptText = "Vad tycker du om andra smeder?",
        RequiredTrust = TrustLevel.Friend,
        Response = "Magnus på andra sidan stan? Han använder billig järn. Hans vapen går sönder efter några strider.",
        Action = () => player.Knowledge.Add("magnus_uses_cheap_materials"),
        UnlockCondition = owner => owner.BusinessStyle.Competitive && owner.Prosperity < 0.7f
    }
};
```

## 💀 Violence & Economic Consequences

### Robbing Stores - Immediate vs Long-term Consequences
```csharp
public class ViolentStoreInteraction
{
    public ViolentActionResult RobStore(Player player, Store store)
    {
        var result = new ViolentActionResult();

        // Immediate consequences
        var loot = CalculateLoot(store);
        var resistanceLevel = CalculateResistance(store.Owner);

        if (resistanceLevel > player.CombatSkill)
        {
            return result.Fail("Butiksägaren slår ifrån sig! Du blir utslängd!");
        }

        // Successful robbery
        player.AddItems(loot);
        result.Success = true;
        result.ImmediateGains = loot;

        // But trigger long-term consequences
        TriggerLongTermConsequences(player, store, result);

        return result;
    }

    private void TriggerLongTermConsequences(Player player, Store store, ViolentActionResult result)
    {
        // Store consequences
        store.OwnerStatus = DetermineOwnerFate(store.Owner, player.Violence);
        store.Status = StoreStatus.Abandoned;
        store.RestockRate = 0.0f;

        // Economic ripple effects
        var economicSystem = GameWorld.Instance.EconomicSystem;
        economicSystem.ProcessStoreDestruction(store);

        // Society consequences
        var reputationSystem = GameWorld.Instance.ReputationSystem;
        reputationSystem.RecordViolentCrime(player, "store_robbery", store);

        // Witness consequences
        var witnesses = store.CurrentRoom.GetNPCs().Where(npc => npc != store.Owner);
        foreach (var witness in witnesses)
        {
            witness.WitnessViolentCrime(player, "robbery");
            result.Witnesses.Add(witness);
        }

        // Future quest consequences
        var questSystem = GameWorld.Instance.QuestSystem;
        questSystem.BlockQuestsRequiringStore(store);
        questSystem.CreateRevengeQuests(store);
    }

    private OwnerStatus DetermineOwnerFate(NPC owner, float playerViolenceLevel)
    {
        return playerViolenceLevel switch
        {
            < 0.3f => OwnerStatus.Scared,    // Intimidation, owner hides
            < 0.7f => OwnerStatus.Injured,  // Beaten up, reduced capacity
            < 0.9f => OwnerStatus.Fled,     // Seriously hurt, flees town
            _ => OwnerStatus.Dead            // Killed, permanent consequences
        };
    }
}
```

### Economic Ecosystem Collapse
```csharp
public class EconomicSystem
{
    private readonly List<Store> _stores = new();
    private float _overallProsperity = 1.0f;

    public void ProcessStoreDestruction(Store destroyedStore)
    {
        // Direct effects på destroyed store
        destroyedStore.Status = StoreStatus.Abandoned;
        destroyedStore.RestockRate = 0.0f;

        // Ripple effects på andra stores
        ProcessEconomicRippleEffects(destroyedStore);

        // Create scarcity
        CreateItemScarcity(destroyedStore.Type);

        // Social consequences
        ReduceOverallSafety(destroyedStore);
    }

    private void ProcessEconomicRippleEffects(Store destroyedStore)
    {
        var relatedStores = _stores.Where(s => IsEconomicallyRelated(s, destroyedStore));

        foreach (var store in relatedStores)
        {
            if (store.Type == destroyedStore.Type)
            {
                // Competitors benefit from reduced competition
                store.Prosperity += 0.2f;
                store.DailyCustomers += destroyedStore.DailyCustomers * 0.6f;
            }
            else if (IsSupplierRelation(store, destroyedStore))
            {
                // Suppliers lose a customer
                store.Prosperity -= 0.15f;
                store.RestockRate -= 0.1f;
            }
            else if (IsCustomerRelation(store, destroyedStore))
            {
                // Customers lose a supplier
                store.Prosperity -= 0.1f;
                store.Inventory.ReduceQuality(0.1f); // Harder to get good materials
            }
        }
    }

    private void CreateItemScarcity(StoreType destroyedStoreType)
    {
        // Specific items become harder to find
        var affectedItems = GetItemsByStoreType(destroyedStoreType);

        foreach (var item in affectedItems)
        {
            // Increase prices across all remaining stores
            IncreaseItemPrices(item, 1.3f);

            // Reduce availability
            ReduceItemAvailability(item, 0.7f);
        }

        // Create black market opportunities
        if (_overallProsperity < 0.6f)
        {
            CreateBlackMarketVendors(affectedItems);
        }
    }

    private void ReduceOverallSafety(Store destroyedStore)
    {
        var town = destroyedStore.Location.Town;
        town.SafetyLevel -= 0.1f;
        town.CrimeRate += 0.15f;

        // Other NPCs become more cautious
        var allNPCs = town.GetAllNPCs();
        foreach (var npc in allNPCs)
        {
            npc.TrustInPlayer -= 0.1f;
            npc.Fearfulness += 0.05f;

            if (npc is StoreOwner owner)
            {
                owner.SecurityLevel += 0.2f; // Hire guards, better locks
                owner.PlayerRelationship.Suspicion += 0.15f;
            }
        }
    }
}
```

## 🛡️ Store Protection & Reputation Building

### Positive Economic Interactions
```csharp
public class ProtectiveStoreInteraction
{
    public ProtectionResult DefendStore(Player player, Store store, ThreatEvent threat)
    {
        var result = new ProtectionResult();

        // Combat resolution
        var combatResult = ResolveThreatCombat(player, threat);

        if (combatResult.Success)
        {
            // Immediate rewards
            store.Owner.RelationshipWithPlayer.AddPoints(0.5f);
            player.Reputation.AddPoints("hero", 0.3f);

            // Long-term benefits
            store.PlayerRelationship = RelationshipLevel.Ally;
            store.Knowledge.PlayerSavedStore = true;

            // Economic benefits
            UnlockHeroDiscounts(player, store);
            UnlockExclusiveItems(player, store);
            ProvideTrainingOpportunities(player, store);

            result.Success = true;
            result.ImmediateReward = combatResult.Loot;
            result.ReputationGain = 0.3f;
            result.LongTermBenefits = CalculateLongTermBenefits(store);

            return result;
        }

        return result.Fail("Du lyckades inte skydda butiken...");
    }

    private List<LongTermBenefit> CalculateLongTermBenefits(Store store)
    {
        return new List<LongTermBenefit>
        {
            new() { Type = "discount", Value = 0.2f, Description = "20% permanent discount" },
            new() { Type = "exclusive_access", Description = "Access to rare items" },
            new() { Type = "free_upgrades", Description = "Free weapon maintenance" },
            new() { Type = "information", Description = "Insider trading information" },
            new() { Type = "training", Description = "Combat training from grateful owner" }
        };
    }
}

public class StoreRelationshipProgression
{
    public void BuildRelationshipThroughTrade(Player player, Store store, StoreTransaction transaction)
    {
        var owner = store.Owner;
        var relationship = owner.RelationshipWithPlayer;

        // Regular trading builds relationship slowly
        if (transaction.Success && transaction.Type == TransactionType.Buy)
        {
            relationship.AddPoints(0.05f);

            // Buying expensive items builds more trust
            if (transaction.TotalValue > 1000)
            {
                relationship.AddTrust(0.1f);
            }
        }

        // Selling rare items builds significant relationship
        if (transaction.Success && transaction.Type == TransactionType.Sell)
        {
            var item = transaction.Item;
            if (item.Rarity >= ItemRarity.Rare)
            {
                relationship.AddPoints(0.15f);
                relationship.AddTrust(0.1f);

                // Unlock insider knowledge
                if (relationship.Level >= RelationshipLevel.Trusted)
                {
                    UnlockMarketInformation(player, store);
                }
            }
        }
    }

    private void UnlockMarketInformation(Player player, Store store)
    {
        var marketIntel = new MarketInformation
        {
            PriceFluctuations = GetUpcomingPriceChanges(),
            RareItemArrivals = GetScheduledRareItems(),
            CompetitorWeaknesses = GetCompetitorInformation(),
            SecretSuppliers = GetSecretSupplierContacts()
        };

        player.Knowledge.AddMarketIntel(marketIntel);

        store.Owner.SayToPlayer($"Eftersom vi har handlat så mycket... låt mig berätta några handelsshemligheter för dig.");
    }
}
```

## 🎮 Practical Economic Scenarios

### The Moral Dilemma Shop
```csharp
// You need powerful weapon, but limited options

// Option 1: Violence (immediate gains, long-term problems)
"rob blacksmith" →
"Du hotar smeden med kniv. Han ger dig sitt bästa svärd, men flyr sedan staden.
 Du får: Legendary Sword
 Consequences: Ingen att uppgradera vapen, staden blir mer farlig"

// Option 2: Relationship building (slow but sustainable)
"befriend blacksmith" →
"Du spenderar tid med smeden, hjälper i verkstaden, köper små saker.
 Efter 2 veckor: 'Du är som en son för mig. Här, ta detta svärd gratis.'"

// Option 3: Protection service (heroic path)
"protect shop from bandits" →
"Du räddar verkstaden från rånare.
 'Tack! Gratis uppgraderingar för livet! Plus jag lär dig några trick.'"

// Option 4: Economic manipulation (clever but risky)
"undercut competitor prices" →
"Du startar din egen vapenhandel och krossar konkurrenterna.
 Result: Du kontrollerar marknaden, men många fiender och ekonomisk instabilitet"
```

### The Dead Merchant Consequence Chain
```csharp
// Player killed weapon merchant 3 weeks ago

"look for weapons" →
"De flesta butiker är stängda. Magnus's Armoury är övergiven, fönstren krossade.
 En skylt säger: 'CLOSED - Owner Missing'. Blodfläckar på tröskeln."

"try to buy sword elsewhere" →
"'Nej, vi säljer inga vapen längre. För farligt. Har du hört vad som hände Magnus?'"

"check black market" →
"I skuggorna hittar du smugglare:
 'Vapen? Dyrt nu. Magnus dog, så utbudet är lågt. 5x normalpris.'"

"try to upgrade existing weapon" →
"'Uppgradera? Magnus var den enda som kunde göra det. Han är... borta.
 Din weapon kommer att försämras över tid utan maintenance.'"

// Long-term consequences cascade
"months later" →
"Staden har ingen vapensmed. Banditer attackerar oftare.
 Dina vapen är rostiga och dåliga. NPCs är rädda för dig.
 Quests som krävde handelskontakter är permanently blocked."
```

### The Hero's Economic Reward
```csharp
// Player consistently protected merchants

"enter any shop" →
"Ägaren ler brett: 'Vår hjälte! 25% rabatt som vanligt.'"

"ask about rare items" →
"'För dig? Jag har något speciellt i backroom...
 Detta kom från huvudstaden. Bara för dig.'"

"need weapon repair" →
"'Gratis självklart! Du räddade min butik tre gånger.
 Hmm, jag kan även förbättra denna lite...'"

"ask for information" →
"'Jag hörde att Dragon's Cave har öppnats igen.
 Och här, ta dessa potions. Du kommer att behöva dem.'"

// Economic network benefits
"travel to new town" →
"Merchant: 'Du är den berömda hjälten! Min kusin i gamla staden
 berättade om dig. Välkommen till mitt hus, vad behöver du?'"
```

---

## 🚀 Benefits av Economic Consequence System

### För Spelare:
✅ **Meaningful Choices** - varje beslut har long-term consequences
✅ **Moral Complexity** - snabba gains vs sustainable relationships
✅ **Economic Strategy** - planera för future needs och market stability
✅ **Character Development** - bli hero, villain, eller economic mastermind

### För Game Designers:
✅ **Rich Consequences** - actions påverkar world permanently
✅ **Emergent Storytelling** - economic choices skapar personal narratives
✅ **Replay Value** - olika economic strategies ger olika experiences
✅ **Social Simulation** - NPCs reagerar naturligt på economic disruption

### För World Builders:
✅ **Living Economy** - world känns alive med interdependent systems
✅ **Realistic Consequences** - destroy infrastructure, suffer consequences
✅ **Relationship Networks** - economic ties skapar social webs
✅ **Dynamic Content** - world changes permanently baserat på player actions

**Economic System med consequences revolutionerar player choice från "what can I get now?" till "what kind of world do I want to live in?" 💰🚀**

Violence ger immediate gains men destroy sustainable prosperity. Relationship building skapar long-term benefits och stable economy. Player måste välja mellan short-term power och long-term world stability!