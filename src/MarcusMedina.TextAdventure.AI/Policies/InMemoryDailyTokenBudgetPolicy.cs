// <copyright file="InMemoryDailyTokenBudgetPolicy.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace MarcusMedina.TextAdventure.AI.Policies;

public sealed class InMemoryDailyTokenBudgetPolicy : ITokenBudgetPolicy
{
    private readonly Dictionary<string, int> _dailyLimits = new(StringComparer.OrdinalIgnoreCase);
    private readonly Dictionary<string, int> _dailyUsage = new(StringComparer.OrdinalIgnoreCase);
    private readonly Func<DateTimeOffset> _clock;
    private readonly object _gate = new();
    private DateOnly _currentDay;

    public InMemoryDailyTokenBudgetPolicy(Func<DateTimeOffset>? clock = null)
    {
        _clock = clock ?? (() => DateTimeOffset.UtcNow);
        _currentDay = DateOnly.FromDateTime(_clock().UtcDateTime);
    }

    public InMemoryDailyTokenBudgetPolicy SetDailyLimit(string providerName, int tokens)
    {
        if (string.IsNullOrWhiteSpace(providerName))
            return this;

        lock (_gate)
        {
            ResetIfNewDay();
            _dailyLimits[providerName] = Math.Max(0, tokens);
        }

        return this;
    }

    public bool CanUse(string providerName, int estimatedTokens)
    {
        if (string.IsNullOrWhiteSpace(providerName))
            return false;

        lock (_gate)
        {
            ResetIfNewDay();

            if (!_dailyLimits.TryGetValue(providerName, out int limit))
                return true;

            int usage = _dailyUsage.GetValueOrDefault(providerName);
            return usage + Math.Max(0, estimatedTokens) <= limit;
        }
    }

    public void TrackUsage(string providerName, int usedTokens)
    {
        if (string.IsNullOrWhiteSpace(providerName) || usedTokens <= 0)
            return;

        lock (_gate)
        {
            ResetIfNewDay();
            _dailyUsage[providerName] = _dailyUsage.GetValueOrDefault(providerName) + usedTokens;
        }
    }

    private void ResetIfNewDay()
    {
        DateOnly today = DateOnly.FromDateTime(_clock().UtcDateTime);
        if (today == _currentDay)
            return;

        _currentDay = today;
        _dailyUsage.Clear();
    }
}
