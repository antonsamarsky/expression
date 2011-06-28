using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace ExpressionMapper.Generators
{
	/// <summary>
    /// Returns generator for List to List mappings.
    /// </summary>
    public static partial class Generators
    {
        public static Func<Expression, Type, Expression, Expression> ListToList(Func<Expression, Type, Expression, Expression> parent)
        {
            return (fromExpr, to, customMapping) =>
                    (IsList(fromExpr.Type) && IsList(to))
                    ? customMapping ?? ListToListExpressionGenerator(parent, fromExpr, to)
                    : null;
        }

        private static Expression ListToListExpressionGenerator(Func<Expression, Type, Expression, Expression> parent, Expression fromExpr, Type to)
        {
            var fromElementType = GetListElementType(fromExpr.Type);
            var toElementType = GetListElementType(to);

            var converterType = typeof(Converter<,>).MakeGenericType(fromElementType, toElementType);

            var p = Expression.Parameter(fromElementType, "item");
            var elementLambda = Expression.Lambda(converterType, parent(p, toElementType, null), p);
            var convertAllMethod = typeof(List<>)
                                    .MakeGenericType(fromElementType)
                                    .GetMethod("ConvertAll", BindingFlags.Instance | BindingFlags.Public)
                                    .MakeGenericMethod(toElementType);
            return Expression.Call(fromExpr, convertAllMethod, elementLambda);

        }

        private static bool IsList(Type t)
        {
            return t.IsGenericType && t.GetGenericTypeDefinition() == typeof(List<>);
        }

        private static Type GetListElementType(Type t)
        {
            return t.GetGenericArguments()[0];
        }
    }
}
