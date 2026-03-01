using MarcusMedina.TextAdventure.AI.Contracts;
using MarcusMedina.TextAdventure.AI.Models;

internal sealed class SpinnerAiProviderRouter(IAiProviderRouter inner, AiConsoleSpinner spinner) : IAiProviderRouter
{
    private readonly IAiProviderRouter _inner = inner ?? throw new ArgumentNullException(nameof(inner));
    private readonly AiConsoleSpinner _spinner = spinner ?? throw new ArgumentNullException(nameof(spinner));

    public async Task<AiRoutingResult> RouteAsync(AiParseRequest request, CancellationToken cancellationToken = default)
    {
        using IDisposable _ = _spinner.Begin();
        return await _inner.RouteAsync(request, cancellationToken).ConfigureAwait(false);
    }
}
