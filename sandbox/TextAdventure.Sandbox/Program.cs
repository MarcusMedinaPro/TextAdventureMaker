using MarcusMedina.TextAdventure.Commands;
using MarcusMedina.TextAdventure.Dsl;
using MarcusMedina.TextAdventure.Engine;
using MarcusMedina.TextAdventure.Extensions;
using MarcusMedina.TextAdventure.Parsing;
using static MarcusMedina.TextAdventure.Extensions.ConsoleExtensions;

string dsl = """
world: Blackthorn Lighthouse
goal: Relight the beacon before the cutters run aground.
start: quay
command_alias: blow=use

location: quay | A wet stone quay with a fog horn, an iron chain, and a weather board nailed to a piling.
item: chain | iron chain | A salt-stiff iron chain bolted into the quay stones. | aliases=chain,iron chain | takeable=false
item: horn | fog horn | A brass fog horn on a cracked wooden stand. | aliases=horn,fog horn | takeable=false
item_reaction: horn | on=use | text=Tuuuuuut
item: board | weather board | A chalk board listing tide times and keeper notes. | aliases=board,weather board | takeable=false
item: chart | sea chart | A chart of local shoals and hidden reefs. | aliases=chart,map
key: brass_key | brass key | A tarnished brass key with a lighthouse crest. | aliases=key
exit: east -> keeper_house | door=keeper_door
exit: north -> signal_stairs
exit: west -> cliff_path

location: keeper_house | The keeper's house with a ledger desk, a paraffin kettle, and a framed rescue map.
item: ledger | signal ledger | A thick ledger recording lamp failures and ship sightings. | aliases=ledger,book | takeable=false
item: kettle | paraffin kettle | A dented kettle warming over a spirit flame. | aliases=kettle | takeable=false
item: rescue_map | rescue map | A map with lifeboat routes inked in red. | aliases=map,rescue map | takeable=false
npc: keeper | name=Keeper Sable | state=friendly | movement=patrol:keeper_house,quay,keeper_house | description=A weather-beaten keeper with oil-stained cuffs and a voice like gravel.
npc_place: keeper_house | keeper
npc_dialog: keeper | text=If the lens is in place and the feed is primed, we might still save them.
npc_reaction: keeper | on=talk | text=The keeper strokes his chin thoughtfully. "The hatch to the cellar is stiff, but it might still open. The lamp's reserve lens is in the gallery, but the stairs are rickety and the gate is stuck fast during the day."
npc_reaction: keeper | on=talk,has_item=brass_key | text=The keeper's eyes widen at the sight of the key. "Where did you find that? It looks like it might fit the cellar hatch. If you can get down there, you might be able to get the lamp working again."
npc_reaction: keeper | on=talk,has_item=brass_key,has_item=reserve_lens | text=The keeper's face breaks into a grin. "With the key and the reserve lens, you might just be able to get the lamp working again. The hatch is stiff, but it might still open. The stairs are rickety and the gate is stuck fast during the day."
npc_reaction: keeper | on=talk,has_item=brass_key,has_item=reserve_lens,has_item=tonic | text=The keeper's eyes widen at the sight of the tonic. "With the key, the reserve lens, and that storm tonic, you might just be able to get the lamp working again. The hatch is stiff, but it might still open. The stairs are rickety and the gate is stuck fast during the day."
npc_reaction: keeper | on=talk,has_item=brass_key,has_item=reserve_lens,has_item=tonic,has_item=coal | text=The keeper's face breaks into a grin. "With the key, the reserve lens, that storm tonic, and some dry lamp coal, you might just be able to get the lamp working again. The hatch is stiff, but it might still open. The stairs are rickety and the gate is stuck fast during the day."
npc_reaction: keeper | on=talk,has_item=brass_key,has_item=reserve_lens,has_item=tonic,has_item=coal,door_unlocked=cellar_hatch | text=The keeper's eyes widen at the sight of the unlocked hatch. "With the key, the reserve lens, that storm tonic, some dry lamp coal, and the hatch open, you might just be able to get the lamp working again. The stairs are rickety and the gate is stuck fast during the day."
npc_reaction: keeper | on=use | text=The keeper looks at you "WTF are you trying to do?"

exit: down -> cellar | door=cellar_hatch
exit: west -> quay | door=keeper_door

location: cellar | A cramped coal cellar with a split crate, a medicine tin, and a wall valve.
item: crate | split crate | A split crate still full of coal dust. | aliases=crate | takeable=false
item: valve | wall valve | A rusted wall valve controlling fuel feed. | aliases=valve,wheel | takeable=false
item: coal | lamp coal | A sack of lamp coal, dry enough to burn cleanly. | aliases=coal
item: tonic | storm tonic | A bitter tonic used to steady shaking hands. | aliases=tonic
exit: up -> keeper_house | door=cellar_hatch

location: signal_stairs | Narrow stairs with a handrail, a warning bell, and a cracked inspection mirror.
item: handrail | iron handrail | A cold handrail polished by decades of anxious hands. | aliases=rail,handrail | takeable=false
item: bell | warning bell | A warning bell with a frayed pull rope. | aliases=bell
item: mirror | inspection mirror | A mirror used to check lamp smoke from below. | aliases=mirror | takeable=false
exit: south -> quay
exit: up -> lantern_gallery
timed_door: up | opens_at=dusk | closes_at=day | message=The gallery gate clicks open for the evening watch. | closed_message=The gallery gate is bolted for daylight repairs.

location: lantern_gallery | The lantern gallery with a lens cradle, oil feed, and beacon controls.
item: lens_cradle | lens cradle | A brass cradle where the primary lens should sit. | aliases=cradle,lens cradle | takeable=false
item: oil_feed | oil feed | A copper oil feed with a pressure gauge. | aliases=feed,oil feed | takeable=false
item: controls | beacon controls | A set of old controls marked PRIME, IGNITE, and SHUT. | aliases=controls,panel | takeable=false
item: reserve_lens | reserve lens | A heavy reserve lens wrapped in canvas.
exit: down -> signal_stairs

location: cliff_path | A cliff path with marker posts, a mile bell, and a bent signal flag.
item: posts | marker posts | Marker posts painted white to catch moonlight. | aliases=posts,markers | takeable=false
item: mile_bell | mile bell | A bell used to report visibility by sound. | aliases=bell,mile bell | takeable=false
item: flag | signal flag | A torn signal flag snapping in the wind. | aliases=flag
exit: east -> quay

# Doors

door: keeper_door | keeper door | A thick oak door swollen by sea damp. | key=brass_key

door: cellar_hatch | cellar hatch | A bolted hatch with a stiff locking plate. | key=brass_key

# Timed world objects

timed_spawn: flare | appears_at=night | disappears_at=day | message=A red distress flare has washed against the quay wall.
""";

DslParser dslParser = new();
DslAdventure adventure = dslParser.ParseString(dsl);

if (adventure.HasWarnings)
{
    foreach (DslParseError warning in adventure.Warnings)
        Console.WriteLine($"DSL warning (line {warning.Line}): {warning.Message}");
}

KeywordParserConfigBuilder parserBuilder = KeywordParserConfigBuilder.BritishDefaults()
    .WithFuzzyMatching(true, 1)
    .WithPhraseAlias("look at", "look");

foreach (DslCommandAlias alias in dslParser.GetParserConfiguration().CommandAliases)
{
    if (!string.IsNullOrWhiteSpace(alias.Alias) && !string.IsNullOrWhiteSpace(alias.TargetCommand))
        parserBuilder.AddSynonyms(alias.TargetCommand, alias.Alias);
}

foreach (DslCustomVerb verb in dslParser.GetParserConfiguration().CustomVerbs)
    parserBuilder.AddCustomVerb(verb.Verb);

KeywordParser parser = new(parserBuilder.Build());

SetupC64("Blackthorn Lighthouse");
WriteLineC64("=== BLACKTHORN LIGHTHOUSE ===");
WriteLineC64(adventure.WorldName ?? "Blackthorn Lighthouse");
WriteLineC64($"Goal: {adventure.Goal}");
WriteLineC64("Commands: look, go <direction>, open, unlock, take, use, read, talk, inventory, quests, quit.");
WriteLineC64();

bool firstTurn = true;

Game game = GameBuilder.Create()
    .UseState(adventure.State)
    .UseParser(parser)
    .UsePrompt("> ")
    .AddTurnStart(g =>
    {
        if (!firstTurn)
            return;

        g.State.ShowRoom();
        firstTurn = false;
    })
    .AddTurnEnd((g, command, result) =>
    {
        if (result.ShouldAutoLook(command))
            g.State.ShowRoom();
    })
    .Build();

game.Run();
