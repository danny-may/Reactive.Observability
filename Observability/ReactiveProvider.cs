using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reactive.Linq;
using System.Runtime.CompilerServices;
using Observability.Expressions;

namespace Observability;

public sealed class ReactiveProvider(IReactiveBinder binder)
{
    private static readonly ConcurrentDictionary<Expression, Delegate> _binders = new(
        ReferenceEqualityComparer.Instance
    );

    public IObservable<TResult?> Observe<TResult>(Expression<Func<TResult>> expression)
    {
        expression = ExpressionNormalizer.Normalize(expression, out var context, out var constants);
        var binderFn = _binders.GetOrAdd(expression.Body, CompileParameterless, context);
        return Unsafe.As<IObservable<TResult>>(binderFn.DynamicInvoke(constants)!);
    }

    public IObservable<TResult> Observe<TSource, TResult>(
        IObservable<TSource> source,
        Expression<Func<TSource, TResult>> expression
    )
    {
        expression = ExpressionNormalizer.Normalize(expression, out var context, out var constants);
        var binderFn = _binders.GetOrAdd(
            expression.Body,
            CompileParameterized,
            (context, expression.Parameters)
        );
        var selector = Unsafe.As<Func<TSource, IObservable<TResult>>>(
            binderFn.DynamicInvoke(constants)!
        );
        return source.Select(selector).Switch();
    }

    private Delegate CompileParameterless(Expression body, ParameterExpression context)
    {
        var observable = ReactiveRewriter.Rewrite(body, binder, [context]);
        var lambda = Expression.Lambda(
            TypeLookup.Delegate([context.Type], observable.Type),
            observable,
            [context]
        );
        return lambda.Compile();
    }

    private Delegate CompileParameterized(
        Expression body,
        (ParameterExpression, ReadOnlyCollection<ParameterExpression>) context
    )
    {
        var observable = ReactiveRewriter.Rewrite(body, binder, [context.Item1, .. context.Item2]);
        var boundLambda = Expression.Lambda(
            TypeLookup.Delegate(context.Item2.Map(static x => x.Type), observable.Type),
            observable,
            [.. context.Item2]
        );
        var lambda = Expression.Lambda(
            TypeLookup.Delegate([context.Item1.Type], boundLambda.Type),
            boundLambda,
            [context.Item1]
        );
        return lambda.Compile();
    }
}
