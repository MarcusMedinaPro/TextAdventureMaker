// <copyright file="Bond.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using MarcusMedina.TextAdventure.Interfaces;

namespace MarcusMedina.TextAdventure.Models;

public sealed class Bond(string id) : IBond
{
    private readonly HashSet<string> _required = new(StringComparer.OrdinalIgnoreCase);
    private readonly HashSet<string> _completed = new(StringComparer.OrdinalIgnoreCase);
    private Action<BondContext>? _payoff;

    public string Id { get; } = id ?? "";
    public int Strength => _completed.Count;
    public IReadOnlyCollection<string> RequiredInvestments => _required;
    public IReadOnlyCollection<string> CompletedInvestments => _completed;
    public IReadOnlyCollection<string> MissingInvestments => _required.Except(_completed).ToArray();
    public bool IsEstablished => _required.Count > 0 && _required.All(_completed.Contains);

    public IBond InvestmentMoments(params string[] moments)
    {
        if (moments == null)
        {
            return this;
        }

        foreach (string moment in moments)
        {
            if (!string.IsNullOrWhiteSpace(moment))
            {
                _required.Add(moment);
            }
        }

        return this;
    }

    public IBond RecordInvestment(string moment)
    {
        if (!string.IsNullOrWhiteSpace(moment))
        {
            _completed.Add(moment);
        }

        return this;
    }

    public IBond Payoff(Action<BondContext> payoff)
    {
        _payoff = payoff;
        return this;
    }

    public BondContext? TriggerPayoff(IGameState state, INpc npc)
    {
        if (_payoff == null)
        {
            return null;
        }

        BondContext context = new(state, npc, this);
        _payoff(context);
        return context;
    }
}
