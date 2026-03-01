// <copyright file="AiNpcMovementStrategy.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using MarcusMedina.TextAdventure.Extensions;
using MarcusMedina.TextAdventure.Interfaces;

namespace MarcusMedina.TextAdventure.AI.Plugin;

public sealed class AiNpcMovementStrategy(
    INpc npc,
    AiFeatureModule module,
    INpcMovement fallback,
    AiPluginOptions? options = null) : INpcMovement
{
    private readonly INpc _npc = npc ?? throw new ArgumentNullException(nameof(npc));
    private readonly AiFeatureModule _module = module ?? throw new ArgumentNullException(nameof(module));
    private readonly INpcMovement _fallback = fallback ?? throw new ArgumentNullException(nameof(fallback));
    private readonly AiPluginOptions _options = options ?? new AiPluginOptions();
    private int _turnCounter;
    private string? _lastPlayerLocationId;

    public ILocation? GetNextLocation(ILocation currentLocation, IGameState state)
    {
        ArgumentNullException.ThrowIfNull(currentLocation);
        ArgumentNullException.ThrowIfNull(state);

        if (_options.NpcMovementAiOnlyInPlayerLocation
            && !currentLocation.Id.TextCompare(state.CurrentLocation.Id))
            return _fallback.GetNextLocation(currentLocation, state);

        if (_options.NpcMovementAiRequiresPlayerTravel)
        {
            if (string.IsNullOrWhiteSpace(_lastPlayerLocationId))
            {
                _lastPlayerLocationId = state.CurrentLocation.Id;
                return _fallback.GetNextLocation(currentLocation, state);
            }

            bool playerTravelled = !string.Equals(_lastPlayerLocationId, state.CurrentLocation.Id, StringComparison.OrdinalIgnoreCase);
            _lastPlayerLocationId = state.CurrentLocation.Id;
            if (!playerTravelled)
                return _fallback.GetNextLocation(currentLocation, state);
        }
        else
        {
            _lastPlayerLocationId = state.CurrentLocation.Id;
        }

        int interval = Math.Max(1, _options.NpcMovementAiEveryTurns);
        _turnCounter++;
        if (_turnCounter % interval != 0)
            return _fallback.GetNextLocation(currentLocation, state);

        NpcMovementContext context = AiPluginContextFactory.BuildNpcMovementContext(state, _npc, currentLocation);
        int timeoutMs = Math.Max(250, _options.RuntimeFeatureTimeoutMs);
        using CancellationTokenSource cts = new(TimeSpan.FromMilliseconds(timeoutMs));
        NpcMovementDecision? decision = _module.NpcMovement
            .ChooseNextLocationAsync(context, cts.Token)
            .GetAwaiter()
            .GetResult();

        if (decision != null && !string.IsNullOrWhiteSpace(decision.NextLocationId))
        {
            ILocation? next = currentLocation.Exits.Values
                .Where(exit => exit.IsVisible && exit.IsPassable)
                .Select(exit => exit.Target)
                .FirstOrDefault(target => target.Id.TextCompare(decision.NextLocationId));

            if (next != null && !ReferenceEquals(next, currentLocation))
                return next;
        }

        return _fallback.GetNextLocation(currentLocation, state);
    }
}
