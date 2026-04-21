using System;
using System.Linq.Expressions;
using Observability.Binding;

namespace Observability;

public static partial class Reactive
{
    public static ReactiveProvider Provider { get; } =
        new(
            new ReactiveBinder(
                new DefaultReactiveBinderItem(),
                new NotifyPropertyChangedBinderItem(),
                new NotifyCollectionChangedBinderItem()
            )
        );

    /// <inheritdoc cref="ReactiveProvider.Build{TDelegate}(Expression{TDelegate})"/>
    public static Delegate Build<TDelegate>(Expression<TDelegate> expression)
        where TDelegate : Delegate
    {
        return Provider.Build(expression);
    }
}
