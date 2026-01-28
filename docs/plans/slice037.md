## Slice 37: Generic Chapter System

**Mål:** Flexibel kapitelstruktur utan låst template. Bygg din egen arc.

### Task 37.1: IChapter + ChapterState

```csharp
public interface IChapter
{
    string Id { get; }
    string Title { get; }
    ChapterState State { get; }  // NotStarted, Active, Completed, Skipped
    IEnumerable<IChapterObjective> Objectives { get; }
    IChapter? NextChapter { get; }
}
```

### Task 37.2: ChapterBuilder — define custom arcs

```csharp
game.DefineChapters()
    .Chapter("prologue", "The Beginning")
        .Objectives(o => o
            .Required("wake_up")
            .Required("leave_house")
            .Optional("talk_to_neighbor"))
        .OnComplete(ctx => ctx.Message("Chapter 1 begins..."))

    .Chapter("chapter1", "Into the Unknown")
        .UnlockedWhen(c => c.IsComplete("prologue"))
        .Objectives(o => o
            .Required("reach_forest")
            .Required("find_map")
            .Branch("help_stranger", leadsTo: "chapter2a")
            .Branch("ignore_stranger", leadsTo: "chapter2b"))

    .Chapter("chapter2a", "The Ally Path")
        // ...

    .Chapter("chapter2b", "The Lone Path")
        // ...

    .Chapter("finale", "The End")
        .ConvergesFrom("chapter2a", "chapter2b")
        .MultipleEndings(endings => endings
            .Ending("good", when: w => w.GetCounter("karma") > 50)
            .Ending("bad", when: w => w.GetCounter("karma") < -50)
            .Ending("neutral", isDefault: true))

    .Build();
```

### Task 37.3: Chapter transitions och branching

```csharp
// Auto-advance
game.OnObjectiveComplete("find_map", ctx =>
    ctx.AdvanceChapter());

// Manual control
game.CurrentChapter.Complete();
game.ActivateChapter("chapter2a");
```

### Task 37.4: Chapter progress UI

```
╔══════════════════════════════════════╗
║  CHAPTER 2: Into the Unknown         ║
╠══════════════════════════════════════╣
║  [✓] Reach the forest                ║
║  [●] Find the ancient map            ║
║  [ ] Choose your path                ║
╚══════════════════════════════════════╝
```

### Task 37.5: DSL för chapters

```
chapters {
    chapter "prologue" title: "The Beginning" {
        objectives {
            required "wake_up"
            required "leave_house"
            optional "talk_neighbor"
        }
        on_complete: message "Chapter 1 begins..."
    }

    chapter "chapter1" title: "Into the Unknown" {
        unlocked_when: complete "prologue"
        branches {
            "help_stranger" -> "chapter2a"
            "ignore_stranger" -> "chapter2b"
        }
    }

    chapter "finale" {
        converges_from: ["chapter2a", "chapter2b"]
        endings {
            "good" when: karma > 50
            "bad" when: karma < -50
            "neutral" default: true
        }
    }
}
```

### Task 37.6: Sandbox — 3-chapter mini-adventure med branching

---
