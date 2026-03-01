// <copyright file="IDramaticIronySystem.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace MarcusMedina.TextAdventure.Interfaces;

public interface IDramaticIronySystem
{
    void PlayerLearn(string secretId);
    void NpcLearn(INpc npc, string secretId);
    void RegisterAction(string secretId, string actionId);
    IReadOnlyCollection<string> GetAvailableActions();
    IReadOnlyCollection<string> GetGaps(INpc npc);
    bool Exists();
    bool ExistsForNpc(INpc npc);
}
