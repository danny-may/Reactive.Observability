using System;

namespace Observability;

public delegate IObservable<TInstance> WatchInstanceChanges<TInstance>(TInstance instance);
