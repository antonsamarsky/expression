using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ExpressionMapper.MemberExtractors
{
	/// <summary>
	/// Extracts all public properties and fields.
	/// </summary>
	public class PublicMembersExtractor : IMemberExtractor
	{
		const BindingFlags Flags = BindingFlags.FlattenHierarchy | BindingFlags.Public | BindingFlags.Instance;
		public IEnumerable<MappingMember> GetFromMembers(Type t, IEnumerable<MappingMember> toMembers)
		{
			return t.GetProperties(Flags | BindingFlags.GetProperty)
							.Select(p => new MappingMember(p.Name, p.PropertyType, p))
							.Concat(t.GetFields(Flags)
											 .Select(f => new MappingMember(f.Name, f.FieldType, f)));
		}

		public IEnumerable<MappingMember> GetToMembers(Type t)
		{
			return t.GetProperties(Flags | BindingFlags.SetProperty)
							.Select(p => new MappingMember(p.Name, p.PropertyType, p))
							.Concat(t.GetFields(Flags)
											 .Select(f => new MappingMember(f.Name, f.FieldType, f)));
		}

	}
}
