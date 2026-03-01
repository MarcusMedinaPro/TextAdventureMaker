// <copyright file="NarrativeTemplateBuilders.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using MarcusMedina.TextAdventure.Models;

namespace MarcusMedina.TextAdventure.Engine;

public abstract class NarrativeTemplateBuilder<TBuilder> where TBuilder : NarrativeTemplateBuilder<TBuilder>
{
    protected NarrativeTemplate Template { get; }

    protected NarrativeTemplateBuilder(string name)
    {
        Template = new NarrativeTemplate(name);
    }

    protected TBuilder Add(string step)
    {
        Template.AddStep(step);
        return (TBuilder)this;
    }

    public NarrativeTemplate Build()
    {
        return Template;
    }
}

public sealed class TragicArcBuilder : NarrativeTemplateBuilder<TragicArcBuilder>
{
    public TragicArcBuilder() : base("TragicArc") { }
    public TragicArcBuilder Hybris(string id) => Add($"hybris:{id}");
    public TragicArcBuilder Hamartia(string id) => Add($"hamartia:{id}");
    public TragicArcBuilder Peripeteia(string id) => Add($"peripeteia:{id}");
    public TragicArcBuilder Anagnorisis(string id) => Add($"anagnorisis:{id}");
    public TragicArcBuilder Katharsis(string id) => Add($"katharsis:{id}");
}

public sealed class TransformationArcBuilder : NarrativeTemplateBuilder<TransformationArcBuilder>
{
    public TransformationArcBuilder() : base("TransformationArc") { }
    public TransformationArcBuilder FragmentedIdentity() => Add("fragmented_identity");
    public TransformationArcBuilder ConfrontShadow() => Add("confront_shadow");
    public TransformationArcBuilder Integration() => Add("integration");
    public TransformationArcBuilder NewSelfImage() => Add("new_self_image");
}

public sealed class EnsembleJourneyBuilder : NarrativeTemplateBuilder<EnsembleJourneyBuilder>
{
    public EnsembleJourneyBuilder() : base("EnsembleJourney") { }
    public EnsembleJourneyBuilder Protagonists(params string[] ids) => Add($"protagonists:{string.Join(",", ids ?? [])}");
    public EnsembleJourneyBuilder ShiftingPerspectives() => Add("shifting_perspectives");
    public EnsembleJourneyBuilder InternalConflicts() => Add("internal_conflicts");
    public EnsembleJourneyBuilder NoSingleSavior() => Add("no_single_savior");
}

public sealed class DescentArcBuilder : NarrativeTemplateBuilder<DescentArcBuilder>
{
    public DescentArcBuilder() : base("DescentArc") { }
    public DescentArcBuilder DescentIntoChaos() => Add("descent_into_chaos");
    public DescentArcBuilder LossOfControl() => Add("loss_of_control");
    public DescentArcBuilder ConfrontDeath() => Add("confront_death");
    public DescentArcBuilder ReturnChanged() => Add("return_changed");
    public DescentArcBuilder NoReturn() => Add("no_return");
}

public sealed class SpiralNarrativeBuilder : NarrativeTemplateBuilder<SpiralNarrativeBuilder>
{
    public SpiralNarrativeBuilder() : base("SpiralNarrative") { }
    public SpiralNarrativeBuilder RepeatEvents() => Add("repeat_events");
    public SpiralNarrativeBuilder WithVariations() => Add("with_variations");
    public SpiralNarrativeBuilder DeeperUnderstandingEachLoop() => Add("deeper_understanding");
    public SpiralNarrativeBuilder TimeLoops() => Add("time_loops");
}

public sealed class MoralLabyrinthBuilder : NarrativeTemplateBuilder<MoralLabyrinthBuilder>
{
    public MoralLabyrinthBuilder() : base("MoralLabyrinth") { }
    public MoralLabyrinthBuilder NoCorrectEnding() => Add("no_correct_ending");
    public MoralLabyrinthBuilder AllChoicesCost() => Add("all_choices_cost");
    public MoralLabyrinthBuilder SituationalTruth() => Add("situational_truth");
}

public sealed class CaretakerArcBuilder : NarrativeTemplateBuilder<CaretakerArcBuilder>
{
    public CaretakerArcBuilder() : base("CaretakerArc") { }
    public CaretakerArcBuilder RepairNotConquer() => Add("repair_not_conquer");
    public CaretakerArcBuilder HealStabilizeProtect() => Add("heal_stabilize_protect");
    public CaretakerArcBuilder FightEntropy() => Add("fight_entropy");
}

public sealed class WitnessArcBuilder : NarrativeTemplateBuilder<WitnessArcBuilder>
{
    public WitnessArcBuilder() : base("WitnessArc") { }
    public WitnessArcBuilder ObserveMoreThanAct() => Add("observe_more_than_act");
    public WitnessArcBuilder CollectStories() => Add("collect_stories");
    public WitnessArcBuilder AssembleTruth() => Add("assemble_truth");
    public WitnessArcBuilder ChangeByUnderstanding() => Add("change_by_understanding");
}

public sealed class WorldShiftArcBuilder : NarrativeTemplateBuilder<WorldShiftArcBuilder>
{
    public WorldShiftArcBuilder() : base("WorldShiftArc") { }
    public WorldShiftArcBuilder GradualWorldChange() => Add("gradual_world_change");
    public WorldShiftArcBuilder PlayerAsCatalyst() => Add("player_as_catalyst");
    public WorldShiftArcBuilder SystemsCollide(params string[] systems) => Add($"systems_collide:{string.Join(",", systems ?? [])}");
    public WorldShiftArcBuilder NewEquilibrium() => Add("new_equilibrium");
}

public sealed class PrescriptiveStructureBuilder : NarrativeTemplateBuilder<PrescriptiveStructureBuilder>
{
    public PrescriptiveStructureBuilder() : base("PrescriptiveStructure") { }
    public PrescriptiveStructureBuilder Given(string context) => Add($"given:{context}");
    public PrescriptiveStructureBuilder When(string @event) => Add($"when:{@event}");
    public PrescriptiveStructureBuilder Then(string outcome) => Add($"then:{outcome}");
}

public sealed class FamiliarToForeignBuilder : NarrativeTemplateBuilder<FamiliarToForeignBuilder>
{
    public FamiliarToForeignBuilder() : base("FamiliarToForeign") { }
    public FamiliarToForeignBuilder FamiliarWorld(string location) => Add($"familiar_world:{location}");
    public FamiliarToForeignBuilder TransitionEvent(string @event) => Add($"transition_event:{@event}");
    public FamiliarToForeignBuilder ForeignWorld(string location) => Add($"foreign_world:{location}");
    public FamiliarToForeignBuilder ReturnWithInsight() => Add("return_with_insight");
}

public sealed class FramedNarrativeBuilder : NarrativeTemplateBuilder<FramedNarrativeBuilder>
{
    public FramedNarrativeBuilder() : base("FramedNarrative") { }
    public FramedNarrativeBuilder Present(string moment) => Add($"present:{moment}");
    public FramedNarrativeBuilder Flashback(string memory) => Add($"flashback:{memory}");
    public FramedNarrativeBuilder ReturnWithNewUnderstanding() => Add("return_with_new_understanding");
    public FramedNarrativeBuilder MyPerspective() => Add("my_perspective");
    public FramedNarrativeBuilder TheirPerspective(string character) => Add($"their_perspective:{character}");
    public FramedNarrativeBuilder MyNewUnderstanding() => Add("my_new_understanding");
    public FramedNarrativeBuilder StartLocation(string location) => Add($"start_location:{location}");
    public FramedNarrativeBuilder JourneyTo(string location) => Add($"journey_to:{location}");
    public FramedNarrativeBuilder ReturnHome() => Add("return_home");
}

public sealed class LayeredNarrativeBuilder : NarrativeTemplateBuilder<LayeredNarrativeBuilder>
{
    public LayeredNarrativeBuilder() : base("LayeredNarrative") { }
    public LayeredNarrativeBuilder AddLayer(int index, string label) => Add($"layer:{index}:{label}");
    public LayeredNarrativeBuilder RevealLayersProgressively() => Add("reveal_progressively");
}
