using System;

namespace Observability.Observables;

public sealed class ReactiveObservable<T>(T source, string? memberName)
    : WatchChangeObservable<T>(source)
{
    protected override IDisposable? Subscribe(T source, Action onChange)
    {
        return (source as IReactive)?.Watch(memberName, onChange);
    }
}
