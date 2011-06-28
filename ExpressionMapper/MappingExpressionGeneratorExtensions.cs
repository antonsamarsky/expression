using System;
using System.Linq.Expressions;

namespace ExpressionMapper
{
	using MappingExpressionGenerator = Func<Expression, Type, Expression, Expression>;

	public static class MappingExpressionGeneratorExtensions
	{
		public static Func<TFrom, TTo> GetMapper<TFrom, TTo>(this MappingExpressionGenerator generator, Expression<Func<TFrom, TTo>> customMapping)
		{
			var fromType = typeof(TFrom);
			var toType = typeof(TTo);
			var p = Expression.Parameter(fromType, "p");
			Expression customMappingBody = null;

			if (customMapping != null)
			{
				customMappingBody = IQToolkit.ExpressionReplacer.Replace(customMapping.Body, customMapping.Parameters[0], p);
			}
			var resultExpression = generator(p, toType, customMappingBody);
			if (resultExpression == null)
			{
				throw new InvalidOperationException(string.Format("Can't map from {0} to {1}", typeof(TFrom).Name, typeof(TTo).Name));
			}
			return Expression.Lambda<Func<TFrom, TTo>>(resultExpression, p).Compile();
		}
	}
}
