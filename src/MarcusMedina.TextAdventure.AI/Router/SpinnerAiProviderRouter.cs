// <copyright file="SpinnerAiProviderRouter.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace MarcusMedina.TextAdventure.AI.Router;

/// <summary>Wraps an <see cref="IAiProviderRouter"/> to show a spinner while the call is in flight.</summary>
public sealed class SpinnerAiProviderRouter(IAiProviderRouter inner, IAiSpinner spinner) : IAiProviderRouter
{
    private readonly IAiProviderRouter _inner = inner ?? throw new ArgumentNullException(nameof(inner));
    private readonly IAiSpinner _spinner = spinner ?? throw new ArgumentNullException(nameof(spinner));

    public async Task<AiRoutingResult> RouteAsync(AiParseRequest request, CancellationToken cancellationToken = default)
    {
        using IDisposable _ = _spinner.Begin();
        return await _inner.RouteAsync(request, cancellationToken).ConfigureAwait(false);
    }
}
