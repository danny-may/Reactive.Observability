using System;
using System.Collections.Concurrent;
using System.Linq.Expressions;
using System.Reflection;

namespace Observability.Expressions;

internal static class ExprUtil
{
    private static readonly Expression _nullLiteral = Expression.Constant(null);
    private static readonly ConcurrentDictionary<Type, MethodInfo> _getValueOrDefault = [];

    extension(Expression)
    {
        public static Expression IsNull(Expression value)
        {
            if (!value.Type.IsValueType)
                return Expression.ReferenceEqual(value, _nullLiteral);
            if (Nullable.GetUnderlyingType(value.Type) is null)
                return Expression.Constant(false);
            return Expression.Equal(value, Expression.Constant(null, value.Type));
        }

        public static Expression NonNull(Expression value)
        {
            if (!value.Type.IsValueType)
                return value;
            if (Nullable.GetUnderlyingType(value.Type) is not { } notNull)
                return value;

            return Expression.Call(
                value,
                _getValueOrDefault.GetOrAdd(
                    notNull,
                    static t =>
                        typeof(Nullable<>)
                            .MakeGenericType(t)
                            .GetMethod(nameof(Nullable<>.GetValueOrDefault), [])!
                )
            );
        }
    }
}
