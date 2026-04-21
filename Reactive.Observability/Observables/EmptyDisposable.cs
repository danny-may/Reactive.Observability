using System;

namespace Reactive.Observability.Observables;

internal sealed class EmptyDisposable : IDisposable
{
    public static EmptyDisposable Instance { get; } = new();

    private EmptyDisposable() { }

    public void Dispose() { }
}
