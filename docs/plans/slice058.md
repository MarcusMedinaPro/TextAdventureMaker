## Slice 58: Cinematic Presentation

**Mål:** Dramatisk textpresentation med typewriter-effekter, pauser, ASCII-art och stämningsförstärkning.

**Referens:** `docs/plans/imported/Future_Development_Ideas.md`

### Task 58.1: ITextPresenter interface

```csharp
public interface ITextPresenter
{
    Task PresentAsync(string text, PresentationOptions? options = null);
    void SetSpeed(TextSpeed speed);
    void SetStyle(TextStyle style);
}

public enum TextSpeed
{
    Instant,        // Allt på en gång
    Fast,           // 10ms/tecken
    Normal,         // 30ms/tecken
    Slow,           // 50ms/tecken
    Dramatic        // 100ms/tecken
}

public record PresentationOptions(
    TextSpeed Speed = TextSpeed.Normal,
    bool AllowSkip = true,
    TimeSpan? PauseBefore = null,
    TimeSpan? PauseAfter = null,
    ConsoleColor? Color = null
);
```

### Task 58.2: TypewriterPresenter

```csharp
public class TypewriterPresenter : ITextPresenter
{
    private TextSpeed _speed = TextSpeed.Normal;
    private readonly CancellationTokenSource _skipCts = new();

    public async Task PresentAsync(string text, PresentationOptions? options = null)
    {
        options ??= new PresentationOptions();

        if (options.PauseBefore.HasValue)
            await Task.Delay(options.PauseBefore.Value);

        var delay = GetDelay(options.Speed);

        foreach (var c in text)
        {
            if (_skipCts.Token.IsCancellationRequested)
            {
                Console.Write(text[text.IndexOf(c)..]);
                break;
            }

            Console.Write(c);
            await Task.Delay(delay);

            // Extra paus vid interpunktion
            if (c is '.' or '!' or '?')
                await Task.Delay(delay * 3);
            else if (c is ',' or ';' or ':')
                await Task.Delay(delay * 2);
        }

        Console.WriteLine();

        if (options.PauseAfter.HasValue)
            await Task.Delay(options.PauseAfter.Value);
    }

    private static int GetDelay(TextSpeed speed) => speed switch
    {
        TextSpeed.Instant => 0,
        TextSpeed.Fast => 10,
        TextSpeed.Normal => 30,
        TextSpeed.Slow => 50,
        TextSpeed.Dramatic => 100,
        _ => 30
    };
}
```

### Task 58.3: Text Markup System

```csharp
public class TextMarkupParser
{
    // Stöd för markup: [color=red]text[/color], [slow]text[/slow], [pause:500]
    public IEnumerable<TextSegment> Parse(string markup)
    {
        var regex = new Regex(@"\[(?<tag>\w+)(?::(?<value>\w+))?\](?<content>.*?)\[/\k<tag>\]|\[pause:(?<pause>\d+)\]|(?<plain>[^\[]+)");

        foreach (Match match in regex.Matches(markup))
        {
            if (match.Groups["pause"].Success)
            {
                yield return new PauseSegment(int.Parse(match.Groups["pause"].Value));
            }
            else if (match.Groups["tag"].Success)
            {
                var tag = match.Groups["tag"].Value;
                var value = match.Groups["value"].Value;
                var content = match.Groups["content"].Value;

                yield return tag switch
                {
                    "color" => new ColoredSegment(content, ParseColor(value)),
                    "slow" => new SpeedSegment(content, TextSpeed.Slow),
                    "fast" => new SpeedSegment(content, TextSpeed.Fast),
                    "dramatic" => new SpeedSegment(content, TextSpeed.Dramatic),
                    "bold" => new StyledSegment(content, FontStyle.Bold),
                    _ => new TextSegment(content)
                };
            }
            else
            {
                yield return new TextSegment(match.Groups["plain"].Value);
            }
        }
    }
}

// Exempel: "Du ser en [color=red]blodig[/color] kniv.[pause:500] [dramatic]Något är fel här...[/dramatic]"
```

### Task 58.4: ASCII Art System

```csharp
public class AsciiArtRenderer
{
    private readonly Dictionary<string, string[]> _art = [];

    public void LoadArt(string id, string[] lines) => _art[id] = lines;

    public void LoadFromFile(string path)
    {
        var content = File.ReadAllText(path);
        var sections = content.Split("---");

        foreach (var section in sections)
        {
            var lines = section.Split('\n', StringSplitOptions.RemoveEmptyEntries);
            if (lines.Length > 0)
            {
                var id = lines[0].Trim();
                _art[id] = lines[1..];
            }
        }
    }

    public async Task RenderAsync(string id, ITextPresenter presenter, bool animated = false)
    {
        if (!_art.TryGetValue(id, out var lines))
            return;

        foreach (var line in lines)
        {
            if (animated)
                await presenter.PresentAsync(line, new PresentationOptions(TextSpeed.Fast));
            else
                Console.WriteLine(line);
        }
    }
}

// Exempel ASCII-art fil:
// skull
// ---
//    _____
//   /     \
//  | () () |
//   \  ^  /
//    |||||
```

### Task 58.5: Mood/Atmosphere System

```csharp
public class AtmosphereSystem
{
    public Atmosphere CurrentMood { get; private set; } = Atmosphere.Neutral;

    public PresentationOptions GetPresentationForMood()
    {
        return CurrentMood switch
        {
            Atmosphere.Tense => new(TextSpeed.Slow, PauseAfter: TimeSpan.FromMilliseconds(200)),
            Atmosphere.Horror => new(TextSpeed.Dramatic, Color: ConsoleColor.DarkRed),
            Atmosphere.Peaceful => new(TextSpeed.Normal, Color: ConsoleColor.Green),
            Atmosphere.Urgent => new(TextSpeed.Fast, Color: ConsoleColor.Yellow),
            Atmosphere.Mysterious => new(TextSpeed.Slow, Color: ConsoleColor.DarkCyan),
            _ => new()
        };
    }

    public void SetMood(Atmosphere mood) => CurrentMood = mood;
}

public enum Atmosphere
{
    Neutral,
    Tense,
    Horror,
    Peaceful,
    Urgent,
    Mysterious,
    Comedic,
    Romantic,
    Epic
}
```

### Task 58.6: Sound Effect Hooks

```csharp
public interface ISoundHook
{
    void Play(string soundId);
    void SetVolume(float volume);
}

public class SoundTriggerSystem
{
    private readonly ISoundHook? _soundHook;

    public void OnTextPresented(string text, Atmosphere mood)
    {
        // Trigga ljud baserat på innehåll
        if (text.Contains("thunder", StringComparison.OrdinalIgnoreCase))
            _soundHook?.Play("thunder");

        if (text.Contains("door", StringComparison.OrdinalIgnoreCase) && text.Contains("creak"))
            _soundHook?.Play("door_creak");

        // Ambient baserat på mood
        var ambient = mood switch
        {
            Atmosphere.Horror => "ambient_horror",
            Atmosphere.Peaceful => "ambient_birds",
            Atmosphere.Mysterious => "ambient_wind",
            _ => null
        };

        if (ambient != null)
            _soundHook?.Play(ambient);
    }
}
```

### Task 58.7: Gradual Text Reveal

```csharp
public class GradualRevealPresenter : ITextPresenter
{
    // Visa text ord för ord istället för tecken för tecken
    public async Task PresentWordByWordAsync(string text, int delayMs = 200)
    {
        var words = text.Split(' ');

        foreach (var word in words)
        {
            Console.Write(word + " ");
            await Task.Delay(delayMs);
        }
        Console.WriteLine();
    }

    // Visa text rad för rad med fade-in effekt (simulerad)
    public async Task PresentWithFadeAsync(string text)
    {
        var lines = text.Split('\n');

        foreach (var line in lines)
        {
            // Simulera fade genom att visa punkter först
            Console.Write(new string('.', Math.Min(line.Length, 20)));
            await Task.Delay(100);
            Console.Write('\r');
            Console.WriteLine(line);
            await Task.Delay(300);
        }
    }
}
```

### Task 58.8: Tester

```csharp
[Fact]
public void TextMarkupParser_ParsesColorTags()
{
    var parser = new TextMarkupParser();
    var segments = parser.Parse("[color=red]blood[/color]").ToList();

    Assert.Single(segments);
    Assert.IsType<ColoredSegment>(segments[0]);
    Assert.Equal("blood", segments[0].Content);
}

[Fact]
public async Task TypewriterPresenter_CanBeSkipped()
{
    var presenter = new TypewriterPresenter();
    var task = presenter.PresentAsync("Long text...", new(TextSpeed.Slow, AllowSkip: true));

    presenter.Skip();

    await task;
    // Should complete quickly despite slow speed
}
```

### Task 58.9: Sandbox — skräckäventyr med atmosfär

Demo med dramatisk presentation, ASCII-art och stämningsmusik-hooks.

---
