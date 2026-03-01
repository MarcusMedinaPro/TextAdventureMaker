// <copyright file="IAiDescriptionCache.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace MarcusMedina.TextAdventure.AI.Contracts;

public interface IAiDescriptionCache
{
    bool TryGet(string key, out string description);
    void Set(string key, string description);
    void Clear();
}
