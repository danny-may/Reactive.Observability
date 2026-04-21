using System;
using Reactive.Observability.Observables;

namespace Reactive.Observability;

public delegate IObservable<Nothing> WatchStaticChanges();
