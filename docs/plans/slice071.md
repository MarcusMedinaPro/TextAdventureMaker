## Slice 71: Crafting System

**Mål:** Kombinera items för att skapa nya föremål.

**Referens:** `docs/plans/slice045.md` (crafting idea)

### Task 71.1: ICraftingRecipe Interface

```csharp
public interface ICraftingRecipe
{
    string Id { get; }
    string Name { get; }
    string Description { get; }
    IReadOnlyList<RecipeIngredient> Ingredients { get; }
    IReadOnlyList<IItem> Results { get; }

    // Requirements
    string? RequiredToolId { get; }
    string? RequiredLocationTag { get; }
    int? RequiredSkillLevel { get; }

    bool CanCraft(ICharacter crafter, ILocation location);
    CraftingResult Craft(ICharacter crafter);
}

public record RecipeIngredient(
    string ItemId,
    int Quantity,
    bool IsConsumed = true
);

public record CraftingResult(
    bool Success,
    string Message,
    IReadOnlyList<IItem> CreatedItems
);
```

### Task 71.2: CraftingRecipe Implementation

```csharp
public class CraftingRecipe : ICraftingRecipe
{
    public string Id { get; init; } = "";
    public string Name { get; init; } = "";
    public string Description { get; init; } = "";
    public IReadOnlyList<RecipeIngredient> Ingredients { get; init; } = [];
    public IReadOnlyList<IItem> Results { get; init; } = [];

    public string? RequiredToolId { get; init; }
    public string? RequiredLocationTag { get; init; }
    public int? RequiredSkillLevel { get; init; }

    public bool CanCraft(ICharacter crafter, ILocation location)
    {
        // Check tool
        if (RequiredToolId != null && !crafter.Inventory.HasItem(RequiredToolId))
            return false;

        // Check location
        if (RequiredLocationTag != null && !location.HasTag(RequiredLocationTag))
            return false;

        // Check skill
        if (RequiredSkillLevel != null)
        {
            var skill = crafter.GetSkillLevel("crafting");
            if (skill < RequiredSkillLevel)
                return false;
        }

        // Check ingredients
        foreach (var ingredient in Ingredients)
        {
            var count = crafter.Inventory.CountItem(ingredient.ItemId);
            if (count < ingredient.Quantity)
                return false;
        }

        return true;
    }

    public CraftingResult Craft(ICharacter crafter)
    {
        if (!CanCraft(crafter, crafter.CurrentLocation))
        {
            return new CraftingResult(false, "You don't have the required materials or tools.", []);
        }

        // Consume ingredients
        foreach (var ingredient in Ingredients.Where(i => i.IsConsumed))
        {
            for (int i = 0; i < ingredient.Quantity; i++)
            {
                var item = crafter.Inventory.FindItem(ingredient.ItemId);
                if (item != null)
                    crafter.Inventory.Remove(item);
            }
        }

        // Create results
        var createdItems = Results.Select(r => r.Clone()).ToList();
        foreach (var item in createdItems)
        {
            crafter.Inventory.Add(item);
        }

        return new CraftingResult(
            true,
            $"You craft: {string.Join(", ", createdItems.Select(i => i.Name))}",
            createdItems
        );
    }
}
```

### Task 71.3: CraftingManager

```csharp
public class CraftingManager
{
    private readonly Dictionary<string, ICraftingRecipe> _recipes = [];
    private readonly Dictionary<string, List<ICraftingRecipe>> _recipesByIngredient = [];

    public IReadOnlyList<ICraftingRecipe> AllRecipes => _recipes.Values.ToList();

    public void RegisterRecipe(ICraftingRecipe recipe)
    {
        _recipes[recipe.Id] = recipe;

        // Index by ingredient for quick lookup
        foreach (var ingredient in recipe.Ingredients)
        {
            if (!_recipesByIngredient.ContainsKey(ingredient.ItemId))
                _recipesByIngredient[ingredient.ItemId] = [];

            _recipesByIngredient[ingredient.ItemId].Add(recipe);
        }
    }

    public ICraftingRecipe? GetRecipe(string id) =>
        _recipes.GetValueOrDefault(id);

    public IReadOnlyList<ICraftingRecipe> GetAvailableRecipes(ICharacter crafter)
    {
        return _recipes.Values
            .Where(r => r.CanCraft(crafter, crafter.CurrentLocation))
            .ToList();
    }

    public IReadOnlyList<ICraftingRecipe> GetRecipesUsing(string itemId)
    {
        return _recipesByIngredient.GetValueOrDefault(itemId) ?? [];
    }

    public IReadOnlyList<ICraftingRecipe> FindRecipesWith(IReadOnlyList<string> itemIds)
    {
        // Find recipes that can be made with the given items
        return _recipes.Values
            .Where(r => r.Ingredients.All(i => itemIds.Contains(i.ItemId)))
            .ToList();
    }
}
```

### Task 71.4: Common Recipes

```csharp
public static class Recipes
{
    public static CraftingRecipe Torch() => new()
    {
        Id = "torch",
        Name = "Torch",
        Description = "A burning torch for illumination.",
        Ingredients =
        [
            new RecipeIngredient("stick", 1),
            new RecipeIngredient("cloth", 1),
            new RecipeIngredient("oil", 1)
        ],
        Results = [LightSources.Torch()]
    };

    public static CraftingRecipe Lockpick() => new()
    {
        Id = "lockpick",
        Name = "Lockpick",
        Description = "A simple lockpick for opening locks.",
        Ingredients =
        [
            new RecipeIngredient("wire", 2)
        ],
        Results = [new Item("lockpick", "Lockpick")]
    };

    public static CraftingRecipe HealingSalve() => new()
    {
        Id = "healing_salve",
        Name = "Healing Salve",
        Description = "A restorative ointment.",
        Ingredients =
        [
            new RecipeIngredient("healing_herb", 2),
            new RecipeIngredient("water", 1)
        ],
        Results = [new Item("healing_salve", "Healing Salve")]
    };

    public static CraftingRecipe Raft() => new()
    {
        Id = "raft",
        Name = "Raft",
        Description = "A simple wooden raft for crossing water.",
        Ingredients =
        [
            new RecipeIngredient("log", 4),
            new RecipeIngredient("rope", 2)
        ],
        RequiredToolId = "axe",
        Results = [Vehicles.Boat()]  // Simplified - reuses boat
    };

    public static CraftingRecipe Sword() => new()
    {
        Id = "sword",
        Name = "Iron Sword",
        Description = "A basic iron sword.",
        Ingredients =
        [
            new RecipeIngredient("iron_bar", 3),
            new RecipeIngredient("leather", 1)
        ],
        RequiredToolId = "hammer",
        RequiredLocationTag = "forge",
        RequiredSkillLevel = 2,
        Results = [new Item("iron_sword", "Iron Sword")]
    };

    public static CraftingRecipe CookedMeat() => new()
    {
        Id = "cooked_meat",
        Name = "Cooked Meat",
        Description = "Properly cooked meat.",
        Ingredients =
        [
            new RecipeIngredient("raw_meat", 1)
        ],
        RequiredLocationTag = "fire",
        Results = [Foods.CookedMeat()]
    };
}
```

### Task 71.5: Auto-Discovery System

```csharp
public class RecipeDiscoverySystem
{
    private readonly HashSet<string> _discoveredRecipes = [];
    private readonly CraftingManager _crafting;

    public event Action<ICraftingRecipe>? OnRecipeDiscovered;

    public bool IsDiscovered(string recipeId) =>
        _discoveredRecipes.Contains(recipeId);

    public void DiscoverRecipe(string recipeId)
    {
        if (_discoveredRecipes.Add(recipeId))
        {
            var recipe = _crafting.GetRecipe(recipeId);
            if (recipe != null)
                OnRecipeDiscovered?.Invoke(recipe);
        }
    }

    public void TryDiscoverFromItems(IReadOnlyList<IItem> items)
    {
        var itemIds = items.Select(i => i.Id).ToList();
        var possibleRecipes = _crafting.FindRecipesWith(itemIds);

        foreach (var recipe in possibleRecipes)
        {
            // 30% chance to discover when you have all ingredients
            if (!IsDiscovered(recipe.Id) && Random.Shared.NextDouble() < 0.3)
            {
                DiscoverRecipe(recipe.Id);
            }
        }
    }

    public void DiscoverFromScroll(IItem scroll)
    {
        var recipeId = scroll.GetProperty<string>("teaches_recipe");
        if (recipeId != null)
        {
            DiscoverRecipe(recipeId);
        }
    }
}
```

### Task 71.6: Commands

```csharp
public class CraftCommand : ICommand
{
    public CommandResult Execute(CommandContext context, string recipeName)
    {
        var recipe = context.State.Crafting.AllRecipes
            .FirstOrDefault(r => r.Name.Contains(recipeName, StringComparison.OrdinalIgnoreCase));

        if (recipe == null)
            return CommandResult.Fail($"Unknown recipe: {recipeName}");

        if (!context.State.RecipeDiscovery.IsDiscovered(recipe.Id))
            return CommandResult.Fail("You haven't discovered that recipe yet.");

        var result = recipe.Craft(context.State.Player);
        return result.Success
            ? CommandResult.Ok(result.Message)
            : CommandResult.Fail(result.Message);
    }
}

public class RecipesCommand : ICommand
{
    public CommandResult Execute(CommandContext context)
    {
        var discovered = context.State.Crafting.AllRecipes
            .Where(r => context.State.RecipeDiscovery.IsDiscovered(r.Id))
            .ToList();

        if (!discovered.Any())
            return CommandResult.Ok("You haven't discovered any recipes yet.");

        var sb = new StringBuilder();
        sb.AppendLine("=== Known Recipes ===");

        foreach (var recipe in discovered)
        {
            var canCraft = recipe.CanCraft(context.State.Player, context.State.CurrentLocation);
            var status = canCraft ? "✓" : "✗";

            sb.AppendLine($"{status} {recipe.Name}");
            sb.AppendLine($"    Requires: {string.Join(", ", recipe.Ingredients.Select(i => $"{i.Quantity}x {i.ItemId}"))}");
        }

        return CommandResult.Ok(sb.ToString());
    }
}

public class CombineCommand : ICommand
{
    public CommandResult Execute(CommandContext context, string item1Name, string item2Name)
    {
        var item1 = context.State.Player.Inventory.FindItem(item1Name);
        var item2 = context.State.Player.Inventory.FindItem(item2Name);

        if (item1 == null || item2 == null)
            return CommandResult.Fail("You don't have those items.");

        // Find recipe that uses these two items
        var itemIds = new[] { item1.Id, item2.Id };
        var recipes = context.State.Crafting.FindRecipesWith(itemIds);

        var matchingRecipe = recipes.FirstOrDefault(r =>
            r.Ingredients.Count == 2 &&
            r.Ingredients.All(i => itemIds.Contains(i.ItemId)));

        if (matchingRecipe == null)
        {
            // Try to discover
            context.State.RecipeDiscovery.TryDiscoverFromItems([item1, item2]);
            return CommandResult.Fail("Those items can't be combined.");
        }

        // Auto-discover and craft
        context.State.RecipeDiscovery.DiscoverRecipe(matchingRecipe.Id);
        return matchingRecipe.Craft(context.State.Player).Success
            ? CommandResult.Ok($"You combine the items and create: {matchingRecipe.Results.First().Name}")
            : CommandResult.Fail("Crafting failed.");
    }
}
```

### Task 71.7: GameBuilder Integration

```csharp
var game = new GameBuilder("Crafting Demo")
    .WithCraftingSystem()
    .AddRecipe(Recipes.Torch())
    .AddRecipe(Recipes.Lockpick())
    .AddRecipe(Recipes.HealingSalve())
    .AddRecipe(Recipes.CookedMeat())
    .AddLocation("campsite", loc => loc
        .Name("Campsite")
        .Description("A clearing with a campfire.")
        .WithTag("fire")
        .AddItem("stick", item => item.Name("Wooden Stick"))
        .AddItem("stick", item => item.Name("Wooden Stick"))
        .AddItem("cloth", item => item.Name("Piece of Cloth"))
        .AddItem("raw_meat", Foods.RawMeat()))
    .AddLocation("forge", loc => loc
        .Name("Blacksmith's Forge")
        .Description("A hot forge with anvil and tools.")
        .WithTag("forge"))
    .AddItem("recipe_scroll", item => item
        .Name("Ancient Recipe Scroll")
        .Description("A scroll describing how to make a powerful sword.")
        .WithProperty("teaches_recipe", "sword")
        .OnUse((ctx, _) =>
        {
            ctx.State.RecipeDiscovery.DiscoverFromScroll(item);
            return "You study the scroll and learn a new recipe!";
        }))
    .Build();
```

### Task 71.8: Tester

```csharp
[Fact]
public void Recipe_CanBeCrafted_WithIngredients()
{
    var player = CreatePlayer();
    player.Inventory.Add(new Item("stick", "Stick"));
    player.Inventory.Add(new Item("cloth", "Cloth"));
    player.Inventory.Add(new Item("oil", "Oil"));

    var recipe = Recipes.Torch();
    var result = recipe.Craft(player);

    Assert.True(result.Success);
    Assert.Single(result.CreatedItems);
    Assert.Equal("Torch", result.CreatedItems.First().Name);
}

[Fact]
public void Recipe_FailsWithoutIngredients()
{
    var player = CreatePlayer();
    player.Inventory.Add(new Item("stick", "Stick"));
    // Missing cloth and oil

    var recipe = Recipes.Torch();

    Assert.False(recipe.CanCraft(player, CreateLocation()));
}

[Fact]
public void Recipe_ConsumesIngredients()
{
    var player = CreatePlayer();
    player.Inventory.Add(new Item("wire", "Wire"));
    player.Inventory.Add(new Item("wire", "Wire"));

    var recipe = Recipes.Lockpick();
    recipe.Craft(player);

    Assert.Equal(0, player.Inventory.CountItem("wire"));
}

[Fact]
public void Recipe_RequiresTool_NotConsumed()
{
    var player = CreatePlayer();
    player.Inventory.Add(new Item("log", "Log"));
    player.Inventory.Add(new Item("log", "Log"));
    player.Inventory.Add(new Item("log", "Log"));
    player.Inventory.Add(new Item("log", "Log"));
    player.Inventory.Add(new Item("rope", "Rope"));
    player.Inventory.Add(new Item("rope", "Rope"));
    player.Inventory.Add(new Item("axe", "Axe"));

    var recipe = Recipes.Raft();
    recipe.Craft(player);

    // Axe should still be in inventory
    Assert.True(player.Inventory.HasItem("axe"));
}

[Fact]
public void RecipeDiscovery_TriggersEvent()
{
    var discovery = new RecipeDiscoverySystem();
    var eventTriggered = false;

    discovery.OnRecipeDiscovered += _ => eventTriggered = true;
    discovery.DiscoverRecipe("torch");

    Assert.True(eventTriggered);
}
```

---
