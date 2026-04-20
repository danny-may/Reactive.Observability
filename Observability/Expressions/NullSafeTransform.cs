using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace Observability.Expressions;

internal sealed class NullSafeTransform : ExpressionVisitor
{
    private static readonly NullSafeTransform _instance = new();

    [return: NotNullIfNotNull(nameof(node))]
    public static Expression? Apply(Expression? node)
    {
        return _instance.Visit(node);
    }

    protected override Expression VisitMember(MemberExpression node)
    {
        if (node.Expression is null)
            return base.VisitMember(node);

        return InjectNullChecks(node);
    }

    protected override Expression VisitMethodCall(MethodCallExpression node)
    {
        if (node.Object is not null)
        {
            node = node.Update(node.Object, Visit(node.Arguments));
            return InjectNullChecks(node);
        }

        if (Attribute.IsDefined(node.Method, typeof(ExtensionAttribute)))
        {
            node = node.Update(
                node.Object,
                [node.Arguments[0], .. Visit(node.Arguments.Slice(1..))]
            );
            return InjectNullChecks(node);
        }

        return base.VisitMethodCall(node);
    }

    protected override Expression VisitInvocation(InvocationExpression node)
    {
        node = node.Update(node.Expression, Visit(node.Arguments));
        return InjectNullChecks(node);
    }

    protected override Expression VisitIndex(IndexExpression node)
    {
        if (node.Object is null)
            return base.VisitIndex(node);

        node = node.Update(node.Object, Visit(node.Arguments));
        return InjectNullChecks(node);
    }

    protected override Expression VisitUnary(UnaryExpression node)
    {
        if (node.NodeType != ExpressionType.ArrayLength)
            return base.VisitUnary(node);

        return InjectNullChecks(node);
    }

    private static Expression InjectNullChecks(Expression node)
    {
        var nullables = FindPossibleNullRefs(node).ToList();
        if (nullables.Count == 0)
            return node;

        var variables = nullables.Map(static x => Expression.Variable(x.Type));

        var @default = Expression.Default(node.Type);

        var replacements = new (Expression Nullable, ParameterExpression NotNull)[nullables.Count];
        for (var i = 0; i < replacements.Length; i++)
            replacements[i] = (nullables[i], variables[i]);

        var j = 0;
        var replacer = new ExpressionReplacer(x =>
        {
            if (j >= replacements.Length)
                return x;
            if (replacements[j].Nullable == x)
                return replacements[j].NotNull;
            return x;
        });

        var body = replacer.Visit(node);

        foreach (var (nullable, variable) in replacements)
        {
            j++;
            body = Expression.Condition(
                Expression.IsNull(Expression.Assign(variable, replacer.Visit(nullable))),
                @default,
                body
            );
        }

        return Expression.Block(node.Type, variables, body);
    }

    private static IEnumerable<Expression> FindPossibleNullRefs(Expression? node)
    {
        while (true)
        {
            node = node switch
            {
                MemberExpression m => m.Expression,
                IndexExpression i => i.Object,
                MethodCallExpression { Object: null } m
                    when Attribute.IsDefined(m.Method, typeof(ExtensionAttribute)) =>
                    m.Arguments.FirstOrDefault(),
                MethodCallExpression m => m.Object,
                UnaryExpression { NodeType: ExpressionType.ArrayLength } u => u.Operand,
                InvocationExpression i => i.Expression,
                _ => null,
            };

            if (node is null)
                break;

            if (IsNullable(node))
                yield return node;
        }
    }

    private static bool IsNullable(Expression expression)
    {
        var type = expression.Type;
        if (type.IsValueType)
            return Nullable.GetUnderlyingType(type) is not null;

        return !DisplayClass.IsDisplayClass(type);
    }
}
