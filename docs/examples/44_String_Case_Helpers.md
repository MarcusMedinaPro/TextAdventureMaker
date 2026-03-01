# String Case Helpers

_Slice tag: Slice 44 — String case utilities._

## Example (case helpers)
```csharp
using MarcusMedina.TextAdventure.Extensions;
using static MarcusMedina.TextAdventure.Extensions.ConsoleExtensions;

// Slice 44 — String case helpers
// Tests:
// - ToProperCase, ToSentenceCase, ToCrazyCaps

SetupC64("String Case Helpers - Text Adventure Sandbox");

string title = "a quiet hallway";
string sentence = "SOME LOUD WORDS";
string crazy = "quiet storm";

WriteLineC64($"Proper: {title.ToProperCase()}");
WriteLineC64($"Sentence: {sentence.ToSentenceCase()}");
WriteLineC64($"Crazy: {crazy.ToCrazyCaps()}");
```
