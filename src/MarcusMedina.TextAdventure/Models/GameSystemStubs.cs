// <copyright file="GameSystemStubs.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Interfaces;

namespace MarcusMedina.TextAdventure.Models;

// Slice 061: Debug Console
public sealed class DebugConsole
{
    private bool _enabled;

    public bool IsEnabled => _enabled;

    public void Enable() => _enabled = true;
    public void Disable() => _enabled = false;

    public void Log(string message) => Console.WriteLine($"[DEBUG] {message}");
    public void Error(string message) => Console.WriteLine($"[ERROR] {message}");
    public void Warn(string message) => Console.WriteLine($"[WARN] {message}");
}

// Slice 062: Countdown/Deadlines
public sealed class DeadlineSystem
{
    private readonly Dictionary<string, int> _deadlines = [];

    public void SetDeadline(string id, int turns) => _deadlines[id] = turns;
    public int GetRemainingTurns(string id) => _deadlines.TryGetValue(id, out var t) ? t : 0;
    public void Tick() { foreach (var id in _deadlines.Keys.ToList()) _deadlines[id]--; }
}

// Slice 063: Chase System
public sealed class ChaseSystem
{
    public bool IsPlayerBeingChased { get; set; }
    public int ChaseDistance { get; set; }

    public bool UpdateChase() { return ChaseDistance > 0 && (ChaseDistance -= 1) > 0; }
}

// Slice 064: Status Effects
public sealed class StatusEffect
{
    public string Id { get; set; } = "";
    public string Name { get; set; } = "";
    public int TurnsRemaining { get; set; }
    public Dictionary<string, int> Modifiers { get; set; } = [];
}

public sealed class StatusEffectSystem
{
    private readonly Dictionary<string, List<StatusEffect>> _effects = [];

    public void ApplyEffect(string targetId, StatusEffect effect)
    {
        if (!_effects.ContainsKey(targetId)) _effects[targetId] = [];
        _effects[targetId].Add(effect);
    }

    public IEnumerable<StatusEffect> GetEffects(string targetId) =>
        _effects.TryGetValue(targetId, out var e) ? e : [];
}

// Slice 065: Test Helpers (minimal)
public sealed class TestGameBuilder
{
    public static Engine.GameState CreateTestGame() => new(new Location("test", "Test Room"));
}

// Slice 066: Weather/Environment
public sealed class WeatherSystem : IWeatherSystem
{
    public WeatherState Current { get; private set; } = WeatherState.Clear;

    public IWeatherSystem SetWeather(WeatherState state)
    {
        Current = state;
        return this;
    }

    public float GetVisibilityModifier() => Current switch
    {
        WeatherState.Clear => 1.0f,
        WeatherState.Rain => 0.8f,
        WeatherState.Storm => 0.5f,
        WeatherState.Fog => 0.4f,
        _ => 1.0f
    };
}

// Slice 067: Vehicles/Teleporters
public sealed class TransportSystem
{
    private readonly Dictionary<string, string> _routes = [];

    public void RegisterRoute(string from, string to) => _routes[from] = to;
    public string? Travel(string from) => _routes.TryGetValue(from, out var t) ? t : null;
}

// Slice 068: Stealth/Detection
public sealed class StealthSystem
{
    public float PlayerVisibility { get; set; } = 1.0f;  // 0 = invisible, 1 = visible
    public bool IsPlayerDetected(float detectionRange) => PlayerVisibility > (1.0f / detectionRange);
}

// Slice 069: Light/Darkness
public sealed class LightingSystem
{
    public LightLevel CurrentLight { get; set; } = LightLevel.Bright;

    public float GetVisibilityMultiplier() => CurrentLight switch
    {
        LightLevel.Dim => 0.5f,
        LightLevel.Bright => 1.5f,
        LightLevel.Dark => 0.1f,
        _ => 1.0f
    };
}

// Slice 070: Food/Hunger/Survival
public sealed class HungerSystem
{
    public int HungerLevel { get; set; } = 100;  // 0-100

    public void Eat(int amount) => HungerLevel = Math.Min(100, HungerLevel + amount);
    public void Tick() => HungerLevel = Math.Max(0, HungerLevel - 1);
    public bool IsStarving => HungerLevel < 20;
}

// Slice 071: Crafting
public sealed class Recipe
{
    public string Id { get; set; } = "";
    public string Name { get; set; } = "";
    public Dictionary<string, int> Ingredients { get; set; } = [];
    public string Output { get; set; } = "";
}

public sealed class CraftingSystem
{
    private readonly Dictionary<string, Recipe> _recipes = [];

    public void RegisterRecipe(Recipe recipe) => _recipes[recipe.Id] = recipe;
    public Recipe? GetRecipe(string id) => _recipes.TryGetValue(id, out var r) ? r : null;
    public IEnumerable<Recipe> GetAllRecipes() => _recipes.Values;
}

// Slice 072: Machines/Electronics
public sealed class Machine
{
    public string Id { get; set; } = "";
    public bool IsActive { get; set; }
    public string? Output { get; set; }
}

public sealed class MachineSystem
{
    private readonly Dictionary<string, Machine> _machines = [];

    public void RegisterMachine(Machine machine) => _machines[machine.Id] = machine;
    public Machine? GetMachine(string id) => _machines.TryGetValue(id, out var m) ? m : null;
    public void Activate(string id) { if (_machines.TryGetValue(id, out var m)) m.IsActive = true; }
}
