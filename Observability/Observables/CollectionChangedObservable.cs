using System;
using System.Collections.Specialized;
using System.Reactive.Disposables;

namespace Observability.Observables;

public sealed class CollectionChangedObservable<T>(T source) : WatchChangeObservable<T>(source)
{
    protected override IDisposable? Subscribe(T source, Action onChange)
    {
        if (source is not INotifyCollectionChanged notify)
            return null;

        notify.CollectionChanged += OnCollectionChanged;
        return Disposable.Create(() => notify.CollectionChanged -= OnCollectionChanged);
        void OnCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e) => onChange();
    }
}
