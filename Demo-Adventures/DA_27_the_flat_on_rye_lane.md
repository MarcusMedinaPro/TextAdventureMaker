# Demo Adventure 27: The Flat on Rye Lane

Converted from `docs/examples/46_The_Flat_On_Rye_Lane.md` with expanded scene detail and denser red herring interaction.

## Premise
You are dropped into the flat on rye lane, where each decision pushes the night towards a harsher outcome.

## Expanded Description Pack
- Opening room: Layer in ambient sound, one tactile detail, and one subtle movement in the corner of view.
- Transitional space: Describe light quality (flicker, haze, reflected glare) to signal emotional shift.
- Safe-looking object: Make it richly described, but functionally unimportant, to support red herring play.
- Threat-adjacent zone: Add a repeated motif (metallic smell, distant tapping, static hum) that grows stronger.
- Finale space: Keep descriptions short, sharp, and concrete so the ending lands with urgency.

## Expanded Story Beats (10)
1) Arrive at the flat. Something feels wrong.
2) Explore the hallway. Notice the wallpaper is... breathing.
3) Enter the kitchen. The clock runs backwards.
4) Find a note in the bedroom. The handwriting is yours.
5) The bathroom mirror shows someone behind you. No one is there.
6) Return to the hallway. The front door is gone.
7) Read the note again. The text has changed.
8) Find the hidden door behind the wallpaper.
9) Step through. The flat resets. You are outside again.
10) A misleading clue wastes precious time but reveals character tension.

## ASCII Map
```
          N
    W           E
          S

                    ┌─────────┐
                    │ Bathroom│
                    │   M     │
                    └────┬────┘
                         │
┌──────────┐    ┌────────────────┐    ┌──────────┐
│ Bedroom  ├────┤    Hallway     ├────┤ Kitchen  │
│   N      │    │   W            │    │   C      │
└──────────┘    └────────────────┘    └──────────┘

M = Mirror (fixed)
N = Note
W = Wallpaper (fixed, hides door)
C = Clock (fixed)
```

## Atmosphere Notes
- Descriptions should stress texture, sound, and small motion: humming lights, distant steps, and stale air.
- Each room should include one sensory anchor (smell, temperature, vibration, or light quality).
- Use short reactive lines when players repeat actions, so repeated checks still feel intentional.
- Red herring objects should always answer to `look`, and if possible also `open` or `read`.
- Keep key progression items visually ordinary so players cannot spot them by rarity cues.

## Red Herrings and Interactables
- Framed noticeboard with outdated warnings that seem important but are not.
- Locked drawer containing harmless paperwork and old receipts.
- Cracked mirror that offers flavour text and mood reactions only.
- Vending machine or cabinet that can be opened, inspected, and dismissed.
- Discarded personal item (ticket, keyring, glove) with no quest value.
- Window, vent, or hatch with detailed responses but no progression impact.
- Wall clock that can be checked repeatedly for creeping tension.
- Mundane furniture (chair, trolley, cabinet) with tactile descriptions.
- Readable leaflet or memo with lore hints and false leads.
- Ambient sound source that changes text over time without blocking progress.

## Implementation Guidance (Non-AI)
- Add custom handling for `open`, `read`, `look`, and `examine` on every listed red herring.
- Keep progression linear unless the source story clearly defines alternate outcomes.
- Auto-look after successful movement and preserve British English for all output text.
- Ensure every visible object has at least one response: take, inspect, or explicit refusal.
