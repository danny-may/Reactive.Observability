using System;

namespace Observability;

public interface IReactive
{
    IDisposable Watch(string? memberName, Action onChange);
}
