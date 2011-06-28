using System;
using System.Reflection;
using System.Linq.Expressions;

namespace ExpressionMapper.Generators
{
	public static partial class Generators
		{
				/// <summary>
				/// Returns generator for array to array mappings.
				/// </summary>
				public static Func<Expression, Type, Expression, Expression> ArrayToArray(Func<Expression, Type, Expression, Expression> parent)
				{
						return (fromExpr, to, customMapping) =>
												(fromExpr.Type.IsArray && to.IsArray)
												? customMapping ?? ArrayToArrayExpressionGenerator(parent, fromExpr, to)
												: null;
				}

				private static Expression ArrayToArrayExpressionGenerator(Func<Expression, Type, Expression, Expression> parent, Expression fromExpr, Type to)
				{
						var fromElementType = fromExpr.Type.GetElementType();
						var toElementType = to.GetElementType();

						var converterType = typeof(Converter<,>).MakeGenericType(fromElementType, toElementType);

						var p = Expression.Parameter(fromElementType, "item");
						var elementLambda = Expression.Lambda(converterType, parent(p, toElementType, null), p);
						var convertAllMethod = typeof(Array)
																		.GetMethod("ConvertAll", BindingFlags.Static | BindingFlags.Public)
																		.MakeGenericMethod(fromElementType, toElementType);
						return Expression.Call(convertAllMethod, fromExpr, elementLambda);

				}

		}
}
