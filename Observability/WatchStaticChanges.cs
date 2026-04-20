using System;
using System.Reactive;

namespace Observability;

public delegate IObservable<Unit> WatchStaticChanges();
