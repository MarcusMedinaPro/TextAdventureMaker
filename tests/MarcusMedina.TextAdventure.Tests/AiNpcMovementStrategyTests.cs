// <copyright file="AiNpcMovementStrategyTests.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using MarcusMedina.TextAdventure.AI.Contracts;
using MarcusMedina.TextAdventure.AI.Features;
using MarcusMedina.TextAdventure.AI.Models;
using MarcusMedina.TextAdventure.AI.Plugin;
using MarcusMedina.TextAdventure.Engine;
using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Models;

namespace MarcusMedina.TextAdventure.Tests;

public class AiNpcMovementStrategyTests
{
    [Fact]
    public void GetNextLocation_PlayerDidNotTravel_DoesNotCallAi()
    {
        CountingRouter router = new("move:inn; reason:test");
        (GameState state, Location square, Location _) = CreateState();
        AiNpcMovementStrategy strategy = CreateStrategy(router);

        _ = strategy.GetNextLocation(square, state);
        _ = strategy.GetNextLocation(square, state);
        _ = strategy.GetNextLocation(square, state);

        Assert.Equal(0, router.RequestCount);
    }

    [Fact]
    public void GetNextLocation_PlayerTravelled_CallsAiForCurrentLocationNpc()
    {
        CountingRouter router = new("move:square; reason:test");
        (GameState state, Location square, Location inn) = CreateState();
        AiNpcMovementStrategy strategy = CreateStrategy(router);

        _ = strategy.GetNextLocation(square, state); // prime baseline location
        _ = state.Move(Direction.North);
        _ = strategy.GetNextLocation(inn, state);

        Assert.Equal(1, router.RequestCount);
    }

    private static AiNpcMovementStrategy CreateStrategy(IAiProviderRouter router)
    {
        Npc npc = new("watchman", "Watchman");
        AiFeatureModule module = AiFeatureModule.Create(router, options: new AiParserOptions());
        AiPluginOptions options = new()
        {
            EnableAiNpcMovement = true,
            NpcMovementAiOnlyInPlayerLocation = true,
            NpcMovementAiRequiresPlayerTravel = true,
            NpcMovementAiEveryTurns = 1,
            RuntimeFeatureTimeoutMs = 200
        };

        return new AiNpcMovementStrategy(npc, module, new NoNpcMovement(), options);
    }

    private static (GameState State, Location Square, Location Inn) CreateState()
    {
        Location square = new("square");
        Location inn = new("inn");
        square.AddExit(Direction.North, inn);
        inn.AddExit(Direction.South, square);
        GameState state = new(square, worldLocations: [square, inn]);
        return (state, square, inn);
    }

    private sealed class CountingRouter(string responseText) : IAiProviderRouter
    {
        public int RequestCount { get; private set; }

        public Task<AiRoutingResult> RouteAsync(AiParseRequest request, CancellationToken cancellationToken = default)
        {
            RequestCount++;
            AiRoutingResult response = AiRoutingResult.Success(
                AiProviderNames.OpenAi,
                responseText,
                [new AiProviderAttempt(AiProviderNames.OpenAi, AiAttemptOutcome.Success)]);

            return Task.FromResult(response);
        }
    }
}
