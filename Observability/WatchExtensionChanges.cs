using System;

namespace Observability;

public delegate IObservable<TInstance> WatchExtensionChanges<TInstance>(TInstance instance)
    where TInstance : notnull;
