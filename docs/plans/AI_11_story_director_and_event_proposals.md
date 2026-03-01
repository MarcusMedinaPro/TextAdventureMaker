## AI_11: Story Director and Event Proposals

**Goal:** AI proposes what happens next based on map + world state, while engine validates effects.

### Tasks

1. Add `IStoryDirectorAiService`.
2. Build `StoryDirectorContext`:
   - current location graph excerpt
   - active quests/objectives
   - tension/mood/flags
3. AI outputs structured event proposal:
   - trigger conditions
   - narrative beat text
   - optional consequences
4. Validation layer ensures event legality before applying.

### Tests

- Proposal with invalid location is rejected.
- Valid proposal can be converted into event chain action.
- Story progression remains deterministic when AI disabled.
