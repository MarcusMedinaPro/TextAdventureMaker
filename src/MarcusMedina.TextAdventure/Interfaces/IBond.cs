// <copyright file="IBond.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using MarcusMedina.TextAdventure.Models;

namespace MarcusMedina.TextAdventure.Interfaces;

public interface IBond
{
    string Id { get; }
    int Strength { get; }
    IReadOnlyCollection<string> RequiredInvestments { get; }
    IReadOnlyCollection<string> CompletedInvestments { get; }
    IReadOnlyCollection<string> MissingInvestments { get; }
    bool IsEstablished { get; }
    IBond InvestmentMoments(params string[] moments);
    IBond RecordInvestment(string moment);
    IBond Payoff(Action<BondContext> payoff);
    BondContext? TriggerPayoff(IGameState state, INpc npc);
}
