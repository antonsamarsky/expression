using System;
using System.Linq.Expressions;

namespace ExpressionMapper.Generators
{
	/// <summary>
		/// Container class for mapper generators.
		/// </summary>
		public static partial class Generators  
		{
				/// <summary>
				/// Returns generator for assignable types.
				/// </summary>
				public static Func<Expression, Type, Expression, Expression> Assignable()
				{
						return (fromExpr, to, customMapping) => 
												to.IsAssignableFrom(fromExpr.Type) 
												? (customMapping ?? fromExpr) 
												: null;
				}
		}
}
