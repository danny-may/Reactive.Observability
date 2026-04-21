using System;

namespace Reactive.Observability.Observables;

public sealed class ConstantObservable<T>(T value) : IObservable<T>
{
    public IDisposable Subscribe(IObserver<T> observer)
    {
        observer.OnNext(value);
        observer.OnCompleted();
        return EmptyDisposable.Instance;
    }
}
