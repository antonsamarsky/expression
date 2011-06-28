using System;
using System.Linq.Expressions;

namespace ExpressionMapper.Generators
{
	public static partial class Generators
		{
				/// <summary>
				/// Returns generator for mappings T to T?.
				/// </summary>
				public static Func<Expression, Type, Expression, Expression> ToNullable()
				{
						return (fromExpr, to, customMapping) =>
												(CanConvertToNullable(fromExpr.Type, to))
												? customMapping ?? Expression.Convert(fromExpr, to)
												: null;
				}

				/// <summary>
				/// Returns generator for mappings T? to T.
				/// </summary>
				public static Func<Expression, Type, Expression, Expression> FromNullable()
				{
						return (fromExpr, to, customMapping) =>
								(CanConvertFromNullable(fromExpr.Type, to))
								? customMapping ?? Expression.MakeMemberAccess(fromExpr, fromExpr.Type.GetProperty("Value"))
								: null;
				}

				private static bool CanConvertToNullable(Type from, Type to)
				{
						return from.IsValueType
										&& !(from.IsGenericType && from.GetGenericTypeDefinition() == typeof(Nullable<>))
										&& (to == typeof(Nullable<>).MakeGenericType(from));
				}

				private static bool CanConvertFromNullable(Type from, Type to)
				{
						return to.IsValueType
										&& !(to.IsGenericType && to.GetGenericTypeDefinition() == typeof(Nullable<>))
										&& (from == typeof(Nullable<>).MakeGenericType(to));
				}
		}

}
