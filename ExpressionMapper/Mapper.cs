using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using ExpressionMapper.MemberExtractors;

namespace ExpressionMapper
{
	using MappingExpressionGenerator = Func<Expression, Type, Expression, Expression>;

	/// <summary>
	/// Static facade for Mapper.
	/// </summary>
	public static class Mapper
	{
		private enum MappedMembers
		{
			AllFields,
			PublicMembers,
			PublicMembersWithFlattering
		}

		private static IEnumerable<MappingExpressionGenerator> Generators
		{
			get
			{
				return generators ?? (generators = new[]
				{
					ExpressionMapper.Generators.Generators.ToNullable(),
					ExpressionMapper.Generators.Generators.FromNullable(),
					ExpressionMapper.Generators.Generators.Assignable(),
					ExpressionMapper.Generators.Generators.UseConvertClass(),
					ExpressionMapper.Generators.Generators.UseIConvertible(),
					ExpressionMapper.Generators.Generators.ArrayToArray(Aggregate),
					ExpressionMapper.Generators.Generators.ListToList(Aggregate),
					ExpressionMapper.Generators.Generators.ComplexToComplex(Aggregate, MemberExtractors[MappedMembers.PublicMembersWithFlattering])
				});
			}
		}

		private static IEnumerable<MappingExpressionGenerator> generators;

		private static readonly MappingExpressionGenerator Aggregate;

		private static readonly Dictionary<MappedMembers, IMemberExtractor> MemberExtractors;

		static Mapper()
		{
			Aggregate = (fromExpr, to, customMapping) => 
				Generators.Select(f => f(fromExpr, to, customMapping)).FirstOrDefault(e => e != null);

			MemberExtractors = new Dictionary<MappedMembers, IMemberExtractor>
				{
						{MappedMembers.AllFields, new AllFieldsExtractor()},
						{MappedMembers.PublicMembers, new PublicMembersExtractor()},
						{MappedMembers.PublicMembersWithFlattering, new PublicMembersWithFlatteringExtractor()},
				};
		}

		/// <summary>
		/// Returns a Mapper instance for specified types.
		/// </summary>
		/// <typeparam name="TFrom">Type of source object</typeparam>
		/// <typeparam name="TTo">Type of destination object</typeparam>
		/// <returns></returns>
		public static Func<TFrom, TTo> Create<TFrom, TTo>()
		{
			return Create<TFrom, TTo>(null);
		}

		/// <summary>
		/// Returns a Mapper instance for specified types.
		/// </summary>
		/// <typeparam name="TFrom">Type of source object.</typeparam>
		/// <typeparam name="TTo">Type of destination object.</typeparam>
		/// <param name="customMapping">Lambda for custom mapping.</param>
		/// <returns></returns>
		public static Func<TFrom, TTo> Create<TFrom, TTo>(Expression<Func<TFrom, TTo>> customMapping)
		{
			return Aggregate.GetMapper(customMapping);
		}
	}
}
