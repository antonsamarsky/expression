using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ExpressionMapper.MemberExtractors
{
	/// <summary>
	/// Extracts all fields for mapping.
	/// </summary>
	public class AllFieldsExtractor : IMemberExtractor
	{
		public IEnumerable<MappingMember> GetFromMembers(Type t, IEnumerable<MappingMember> toMembers)
		{
			return GetToMembers(t);
		}

		public IEnumerable<MappingMember> GetToMembers(Type t)
		{
			return t.GetFields(BindingFlags.FlattenHierarchy | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
							.Select(f => new MappingMember(f.Name, f.FieldType, f));
		}
	}
}
