## Slice 20: Conditional Event Chains

**Mål:** Sekvenser av events som påverkar varandra.
**Notis:** Kodbasen använder för närvarande Slice 20 för Hints & Properties (se `docs/examples/20_Hints_and_Properties.md`).

### Task 20.1: IEventChain + ICondition interfaces

### Task 20.2: Time/location/state triggers

```csharp
game.AddEventChain("village_rescue")
    .Step1(e => e.OnEnterLocation("village").ShowDialog("burning_houses"))
    .Step2(e => e.WhenItemFound("water_bucket").EnableAction("extinguish"))
    .Step3(e => e.WhenAllFiresOut().SpawnNpc("grateful_mayor"))
    .Step4(e => e.AfterTicks(20).If(q => !q.IsComplete("save_village"))
                 .Then(w => w.DestroyLocation("village")));
```

### Task 20.3: Sandbox — village rescue med tidslimit

---
