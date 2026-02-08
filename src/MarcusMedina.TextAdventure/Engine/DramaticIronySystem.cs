// <copyright file="DramaticIronySystem.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using MarcusMedina.TextAdventure.Interfaces;

namespace MarcusMedina.TextAdventure.Engine;

public sealed class DramaticIronySystem : IDramaticIronySystem
{
    private readonly HashSet<string> _playerSecrets = new(StringComparer.OrdinalIgnoreCase);
    private readonly Dictionary<string, HashSet<string>> _npcSecrets = new(StringComparer.OrdinalIgnoreCase);
    private readonly Dictionary<string, HashSet<string>> _actions = new(StringComparer.OrdinalIgnoreCase);

    public void PlayerLearn(string secretId)
    {
        if (!string.IsNullOrWhiteSpace(secretId))
        {
            _playerSecrets.Add(secretId);
        }
    }

    public void NpcLearn(INpc npc, string secretId)
    {
        if (npc == null || string.IsNullOrWhiteSpace(secretId))
        {
            return;
        }

        if (!_npcSecrets.TryGetValue(npc.Id, out HashSet<string>? secrets))
        {
            secrets = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            _npcSecrets[npc.Id] = secrets;
        }

        secrets.Add(secretId);
    }

    public void RegisterAction(string secretId, string actionId)
    {
        if (string.IsNullOrWhiteSpace(secretId) || string.IsNullOrWhiteSpace(actionId))
        {
            return;
        }

        if (!_actions.TryGetValue(secretId, out HashSet<string>? actions))
        {
            actions = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            _actions[secretId] = actions;
        }

        actions.Add(actionId);
    }

    public IReadOnlyCollection<string> GetAvailableActions()
    {
        HashSet<string> available = new(StringComparer.OrdinalIgnoreCase);

        foreach (string secret in _playerSecrets)
        {
            if (!HasGap(secret))
            {
                continue;
            }

            if (_actions.TryGetValue(secret, out HashSet<string>? actions))
            {
                available.UnionWith(actions);
            }
        }

        return available.ToArray();
    }

    public IReadOnlyCollection<string> GetGaps(INpc npc)
    {
        if (npc == null)
        {
            return Array.Empty<string>();
        }

        HashSet<string> npcKnowledge = _npcSecrets.TryGetValue(npc.Id, out HashSet<string>? secrets)
            ? secrets
            : [];

        return _playerSecrets
            .Where(secret => !npcKnowledge.Contains(secret))
            .ToArray();
    }

    public bool Exists()
    {
        return _playerSecrets.Any(HasGap);
    }

    public bool ExistsForNpc(INpc npc)
    {
        return npc != null && GetGaps(npc).Count > 0;
    }

    private bool HasGap(string secretId)
    {
        foreach (HashSet<string> secrets in _npcSecrets.Values)
        {
            if (!secrets.Contains(secretId))
            {
                return true;
            }
        }

        return _npcSecrets.Count == 0;
    }
}
