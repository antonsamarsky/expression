using System;
using System.Linq;
using System.Linq.Expressions;

namespace ExpressionMapper.Generators
{
	public static partial class Generators
		{

				/// <summary>
				/// Returns generator that uses IConvertible interface for mapping.
				/// </summary>
				public static Func<Expression, Type, Expression, Expression> UseIConvertible()
				{
						return (fromExpr, to, customMapping) =>
										(ImplementsIConvertible(fromExpr.Type) && SupportedByIConvertible(to))
										? customMapping ?? Expression.Call(fromExpr, 
																											 GetIConvertibleMethod(fromExpr.Type, to), 
																											 Expression.Constant(null))
										: null;
				}

				private static System.Reflection.MethodInfo GetIConvertibleMethod(Type from, Type to)
				{
						var map = from.GetInterfaceMap(typeof(IConvertible));
						return map.TargetMethods.First(mi => mi.Name == "To" + to.Name);
				}

				private static bool ImplementsIConvertible(Type t)
				{
						return t.GetInterfaces().Contains(typeof(IConvertible));
				}
				private static bool SupportedByIConvertible(Type t)
				{
						return (t.IsPrimitive && !t.IsPointer)
								|| (t == typeof(DateTime)) || (t == typeof(String));
				}

		}
}
