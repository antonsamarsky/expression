using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Linq.Expressions;

namespace ExpressionMapper.MemberExtractors
{
	/// <summary>
	/// Extracts all public properties and fields. Applies recursively for members of complex types.
	/// </summary>
	public class PublicMembersWithFlatteringExtractor : IMemberExtractor
	{
		const BindingFlags Flags = BindingFlags.FlattenHierarchy | BindingFlags.Public | BindingFlags.Instance;

		public IEnumerable<MappingMember> GetFromMembers(Type t, IEnumerable<MappingMember> toMembers)
		{
			var toMemberNames = toMembers.Select(m => m.Name).ToList();
			return GetFromMembersAux(t, "", e => e, toMemberNames);
		}

		public IEnumerable<MappingMember> GetToMembers(Type t)
		{
			return t.GetProperties(Flags | BindingFlags.SetProperty)
							.Select(p => new MappingMember(p.Name, p.PropertyType, p))
							.Concat(t.GetFields(Flags)
											 .Select(f => new MappingMember(f.Name, f.FieldType, f)));
		}

		private IEnumerable<MappingMember> GetFromMembersAux(Type t, string namePrefix, Func<Expression, Expression> builder, IEnumerable<string> names)
		{
			var mappingMembers = new List<MappingMember>();
			if (IsTypeMatch(t))
			{
				foreach (var p in t.GetProperties(Flags | BindingFlags.GetProperty))
				{
					var name = namePrefix + p.Name;
					var member = p;
					Func<Expression, Expression> b = e => Expression.MakeMemberAccess(builder(e), member);
					mappingMembers.Add(new MappingMember(name, p.PropertyType, p, b));
				}

				foreach (var f in t.GetFields(Flags))
				{
					var name = namePrefix + f.Name;
					var member = f;
					Func<Expression, Expression> b = e => Expression.MakeMemberAccess(builder(e), member);
					mappingMembers.Add(new MappingMember(name, f.FieldType, f, b));
				}
			}
			mappingMembers.RemoveAll(m => !names.Any(n => n.StartsWith(m.Name)));
			return mappingMembers.Concat(mappingMembers
																			.SelectMany(m =>
																					GetFromMembersAux(m.Type, m.Name, m.GetMemberAccessExpression, names)));
		}

		private static bool IsTypeMatch(Type t)
		{
			return (t.IsClass && t != typeof(string))
							|| (t.IsValueType && !t.IsPrimitive && t != typeof(DateTime));
		}

	}
}
