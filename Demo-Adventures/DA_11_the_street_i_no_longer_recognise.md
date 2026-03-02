# Demo Adventure 11: The Street I No Longer Recognise

Converted from `docs/examples/36_familiar_to_foreign_the_street_i_no_longer_recognise.md` with expanded scene detail and denser red herring interaction.

## Premise
You walk home from work. Same street, same house numbers. Yet every window is dark, and no mirror shows your reflection.

## Expanded Description Pack
- Opening room: Layer in ambient sound, one tactile detail, and one subtle movement in the corner of view.
- Transitional space: Describe light quality (flicker, haze, reflected glare) to signal emotional shift.
- Safe-looking object: Make it richly described, but functionally unimportant, to support red herring play.
- Threat-adjacent zone: Add a repeated motif (metallic smell, distant tapping, static hum) that grows stronger.
- Finale space: Keep descriptions short, sharp, and concrete so the ending lands with urgency.

## Expanded Story Beats (10)
1) The disturbance arrives and feels personal.
2) A rule is broken or a boundary is crossed.
3) A clue reveals the scale of the problem.
4) A choice narrows the world.
5) The environment answers back.
6) A truth is forced into view.
7) A price is paid, willingly or not.
8) The ending leaves a lingering echo.
9) A misleading clue wastes precious time but reveals character tension.
10) A quiet environmental change signals that the danger has moved closer.

## ASCII Map
```
          N
    W           E
          S

┌────────────┐     ┌────────────┐     ┌────────────┐
│ HomeStreet │─────│  Passage   │─────│ ForeignSt  │
│    T       │  E  │    M       │  E  │ Stranger   │
└────────────┘     └────────────┘     └────────────┘

T = Ticket
M = Map
Stranger = NPC
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
