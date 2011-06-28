using System;
using System.Reflection;
using System.Linq.Expressions;

namespace ExpressionMapper.Generators
{
	public static partial class Generators
	{
		/// <summary>
		/// Returns generator that uses Convert class for mapping.
		/// </summary>
		public static Func<Expression, Type, Expression, Expression> UseConvertClass()
		{
			return (fromExpr, to, customMapping) =>
							(SupportedByConvertClass(fromExpr.Type) && SupportedByConvertClass(to))
							? customMapping ?? Expression.Call(GetConvertClassMethod(fromExpr.Type, to), fromExpr)
							: null;
		}

		private static MethodInfo GetConvertClassMethod(Type from, Type to)
		{
			var contype = typeof(Convert);
			return contype.GetMethod("To" + to.Name, BindingFlags.Static | BindingFlags.Public,
																			null, new[] { from }, new ParameterModifier[0]);
		}

		private static bool SupportedByConvertClass(Type t)
		{
			return (t.IsPrimitive && !t.IsPointer)
					|| (t == typeof(DateTime)) || (t == typeof(String));
		}

	}
}
