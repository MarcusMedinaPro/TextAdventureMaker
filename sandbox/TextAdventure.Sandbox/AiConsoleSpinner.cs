using System.Threading;

internal sealed class AiConsoleSpinner : IDisposable
{
    private static readonly char[] Frames = ['|', '/', '-', '\\'];

    private readonly bool _enabled;
    private readonly object _gate = new();
    private CancellationTokenSource? _cts;
    private Task? _loopTask;
    private int _activeRequests;

    public AiConsoleSpinner(bool enabled = true)
    {
        _enabled = enabled && !Console.IsOutputRedirected;
    }

    public IDisposable Begin()
    {
        if (!_enabled)
            return SpinnerScope.Noop;

        lock (_gate)
        {
            _activeRequests++;
            if (_activeRequests == 1)
                StartLoop();
        }

        return new SpinnerScope(this);
    }

    public void Dispose()
    {
        if (!_enabled)
            return;

        lock (_gate)
        {
            _activeRequests = 0;
            StopLoop();
        }
    }

    private void End()
    {
        if (!_enabled)
            return;

        lock (_gate)
        {
            if (_activeRequests == 0)
                return;

            _activeRequests--;
            if (_activeRequests == 0)
                StopLoop();
        }
    }

    private void StartLoop()
    {
        _cts = new CancellationTokenSource();
        CancellationToken token = _cts.Token;
        _loopTask = Task.Run(async () =>
        {
            int index = 0;
            while (!token.IsCancellationRequested)
            {
                Console.Write($"\rAI {Frames[index]} ");
                index = (index + 1) % Frames.Length;

                try
                {
                    await Task.Delay(90, token).ConfigureAwait(false);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
            }
        }, token);
    }

    private void StopLoop()
    {
        _cts?.Cancel();
        try
        {
            _loopTask?.GetAwaiter().GetResult();
        }
        catch (OperationCanceledException)
        {
            // Expected when the spinner stops.
        }

        Console.Write("\r      \r");
        _loopTask = null;
        _cts?.Dispose();
        _cts = null;
    }

    private sealed class SpinnerScope : IDisposable
    {
        public static SpinnerScope Noop { get; } = new(null);

        private readonly AiConsoleSpinner? _spinner;
        private bool _disposed;

        public SpinnerScope(AiConsoleSpinner? spinner)
        {
            _spinner = spinner;
        }

        public void Dispose()
        {
            if (_disposed)
                return;

            _disposed = true;
            _spinner?.End();
        }
    }
}
