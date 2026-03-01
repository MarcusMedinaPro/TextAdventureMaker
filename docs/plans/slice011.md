## Slice 11: Language System

**Mål:** Ladda språkfil för all speltext. Default engelska.

### Task 11.1: ILanguageProvider + JsonLanguageProvider

### Task 11.2: Language file format (gamelang.en.json, gamelang.sv.json)
### Task 11.2b: Deprecate .txt language files

### Task 11.3: System messages (You pick up the {item}, You can't go that way, etc.)

### Task 11.4: Sandbox — ladda svenskt språk

---

## Implementation checklist (engine)
- [x] `ILanguageProvider`
- [x] `JsonLanguageProvider` for `.json` language files
- [x] Default language provider fallback (English)
- [x] System messages routed through `Language`
- [x] `.txt` language provider deprecated (`FileLanguageProvider` marked obsolete)

## Example checklist (docs/examples)
- [x] Runtime language swap (`11_Before_the_Meeting.md`)
- [x] Language command aliases (`language`/`lang`) shown (`11_Before_the_Meeting.md`)
