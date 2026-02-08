## Slice 44: String Case Utilities

**Mål:** Enkla stränghelpers för casing i UI/texter.

### Funktioner

- `string.ToProperCase()` — Title Case.
- `string.ToSentenceCase()` — Första bokstaven versal, resten gemener.
- `string.ToCrazyCaps()` — Slumpad versal/gemen per bokstav.

**Notis:** Använder `Random.Shared`.

---

## Implementation checklist (engine)
- [x] `string.ToProperCase()`
- [x] `string.ToSentenceCase()`
- [x] `string.ToCrazyCaps()`

## Example checklist (docs/examples)
- [ ] String case helpers demo
