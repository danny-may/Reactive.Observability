using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reactive.Linq;
using System.Reflection;
using Observability.Observables;

namespace Observability;

public static class Reactive
{
    private static readonly ReactiveProvider _default = new(new DefaultBinder());

    private sealed class DefaultBinder : IReactiveBinder
    {
        public WatchInstanceChanges<TInstance>? WatchInstance<TInstance>(MemberInfo member)
        {
            var name = member.Name;
            var instanceType = typeof(TInstance);
            if (instanceType.IsAssignableTo(typeof(IReactive)))
                return (src) => new ReactiveObservable<TInstance>(src, name);

            if (!HasNoDerivedTypes(instanceType))
            {
                if (member is not PropertyInfo)
                {
                    return (src) =>
                    {
                        if (src is IReactive)
                            return new ReactiveObservable<TInstance>(src, name);
                        return Observable.Return(src);
                    };
                }

                return (src) =>
                {
                    if (src is IReactive)
                        return new ReactiveObservable<TInstance>(src, name);

                    if (src is INotifyPropertyChanged)
                        return new PropertyChangedObservable<TInstance>(src, name);

                    return Observable.Return(src);
                };
            }
            if (
                member is PropertyInfo
                && instanceType.IsAssignableTo(typeof(INotifyPropertyChanged))
            )
                return (src) => new PropertyChangedObservable<TInstance>(src, name);

            return null;
        }

        public WatchExtensionChanges<TInstance>? WatchExtension<TInstance>(
            MethodInfo extensionMethod
        )
        {
            if (extensionMethod.DeclaringType != typeof(Enumerable))
                return null;

            var instanceType = typeof(TInstance);
            if (instanceType.IsAssignableTo(typeof(INotifyCollectionChanged)))
                return src => new CollectionChangedObservable<TInstance>(src);

            if (!HasNoDerivedTypes(instanceType))
            {
                return src =>
                {
                    if (src is INotifyCollectionChanged)
                        return new CollectionChangedObservable<TInstance>(src);

                    return Observable.Return(src);
                };
            }

            return null;
        }

        public WatchStaticChanges? WatchStatic(MemberInfo staticMember)
        {
            return null;
        }

        private static bool HasNoDerivedTypes(Type type)
        {
            return type.IsSealed
                || type.IsArray
                || type.IsValueType
                || type.IsEnum
                || type.IsPrimitive;
        }
    }

    public static IObservable<TResult?> Observe<TResult>(Expression<Func<TResult>> expression)
    {
        return _default.Observe(expression);
    }

    public static IObservable<TResult> Observe<TSource, TResult>(
        this IObservable<TSource> source,
        Expression<Func<TSource, TResult>> expression
    )
    {
        return _default.Observe(source, expression);
    }
}
