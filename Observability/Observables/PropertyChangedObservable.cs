using System;
using System.ComponentModel;
using System.Reactive.Disposables;

namespace Observability.Observables;

public sealed class PropertyChangedObservable<T>(T source, string? propertyName)
    : WatchChangeObservable<T>(source)
{
    protected override IDisposable? Subscribe(T source, Action onChange)
    {
        if (source is not INotifyPropertyChanged notify)
            return null;

        if (propertyName is null)
        {
            notify.PropertyChanged += OnAnyPropertyChanged;
            return Disposable.Create(() => notify.PropertyChanged -= OnAnyPropertyChanged);
            void OnAnyPropertyChanged(object? sender, PropertyChangedEventArgs e) => onChange();
        }

        notify.PropertyChanged += OnPropertyChanged;
        return Disposable.Create(() => notify.PropertyChanged -= OnPropertyChanged);
        void OnPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (
                e.PropertyName is null
                || e.PropertyName.Equals(propertyName, StringComparison.Ordinal)
            )
            {
                onChange();
            }
        }
    }
}
