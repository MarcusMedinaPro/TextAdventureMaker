## Slice 36: Hero's Journey & Narrative Templates

**Mål:** Inbyggda dramaturgiska strukturer som guide för story design.

### Task 36.1: IHeroJourney + JourneyStage enum

Campbell's 12-17 stages med 3 faser (Departure, Initiation, Return)

### Task 36.2: HeroJourneyBuilder — fluent API

```csharp
game.UseHeroJourneyTemplate()
    .OrdinaryWorld("village")
        .EstablishNormalcy("You've lived here all your life...")
    .CallToAdventure()
        .Trigger(call => call.OnEvent("village_attacked"))
    .MeetingMentor("wise_hermit")
        .Provides(gift => gift.Item("magic_amulet"))
    .CrossingThreshold()
        .PointOfNoReturn()
    .Ordeal()
        .FaceGreatestFear("dragon")
    .ReturnWithElixir()
        .TransformationComplete(hero => hero.SetTitle("Dragonslayer"))
    .Build();
```

### Task 36.3: JourneyValidator

- Varnar om saknade stages
- Kontrollerar ordning (Reward före Ordeal = fel)
- "Mentor saknas före Threshold"

### Task 36.4: Character Archetypes

```csharp
public enum CharacterArchetype
{
    Hero, Mentor, Threshold_Guardian, Herald,
    Shapeshifter, Shadow, Ally, Trickster
}
npc.SetArchetype(CharacterArchetype.Mentor)
    .DiesAt(JourneyStage.Ordeal);
```

### Task 36.5: Alternative Narrative Templates

**The Tragic Arc (Aristoteles)**

```csharp
game.UseTragicArc()
    .Hybris("hero_overconfident")
    .Hamartia("fatal_flaw")  // Felsteget
    .Peripeteia("reversal")  // Oundviklig konsekvens
    .Anagnorisis("recognition")  // Insikt för sent
    .Katharsis("audience_emotion");
```

**The Transformation Arc (inre resa)**

```csharp
game.UseTransformationArc()
    .FragmentedIdentity()
    .ConfrontShadow()
    .Integration()
    .NewSelfImage();
```

**The Ensemble Journey (kollektiv)**

```csharp
game.UseEnsembleJourney()
    .Protagonists("luke", "leia", "han")
    .ShiftingPerspectives()
    .InternalConflicts()
    .NoSingleSavior();
```

**The Descent / Katabasis**

```csharp
game.UseDescentArc()
    .DescentIntoChaos()
    .LossOfControl()
    .ConfrontDeath()
    .ReturnChanged();  // eller .NoReturn()
```

**The Spiral Narrative**

```csharp
game.UseSpiralNarrative()
    .RepeatEvents()
    .WithVariations()
    .DeeperUnderstandingEachLoop()
    .TimeLoops();
```

**The Moral Labyrinth**

```csharp
game.UseMoralLabyrinth()
    .NoCorrectEnding()
    .AllChoicesCost()
    .SituationalTruth();
```

**The Caretaker Arc**

```csharp
game.UseCaretakerArc()
    .RepairNotConquer()
    .HealStabilizeProtect()
    .FightEntropy();
```

**The Witness Arc**

```csharp
game.UseWitnessArc()
    .ObserveMoreThanAct()
    .CollectStories()
    .AssembleTruth()
    .ChangeByUnderstanding();
```

**The World-Shift Arc**

```csharp
game.UseWorldShiftArc()
    .GradualWorldChange()
    .PlayerAsCatalyst()  // Inte hjälte
    .SystemsCollide("ecology", "politics", "magic")
    .NewEquilibrium();
```

### Task 36.6: DSL för journey templates

```
journey "DragonSlayer" type: hero {
    phase departure {
        ordinary_world "village" { routine: farming }
        call trigger: event "village_attacked"
        mentor "hermit" gives: "amulet"
        threshold "dark_forest" no_return: true
    }
    phase initiation {
        tests count: 3
        ordeal "dragon" death_rebirth: true
        reward: "dragon_hoard"
    }
    phase return {
        road_back pursuit: "dragon_mate"
        elixir title: "Dragonslayer"
    }
}
```

### Task 36.7: Additional Story Structures (from book research)

**Prescriptive/Fill-in-the-blank (UX Storytelling)**

```csharp
game.UsePrescriptiveStructure()
    .Given("context")
    .When("event")
    .Then("outcome");
```

**Familiar to Foreign (Alice in Wonderland)**

```csharp
game.UseFamiliarToForeign()
    .FamiliarWorld("home")
    .TransitionEvent("rabbit_hole")
    .ForeignWorld("wonderland")
    .ReturnWithInsight();
```

**Framed Stories**

```csharp
// Now-Then-Now (flashback)
game.UseFramedNarrative()
    .Present("current_situation")
    .Flashback("memory")
    .ReturnWithNewUnderstanding();

// Me-Them-Me (comparison)
game.UseFramedNarrative()
    .MyPerspective()
    .TheirPerspective("other_character")
    .MyNewUnderstanding();

// Here-There-Here (Odysseus)
game.UseFramedNarrative()
    .StartLocation("home")
    .JourneyTo("foreign_land")
    .ReturnHome();
```

---

## Implementation checklist (engine)
- [ ] `IHeroJourney` + `JourneyStage`
- [ ] `HeroJourneyBuilder`
- [ ] `JourneyValidator`
- [ ] `CharacterArchetype` system
- [ ] Alternative narrative template APIs (tragic, transformation, ensemble, descent, spiral, moral, caretaker, witness, world-shift, prescriptive, familiar-to-foreign, framed)
- [ ] DSL support for journey templates

## Example checklist (docs/examples)
- [x] Narrative template examples documented (`docs/examples/36_*.md`)

**Layered Stories**

```csharp
game.UseLayeredNarrative()
    .AddLayer(1, "surface_impression")
    .AddLayer(2, "deeper_context")
    .AddLayer(3, "hidden_truth")
    .RevealLayersProgressively();
```

### Task 36.8: Propp's Folktale Functions (Procedural Narrative)

_Från Procedural Storytelling in Game Design_

```csharp
public enum ProppFunction
{
    Absentation,        // En familjemedlem lämnar hemmet
    Interdiction,       // Hjälten får en varning
    Violation,          // Varningen ignoreras
    Reconnaissance,     // Skurken spanar
    Delivery,           // Skurken får info om hjälten
    Trickery,           // Skurken försöker lura
    Complicity,         // Hjälten blir lurad
    Villainy,           // Skurken skadar någon/något
    Mediation,          // Hjälten blir medveten om problemet
    Counteraction,      // Hjälten bestämmer sig för att agera
    Departure,          // Hjälten ger sig av
    DonorTest,          // Hjälten testas av givaren
    HeroReaction,       // Hjälten reagerar på testet
    AcquisitionOfAgent, // Hjälten får magiskt föremål
    Guidance,           // Hjälten leds till målet
    Struggle,           // Hjälten och skurken strider
    Branding,           // Hjälten märks
    Victory,            // Skurken besegras
    LiquidationOfLack,  // Det ursprungliga problemet löses
    Return,             // Hjälten återvänder
    Pursuit,            // Hjälten jagas
    Rescue,             // Hjälten räddas
    UnrecognizedArrival,// Hjälten kommer hem okänd
    UnfoundedClaims,    // Falsk hjälte gör anspråk
    DifficultTask,      // Hjälten får svår uppgift
    Solution,           // Uppgiften löses
    Recognition,        // Hjälten erkänns
    Exposure,           // Falsk hjälte avslöjas
    Transfiguration,    // Hjälten förändras
    Punishment,         // Skurken straffas
    Wedding             // Hjälten gift sig/kröns
}

// Användning
game.UseProppianStructure()
    .RequiredFunctions(
        ProppFunction.Absentation,
        ProppFunction.Villainy,
        ProppFunction.Departure,
        ProppFunction.AcquisitionOfAgent,
        ProppFunction.Victory,
        ProppFunction.Return
    )
    .OptionalFunctions(ProppFunction.Pursuit, ProppFunction.Rescue)
    .ValidateOrder(true);  // Funktioner måste komma i rätt ordning
```

### Task 36.9: Flashback System

_Från UX Storytelling: Now-Then-Now structure_

```csharp
// Definiera minnen
game.AddMemory("childhood_trauma")
    .SetTrigger(trigger => trigger.OnEnterLocation("old_house"))
    .SetContent("The smell brings it all back...")
    .SetDuration(3);  // Antal turns i flashback

// Eller med fluent trigger
location.TriggerFlashback("memory_id")
    .When(player.Has("locket"))
    .WithTransition("Your vision blurs...")
    .ReturnsTo(currentLocation);
```

### Task 36.10: Layer-based Descriptions

_Från UX Storytelling: Layered stories_

```csharp
location.SetLayeredDescription()
    .FirstVisit("A dusty old library.")
    .SecondVisit("You notice scratch marks on the floor.")
    .ThirdVisit("The scratches lead to a hidden panel!")
    .OnItem("magnifying_glass", "With the glass, you see fingerprints.");
```

### Task 36.7: Journey Progress Tracker

```
=== YOUR JOURNEY ===
Phase: Initiation
Stage: 6/12

[✓][✓][✓][✓][✓][●][ ][ ][ ][ ][ ][ ]
 1  2  3  4  5  6  7  8  9 10 11 12

Next: Approach Inmost Cave
```

### Task 36.8: Sandbox — Star Wars journey, Tragic alternative

---
