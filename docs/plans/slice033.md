## Slice 33: Narrative Voice System

**Mål:** Flexibel berättarröst.

### Task 33.1: Voice enum: FirstPerson, SecondPerson, ThirdPerson

### Task 33.2: Tense enum: Past, Present

```csharp
game.SetNarrativeVoice(Voice.SecondPerson)
    .Tense(Tense.Present);

// Auto-adjusts all descriptions:
// 1st: "I enter the cave"
// 2nd: "You enter the cave"
// 3rd: "The hero enters the cave"
```

### Task 33.3: Flashbacks i past tense

### Task 33.4: Sandbox — byt perspektiv under spelet

---
