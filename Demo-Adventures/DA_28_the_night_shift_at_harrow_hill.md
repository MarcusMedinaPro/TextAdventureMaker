# Demo Adventure 28: The Night Shift at Harrow Hill

Converted from `docs/examples/47_The_Night_Shift_At_Harrow_Hill.md` with expanded scene detail and denser red herring interaction.

## Premise
You are dropped into the night shift at harrow hill, where each decision pushes the night towards a harsher outcome.

## Expanded Description Pack
- Opening room: Layer in ambient sound, one tactile detail, and one subtle movement in the corner of view.
- Transitional space: Describe light quality (flicker, haze, reflected glare) to signal emotional shift.
- Safe-looking object: Make it richly described, but functionally unimportant, to support red herring play.
- Threat-adjacent zone: Add a repeated motif (metallic smell, distant tapping, static hum) that grows stronger.
- Finale space: Keep descriptions short, sharp, and concrete so the ending lands with urgency.

## Expanded Story Beats (10)
1) Arrive at the hospital for the night shift.
2) Begin patrol: reception, ward, morgue, office.
3) Notice the logbook entry is wrong - tonight's date is last week.
4) A figure appears in the ward. She says she's a nurse. She follows you.
5) Round 2: the ward has changed. Beds are occupied. They weren't before.
6) Round 3: the nurse speaks of patients who died here. All of them. Tonight.
7) The logbook now has your name. Time of death: 03:17.
8) Round 4: the morgue drawer is open. Your name is on the tag.
9) The nurse is gone. She was never here.
10) The phone rings. "Your shift is over."

## ASCII Map
```
          N
    W           E
          S

┌──────────┐    ┌──────────┐
│ Office   ├────┤ Reception│
│   L  P   │    │   D      │
└────┬─────┘    └────┬─────┘
     │               │
┌────┴─────┐    ┌────┴─────┐
│ Morgue   ├────┤  Ward    │
│   D      │    │   B  N   │
└──────────┘    └──────────┘

L = Logbook
P = Phone (fixed)
D = Desk/Drawer
B = Beds (fixed)
N = Nurse (NPC, appears round 2)
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
